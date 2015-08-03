using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace QLMService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IQLMSyncService" in both code and config file together.
    [ServiceContract]
    public interface IQLMSyncService
    {
        
       
        void Dispose();

        [OperationContract]
        List<string> getVersions(long lastSync,string project);
        [OperationContract]
        void addVersions(List<string> lverInfo);
        [OperationContract]
        void updateVersion(long versionId,string newVersionInfo);
        [OperationContract]
        void delVersion(long versionId);

        [OperationContract]
        List<string> getItems(long lastSync, string submitter, string owner);
        [OperationContract]
        List<string> getItemsByProject(long lastSync, long versionId, string project);
        [OperationContract]
        void addItems(List<string> litems);
        [OperationContract]
        void updateItem(long itemId, string newItemInfo);
        [OperationContract]
        void changeItemOwner(long itemId, string newOwner);
        [OperationContract]
        void closeItems(List<string> litems,int reason);


        [OperationContract]
        List<string> getPatches(long lastSync,long versionId);
        [OperationContract]
        void addPatches(List<string> lpatchesInfo);
        [OperationContract]
        void changePatchState(long patchId,int newState);
       




    }
}
