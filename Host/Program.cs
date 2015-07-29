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
        static void Main(string[] args)
        {
            using (ServiceHost host = new ServiceHost(typeof(QLMService.QLMSyncService)))
            {
                host.Open();
                Console.WriteLine("Host started @ " + DateTime.Now.ToString());


                QLMService.QLMSyncService client = new QLMService.QLMSyncService();

                client.OnStart();


                Console.ReadLine();
                client.OnStop();
                host.Close();

            }
        }
    }
}
