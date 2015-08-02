using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Net.Http.Headers;


namespace QLMService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "QLMSyncService" in both code and config file together.
    public class QLMSyncService : IQLMSyncService
    {
        System.Timers.Timer timer = new System.Timers.Timer();
        HttpClient clientWorker;
        NetworkScheduler scheduler;
      

        public void OnStart()
        {
            clientWorker = new HttpClient();
            scheduler = new NetworkScheduler();

            timer.Interval = 20000; // 10 seconds
            timer.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimer);
            timer.Start();
            


        }

        static string _address = "http://api.worldbank.org/countries?format=json";

        public void OnTimer(object sender, System.Timers.ElapsedEventArgs args)
        {
            // TODO: Insert monitoring activities here.
           Console.WriteLine("Monitoring the System");


           List<string> result =  scheduler.RunClient(_address);
           Console.WriteLine("result count = " + result.Count);
           

        }


        public void OnStop()
        {
            timer.Stop();
            timer.Close();
            clientWorker.Dispose();
            scheduler.Dispose();

        }



        public List<string> getVersions(long lastSync, string project)
        {


            return null;
        }






        public void addVersions(List<string> lverInfo)
        {
            throw new NotImplementedException();
        }

        public void updateVersion(long versionId, string newVersionInfo)
        {
            throw new NotImplementedException();
        }

        public void delVersion(long versionId)
        {
            throw new NotImplementedException();
        }

        public List<string> getItems(long lastSync, string submitter, string owner)
        {
            throw new NotImplementedException();
        }

        public List<string> getItemsByProject(long lastSync, long versionId, string project)
        {
            throw new NotImplementedException();
        }

        public void addItems(List<string> litems)
        {
            throw new NotImplementedException();
        }

        public void updateItem(long itemId, string newItemInfo)
        {
            throw new NotImplementedException();
        }

        public void changeItemOwner(long itemId, string newOwner)
        {
            throw new NotImplementedException();
        }

        public void closeItems(List<string> litems, int reason)
        {
            throw new NotImplementedException();
        }

        public List<string> getPatches(long lastSync, long versionId)
        {
            throw new NotImplementedException();
        }

        public void addPatches(List<string> lpatchesInfo)
        {
            throw new NotImplementedException();
        }

        public void changePatchState(long patchId, int newState)
        {
            throw new NotImplementedException();
        }




      




    }
}
