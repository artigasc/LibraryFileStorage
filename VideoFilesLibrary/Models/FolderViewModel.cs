using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VideoFilesLibrary.Helpers;
namespace VideoFilesLibrary.Models {
    public class FolderViewModel {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string VisualName { get; set; }
        public string Thumnail { get; set; }
        public string Url { get; set; }
        public string Icon {  get; set; }        
        public Guid IdParentFolder { get; set; }
        public long Size { get; set; }
        public int IsShared { get; set; }
        public Guid UserId { get; set; }
        public int State { get; set; }
        public bool IsFolder { get {
                return true;
            }
        }
        public string UserCreate { get; set; }
        public DateTime DateCreate { get; set; }
        public string UserUpdate { get; set; }
        public DateTime DateUpdate { get; set; }
    }
}
