using System;
using System.Collections.Generic;
using VideoFilesLibrary.Models;

namespace VideoFilesLibrary.DataAccess.Interfaces {
    public interface IFileData {
        bool SaveFile(FileViewModel valFolder);
        List<FileViewModel> GetByUser(Guid valIdUser);
        void UpdateUrlFile(Guid vGuidName, string vResultUrl);
    }
}
