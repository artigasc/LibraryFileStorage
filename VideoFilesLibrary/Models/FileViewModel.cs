using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VideoFilesLibrary.Models {
    public class FileViewModel {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string VisualName { get; set; }
        public string Thumbnail { get; set; }
        public string Url { get; set; }
        public string Icon { get; set; }
        public string Extension { get; set; }
        public long Size { get; set; }
        public long SizeMB {
            get {
                return Size / 1024 / 1024;
            }

        }
        public Guid IdFolder { get; set; }
        public Guid IdUser { get; set; }
        public bool IsFolder {
            get;
            set;
        }
        public int State { get; set; }
        public string UserCreate { get; set; }
        public DateTime DateCreate { get; set; }
        public string UserUpdate { get; set; }
        public DateTime DateUpdate { get; set; }
        public int IsShared { get; internal set; }
        public Guid UserId { get; internal set; }
    }

    public class FileDownloadViewModel {
        public string Url { get; set; }
    }
}

