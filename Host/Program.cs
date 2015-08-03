using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace Host
{
    class Program
    {
        

        static QLMService.QLMSyncService client = new QLMService.QLMSyncService();
        static System.Timers.Timer timer = new System.Timers.Timer();


        static void Main(string[] args)
        {
           
            timer.AutoReset = false;
            timer.Interval = 20000; // 20 seconds
            timer.Elapsed += new System.Timers.ElapsedEventHandler(OnTimer);
            timer.Start();

           

            using (ServiceHost host = new ServiceHost(typeof(QLMService.QLMSyncService)))
            {
                host.Open();
                Console.WriteLine("Host started @ " + DateTime.Now.ToString());
                Console.ReadLine();
                host.Close();

            }

            timer.Stop();
            timer.Close();
            client.Dispose();

        }


        static  void OnTimer(object sender, System.Timers.ElapsedEventArgs args)
        {
            // TODO: Insert monitoring activities here.
            Console.WriteLine("Monitoring the System");

            List<string> result = client.getItems(9, null, null);
            
            Console.WriteLine("result count = " + result.Count);

            //timer.Enabled = true;


        }



    }
}
