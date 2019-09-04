using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VideoFilesLibrary.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.File;
using System.IO;
using System.Threading;
using VideoFilesLibrary.Helpers;
using Microsoft.AspNetCore.Http;
using VideoFilesLibrary.DataAccess.Interfaces;
using VideoFilesLibrary.DataAccess.Implementation;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Win32.SafeHandles;

namespace VideoFilesLibrary.Controllers {
    public class HomeController : Controller {
        
        public IActionResult Login() {
            
            return View();
        }

        public IActionResult Index() {
            List<FileViewModel> vListFile = new List<FileViewModel>();
            try {
                UserViewModel vUserSession = HttpContext.Session.Get<UserViewModel>("UserSesion");
                if (vUserSession == null) {
                    return RedirectToAction("Login");
                }
                IFolderData vFolderData = new FolderData();
                IFileData vFileData = new FileData();
                List<FolderViewModel> vListFolder=  vFolderData.GetByUser(vUserSession.Id);
                vListFile= vFileData.GetByUser(vUserSession.Id);
                AddFolderToList(ref vListFile, vListFolder);
                vListFile=vListFile.OrderByDescending(j => j.IsFolder).ThenByDescending(i => i.DateCreate).ToList();
            } catch (Exception) {
                RedirectToAction("Login");
            }
            return View(vListFile);
        }

        private void AddFolderToList(ref List<FileViewModel> valListFile, List<FolderViewModel> valListFolder) {
            if (valListFile == null) {
                valListFile = new List<FileViewModel>();
            }
            if (valListFolder != null && valListFolder.Count() > 0) {
                foreach (FolderViewModel vItem in valListFolder) {
                    FileViewModel vNewFile = new FileViewModel() {
                        Id = vItem.Id,
                        Name = vItem.Name,
                        VisualName = vItem.VisualName,
                        Thumbnail = vItem.Thumnail,
                        Url = vItem.Url,
                        Icon = vItem.Icon,
                        Extension = string.Empty,
                        Size = vItem.Size,
                        IdFolder = vItem.IdParentFolder,
                        IdUser = vItem.UserId,
                        IsShared = vItem.IsShared,
                        IsFolder = true,
                        UserCreate = vItem.UserCreate,
                        DateCreate = vItem.DateCreate,
                        UserUpdate = vItem.UserUpdate,
                        DateUpdate = vItem.DateUpdate
                    };
                    valListFile.Add(vNewFile);
                }
            
            }
        }

        [HttpPost]
        public async Task<ActionResult> UploadFile(IFormFile valFile) {
            // foreach (string file in Request.Form.Files) {
            bool vResult = false;
            var FileDataContent = valFile;
            if (FileDataContent != null && FileDataContent.Length > 0) {
                // take the input stream, and save it to a temp folder using the original file.part name posted
                var stream = FileDataContent.OpenReadStream();
                var fileName = Path.GetFileName(FileDataContent.FileName);

                string vSpecificPAth = @"\App_Data\uploads";
                string appPath = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location).Replace("\\bin\\Debug\\netcoreapp2.1", "");
                var UploadPath = appPath + vSpecificPAth;
                Directory.CreateDirectory(UploadPath);
                string path = Path.Combine(UploadPath, fileName);
                try {
                    UserViewModel vUserSession = HttpContext.Session.Get<UserViewModel>("UserSesion");
                    if (System.IO.File.Exists(path))
                        System.IO.File.Delete(path);
                    using (var fileStream = System.IO.File.Create(path)) {
                        stream.CopyTo(fileStream);
                    }
                    // Once the file part is saved, see if we have enough to merge it
                    UtilFileManager UT = new UtilFileManager();
                    vResult = await UT.MergeFile(path, vUserSession.Id, FileDataContent.Length);
                } catch (Exception) {
                    // handle
                }
            }
            //}
            return Json(new { content = "true", message = vResult.ToString() });
        }

        [HttpPost]
        public async Task<ActionResult> CreateFolderinContainer(string valNameFolder) {

            bool vResult = false;

            if (!string.IsNullOrEmpty(valNameFolder)) {
                try {
                    UserViewModel vUserSession = HttpContext.Session.Get<UserViewModel>("UserSesion");
                    IFolderData vFolderData = new FolderData();
                    string vFullUrl= await UploadAzureHelper.CreateFolderContainerBlobStorage(valNameFolder);
                    string[] vInfoUrl = vFullUrl.Split("/");
                    string vUrl = string.Join("/", vInfoUrl.Take(vInfoUrl.Count() - 1));
                    FolderViewModel vFolder = new FolderViewModel();
                    vFolder.Id = Guid.NewGuid();
                    vFolder.Name = vInfoUrl.ElementAt(vInfoUrl.Count()-2);
                    vFolder.VisualName = valNameFolder;
                    vFolder.Thumnail = string.Empty;
                    vFolder.Url = vUrl;
                    vFolder.Icon = string.Empty;
                    vFolder.State = 1;
                    vFolder.UserCreate = vUserSession.Id.ToString();
                   
                    vResult =vFolderData.SaveFolder(vFolder);
                } catch (Exception vEx) {
                    vResult = false;
                    return Json(new { content = vResult.ToString(), message = vEx.Message });
                }
            }
            
            return Json(new { content = vResult.ToString(), message = valNameFolder });
        }

        
        public async Task<ActionResult> DownloadFile(string valFile) {
           
            CloudBlockBlob vBlobClient =  UploadAzureHelper.GetBlobStorageContainer(valFile);
           
            //using (Stream vMemoryStream = new FileStream(vHandle, FileAccess.ReadWrite)) {
                AccessCondition vAccesCondition = new AccessCondition();
               
                BlobRequestOptions vBlobRequestOptions = new BlobRequestOptions();
                vBlobRequestOptions.MaximumExecutionTime = TimeSpan.FromHours(24);
                vBlobRequestOptions.ServerTimeout = TimeSpan.FromHours(24);
                OperationContext vOperationContext = new OperationContext();
            var ms = new MemoryStream();
            await vBlobClient.DownloadToStreamAsync(ms, vAccesCondition, vBlobRequestOptions, vOperationContext);
            ActionResult va = File(ms, vBlobClient.Properties.ContentType);
            return va;
            //}            
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
