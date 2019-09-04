using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using VideoFilesLibrary.Helpers;
using System.Threading.Tasks;
using VideoFilesLibrary.DataAccess.Interfaces;
using VideoFilesLibrary.DataAccess.Implementation;
using VideoFilesLibrary.Models;

namespace VideoFilesLibrary.Helpers {
    class UtilFileManager {
        public string FileName { get; set; }
        public string TempFolder { get; set; }
        public int MaxFileSizeMB { get; set; }
        public List<String> FileParts { get; set; }

        public UtilFileManager() {
            FileParts = new List<string>();
        }

        public async Task<bool> MergeFile(string valFileName, Guid valIdUser, long valSizeFile) {
            bool vResult = false;
            // parse out the different tokens from the filename according to the convention
            string vPartToken = ".part_";
            string vBaseFileName = valFileName.Substring(0, valFileName.IndexOf(vPartToken));
            string vTrailingTokens = valFileName.Substring(valFileName.IndexOf(vPartToken) + vPartToken.Length);
            int vFileIndex = 0;
            int FileCount = 0;
            int.TryParse(vTrailingTokens.Substring(0, vTrailingTokens.IndexOf(".")), out vFileIndex);
            int.TryParse(vTrailingTokens.Substring(vTrailingTokens.IndexOf(".") + 1), out FileCount);
            string Searchpattern = Path.GetFileName(vBaseFileName) + vPartToken + "*";
            string[] vFilesList = Directory.GetFiles(Path.GetDirectoryName(valFileName), Searchpattern);
            if (vFilesList.Count() == FileCount) {
                Guid vGuidName = Guid.NewGuid();
                // use a singleton to stop overlapping processes
                if (!MergeFileManager.Instance.InUse(vBaseFileName)) {
                    MergeFileManager.Instance.AddFile(vBaseFileName);
                    if (File.Exists(vBaseFileName))
                        File.Delete(vBaseFileName);
                    List<SortedFile> vMergeList = new List<SortedFile>();
                    vMergeList = GetListFilesMerged(vFilesList, ref vBaseFileName, vPartToken, ref vTrailingTokens, ref vFileIndex);
                    
                    IFileData vFileData = new FileData();
                    try {
                        string[] vFileInfoName = vBaseFileName.Split("\\");
                        FileViewModel vNewFile = GetFileModelDataBase(vGuidName, vFileInfoName, vBaseFileName, valSizeFile, valIdUser);
                        vFileData.SaveFile(vNewFile);
                    } catch (Exception) {
                        throw;
                    }
                 
                    var vMergeOrder = vMergeList.OrderBy(s => s.FileOrder).ToList();
                    CreateFileInPath(vBaseFileName, vMergeOrder);
                    MergeFileManager.Instance.RemoveFile(vBaseFileName);
                    //vResult = await UploadAzureHelper.UploadFilesToTableStorage(baseFileName);
                    string vResultUrl = await UploadAzureHelper.UploadFilesToBlobStorageContainer(vBaseFileName, vGuidName);
                    if(!string.IsNullOrEmpty(vResultUrl))
                        vFileData.UpdateUrlFile(vGuidName, vResultUrl);
                    string[] FilesListToDelete = Directory.GetFiles(Path.GetDirectoryName(valFileName), Searchpattern);
                    foreach (string File in vFilesList) {
                        System.IO.File.Delete(File);
                    }
                    System.IO.File.Delete(vBaseFileName);
                }
            }
            vResult = true;
            return vResult;
        }

        private FileViewModel GetFileModelDataBase(Guid valGuidName, string[] valFileInfoName, string valBaseFileName, long valSizeFile, Guid valIdUser) {
            FileViewModel vReturn= new FileViewModel() {
                Id = valGuidName,
                Name = valGuidName.ToString() + valFileInfoName.LastOrDefault(),
                VisualName = valFileInfoName.LastOrDefault(),
                Thumbnail = string.Empty,
                Url = valBaseFileName,
                Icon = Constants.vIconFile,
                Extension = valFileInfoName.LastOrDefault().Split(".").LastOrDefault(),
                Size = valSizeFile,
                State = 1,
                UserCreate = valIdUser.ToString()
            };
            return vReturn;
        }

        private List<SortedFile> GetListFilesMerged(string[] vFilesList, ref string vBaseFileName, string vPartToken,
                                                    ref string vTrailingTokens, ref int vFileIndex) {
            List<SortedFile> vResult = new List<SortedFile>();
            foreach (string File in vFilesList) {
                SortedFile sFile = new SortedFile();
                sFile.FileName = File;
                vBaseFileName = File.Substring(0, File.IndexOf(vPartToken));
                vTrailingTokens = File.Substring(File.IndexOf(vPartToken) + vPartToken.Length);
                int.TryParse(vTrailingTokens.Substring(0, vTrailingTokens.IndexOf(".")), out vFileIndex);
                sFile.FileOrder = vFileIndex;
                vResult.Add(sFile);
            }
            return vResult;
        }

        private void CreateFileInPath(string valBaseFileName, List<SortedFile> valMergeOrder) {
            using (FileStream FS = new FileStream(valBaseFileName, FileMode.Create)) {
                // merge each file chunk back into one contiguous file stream
                foreach (var chunk in valMergeOrder) {
                    try {
                        using (FileStream vFileChunk = new FileStream(chunk.FileName, FileMode.Open)) {
                            vFileChunk.CopyTo(FS);

                        }
                    } catch (IOException ex) {
                        throw;
                    }
                }
            }
        }
       
    }

    

    public struct SortedFile {
        public int FileOrder { get; set; }
        public String FileName { get; set; }
    }

    public class MergeFileManager {
        private static MergeFileManager instance;
        private List<string> MergeFileList;

        private MergeFileManager() {
            try {
                MergeFileList = new List<string>();
            } catch { }
        }

        public static MergeFileManager Instance {
            get {
                if (instance == null)
                    instance = new MergeFileManager();
                return instance;
            }
        }

        public void AddFile(string BaseFileName) {
            MergeFileList.Add(BaseFileName);
        }

        public bool InUse(string BaseFileName) {
            return MergeFileList.Contains(BaseFileName);
        }

        public bool RemoveFile(string BaseFileName) {
            return MergeFileList.Remove(BaseFileName);
        }
    }

}



