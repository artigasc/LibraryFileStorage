using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.File;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;

namespace VideoFilesLibrary.Helpers {
    public static class UploadAzureHelper {
        public  static async Task<bool> UploadFilesToTableStorage(string valFileName) {
            bool vResult = false;
            const string DemoShare = "demofileshare";
            const string DemoDirectory = "demofiledirectory";
            try {


                CloudStorageAccount vStorageAccount = CreateStorageAccountFromConnectionString(Constants.vStorageConnectionString);

                CloudFileClient fileClient = vStorageAccount.CreateCloudFileClient();
                Console.WriteLine("1. Creating file share");
                CloudFileShare share = fileClient.GetShareReference(DemoShare);
                CloudFileDirectory root = share.GetRootDirectoryReference();

                CloudFileDirectory dir = root.GetDirectoryReference(DemoDirectory);
                string[] FileInfoName = valFileName.Split("\\");
                CloudFile file = dir.GetFileReference(FileInfoName.LastOrDefault());

                FileRequestOptions fr = new FileRequestOptions();
                OperationContext oc = new OperationContext();
                AccessCondition ac = new AccessCondition();
                //using (FileStream FileUpload = new FileStream(valFileName, FileMode.Open)) {
                    await file.UploadFromFileAsync(valFileName);
                    //await file.UploadFromStreamAsync(FileUpload, FileUpload.Length);
                //}
                vResult = true;
            } catch (Exception vEx) {
                string message = vEx.Message;
            }
            return vResult;
            // List all files/directories under the root directory

        }

        private static CloudStorageAccount CreateStorageAccountFromConnectionString(string valStorageConnectionString) {
            CloudStorageAccount vStorageAccount;
            try {
                vStorageAccount = CloudStorageAccount.Parse(valStorageConnectionString);
            } catch (FormatException) {
                throw;
            } catch (ArgumentException) {
                throw;
            }

            return vStorageAccount;
        }

        public static async Task<string> UploadFilesToBlobStorageContainer(string valFileName, Guid valIdName) {
            string vResult = string.Empty;
            try {
                CloudStorageAccount vStorageAccount = CreateStorageAccountFromConnectionString(Constants.vStorageConnectionString);
                CloudBlobClient vBlobClient = vStorageAccount.CreateCloudBlobClient();
                CloudBlobContainer vContainer = vBlobClient.GetContainerReference(Constants.vContainerDefault);
                string[] vFileInfoName = valFileName.Split("\\");
                string vName = valIdName.ToString() + vFileInfoName.LastOrDefault();
                CloudBlockBlob blockBlob = vContainer.GetBlockBlobReference(vName);
                AccessCondition vAccesCondition = new AccessCondition();
                BlobRequestOptions vBlobRequestOptions = new BlobRequestOptions();
                vBlobRequestOptions.MaximumExecutionTime = TimeSpan.FromHours(24);
                vBlobRequestOptions.ServerTimeout= TimeSpan.FromHours(24);
                OperationContext vOperationContext = new OperationContext();
              
                await blockBlob.UploadFromFileAsync(valFileName, vAccesCondition,vBlobRequestOptions, vOperationContext);
                vResult = blockBlob.Uri.AbsoluteUri;
            } catch (Exception vEx) {
                string message = vEx.Message;
            }
            return vResult;
        }

        public static async Task<string> CreateFolderContainerBlobStorage(string valNameFolder) {
            string vResult = string.Empty;
            const string vContainerDefault = Constants.vContainerDefault;
            try {
                CloudStorageAccount vStorageAccount = CreateStorageAccountFromConnectionString(Constants.vStorageConnectionString);
                CloudBlobClient vBlobClient = vStorageAccount.CreateCloudBlobClient();
                CloudBlobContainer vContainer = vBlobClient.GetContainerReference(vContainerDefault);
                string vFolder = Guid.NewGuid()+valNameFolder + "/$$$.$$$";
                CloudBlockBlob vBlockBlob = vContainer.GetBlockBlobReference(vFolder);
                await vBlockBlob.UploadTextAsync(Constants.vTextFileFolder);
                vResult= vBlockBlob.Uri.AbsoluteUri;
            } catch (Exception vEx) {
                string message = vEx.Message;
            }
            return vResult;
        }

        public static CloudBlockBlob GetBlobStorageContainer(string valUrl) {
            CloudBlockBlob vResult = null;
            try {
                CloudStorageAccount vStorageAccount = CreateStorageAccountFromConnectionString(Constants.vStorageConnectionString);
                CloudBlobClient vBlobClient = vStorageAccount.CreateCloudBlobClient();
                CloudBlobContainer vContainer = vBlobClient.GetContainerReference(Constants.vContainerDefault);
                string[] vInfoNameFile = valUrl.Split("/");
                CloudBlockBlob vBlockBlob = vContainer.GetBlockBlobReference(vInfoNameFile.LastOrDefault());
 
                return vBlockBlob;
                
            } catch (Exception vEx) {
                string message = vEx.Message;
            }
            return vResult;
        }
    }
}
