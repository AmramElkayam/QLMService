using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;

namespace QLMService
{
    public class NetworkScheduler
    {

      
        LimitedConcurrencyLevelTaskScheduler lcts;
        List<Task> tasks = new List<Task>();
        TaskFactory factory;
        CancellationTokenSource cts;
        readonly Object lockObj = new Object();
    
        public NetworkScheduler()
        {
            // Create a scheduler that uses two threads. 
            lcts = new LimitedConcurrencyLevelTaskScheduler(2);
            // Create a TaskFactory and pass it our custom scheduler. 
            factory = new TaskFactory(lcts);
            cts = new CancellationTokenSource();

        }





        public  List<string> GetAsync(string uri)
        {

            List<string> result = new List<string>();
            HttpClient httpClient = new HttpClient();
            string contentStr = httpClient.GetStringAsync(uri).GetAwaiter().GetResult();
            
            JArray content = JArray.Parse(contentStr);
            foreach (var country in content[1])
            {

                result.Add(country.ToString());
            }


            return result;
        }



        public List<string> RunClient(string address)
        {
            
            List<string> result = new List<string>();

           
                // Create an HttpClient instance
                HttpClient client = new HttpClient();
                // Send a request asynchronously and continue when complete
                client.GetAsync(address).ContinueWith(
                    (getTask) =>
                    {
                        if (getTask.IsCanceled)
                        {
                            return;
                        }
                        if (getTask.IsFaulted)
                        {
                            throw getTask.Exception;
                        }
                        HttpResponseMessage response = getTask.Result;

                        try
                        {
                            // Check that response was successful or throw exception
                            response.EnsureSuccessStatusCode();

                            // Read response asynchronously as JToken and write out top facts for each country
                            response.Content.ReadAsStringAsync().ContinueWith(
                                (contentTask) =>
                                {
                                    if (contentTask.IsCanceled)
                                    {
                                        return;
                                    }
                                    if (contentTask.IsFaulted)
                                    {
                                        throw contentTask.Exception;
                                    }

                                    JArray content = JArray.Parse(contentTask.Result);


                                    foreach (var country in content[1])
                                    {

                                        result.Add(country.ToString());
                                    }

                                    if (result.Count > 0) Console.WriteLine(result.ElementAt(0));
                                    

                                });
                        }
                        catch (HttpRequestException e)
                        {
                            // Handle exception.

                            Console.WriteLine("exception!!!");
                        }
                    });
            

            return result;
        }




        public void RunPostAsync(string address, string jsonContent)
        {

            Task t = factory.StartNew(() => DoPostWork(address, jsonContent), cts.Token);
            tasks.Add(t); // ???

        }

        private static async void DoPostWork(string address, string jsonContent) 
        {

            using (var client = new HttpClient())
            {
                //client.BaseAddress = new Uri(address); // absolute address passed by GetAsync or PostAsync
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                try
                {
                    //todo : change conseft to support DataContract and then run:
                    // MediaTypeFormatter jsonFormatter = new JsonMediaTypeFormatter();
                    //HttpResponseMessage response = await client.PostAsync(address,object to serilaize as json, jsonFormatter);
                    // see http://www.asp.net/web-api/overview/formats-and-model-binding/json-and-xml-serialization 

                    HttpResponseMessage response = await client.PostAsync(address, new StringContent(jsonContent));

                    // Check that response was successful or throw exception
                    response.EnsureSuccessStatusCode();

                    if (response.IsSuccessStatusCode)
                    {
                    }
                }
                catch (HttpRequestException e)
                {
                    // Handle exception.
                }
            }
            

        }




        public void Dispose()
        {
            try
            {
                cts.Cancel();
            }
            finally
            {
                cts.Dispose();
            }

            tasks.Clear();
        }



        // Provides a task scheduler that ensures a maximum concurrency level while  
        // running on top of the thread pool. 

        class LimitedConcurrencyLevelTaskScheduler : TaskScheduler
        {
            // Indicates whether the current thread is processing work items.
            [ThreadStatic]
            private static bool _currentThreadIsProcessingItems;

            // The list of tasks to be executed  
            private readonly LinkedList<Task> _tasks = new LinkedList<Task>(); // protected by lock(_tasks) 

            // The maximum concurrency level allowed by this scheduler.  
            private readonly int _maxDegreeOfParallelism;

            // Indicates whether the scheduler is currently processing work items.  
            private int _delegatesQueuedOrRunning = 0;

            // Creates a new instance with the specified degree of parallelism.  
            public LimitedConcurrencyLevelTaskScheduler(int maxDegreeOfParallelism)
            {
                if (maxDegreeOfParallelism < 1) throw new ArgumentOutOfRangeException("maxDegreeOfParallelism");
                _maxDegreeOfParallelism = maxDegreeOfParallelism;
            }

            // Queues a task to the scheduler.  
            protected sealed override void QueueTask(Task task)
            {
                // Add the task to the list of tasks to be processed.  If there aren't enough  
                // delegates currently queued or running to process tasks, schedule another.  
                lock (_tasks)
                {
                    _tasks.AddLast(task);
                    if (_delegatesQueuedOrRunning < _maxDegreeOfParallelism)
                    {
                        ++_delegatesQueuedOrRunning;
                        NotifyThreadPoolOfPendingWork();
                    }
                }
            }

            // Inform the ThreadPool that there's work to be executed for this scheduler.  
            private void NotifyThreadPoolOfPendingWork()
            {
                ThreadPool.UnsafeQueueUserWorkItem(_ =>
                {
                    // Note that the current thread is now processing work items. 
                    // This is necessary to enable inlining of tasks into this thread.
                    _currentThreadIsProcessingItems = true;
                    try
                    {
                        // Process all available items in the queue. 
                        while (true)
                        {
                            Task item;
                            lock (_tasks)
                            {
                                // When there are no more items to be processed, 
                                // note that we're done processing, and get out. 
                                if (_tasks.Count == 0)
                                {
                                    --_delegatesQueuedOrRunning;
                                    break;
                                }

                                // Get the next item from the queue
                                item = _tasks.First.Value;
                                _tasks.RemoveFirst();
                            }

                            // Execute the task we pulled out of the queue 
                            base.TryExecuteTask(item);
                        }
                    }
                    // We're done processing items on the current thread 
                    finally { _currentThreadIsProcessingItems = false; }
                }, null);
            }

            // Attempts to execute the specified task on the current thread.  
            protected sealed override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
            {
                // If this thread isn't already processing a task, we don't support inlining 
                if (!_currentThreadIsProcessingItems) return false;

                // If the task was previously queued, remove it from the queue 
                if (taskWasPreviouslyQueued)
                    // Try to run the task.  
                    if (TryDequeue(task))
                        return base.TryExecuteTask(task);
                    else
                        return false;
                else
                    return base.TryExecuteTask(task);
            }

            // Attempt to remove a previously scheduled task from the scheduler.  
            protected sealed override bool TryDequeue(Task task)
            {
                lock (_tasks) return _tasks.Remove(task);
            }

            // Gets the maximum concurrency level supported by this scheduler.  
            public sealed override int MaximumConcurrencyLevel { get { return _maxDegreeOfParallelism; } }

            // Gets an enumerable of the tasks currently scheduled on this scheduler.  
            protected sealed override IEnumerable<Task> GetScheduledTasks()
            {
                bool lockTaken = false;
                try
                {
                    Monitor.TryEnter(_tasks, ref lockTaken);
                    if (lockTaken) return _tasks;
                    else throw new NotSupportedException();
                }
                finally
                {
                    if (lockTaken) Monitor.Exit(_tasks);
                }
            }


        }


    }



}
