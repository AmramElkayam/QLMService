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
        
       
        NetworkScheduler scheduler;


        public QLMSyncService()
        {
            scheduler = new NetworkScheduler();
        }



        public void Dispose()
        {
        
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

        static string _address = "http://api.worldbank.org/countries?format=json";

        public List<string> getItems(long lastSync, string submitter, string owner)
        {

            return scheduler.RunClient(_address);


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
