using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Timers;

namespace Host
{
    class Program
    {
        static void Main(string[] args)
        {

            System.Timers.Timer timer = new System.Timers.Timer();

            timer.Interval = 30000; // 30 seconds
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

        }

        private static void OnTimer(object sender, ElapsedEventArgs e)
        {

            // TODO: Insert monitoring activities here.
            Console.WriteLine("Monitoring the System");
            QLMService.IQLMSyncService client = new QLMService.QLMSyncService();

            client.OnStart();
            List<string> result = client.getVersions(67,null);
            Console.WriteLine("result count = " + result.Count);
            client.OnStop();

        }
    }


    
}
