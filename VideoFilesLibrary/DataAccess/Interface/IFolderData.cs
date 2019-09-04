using System;
using System.Collections.Generic;
using VideoFilesLibrary.Models;

namespace VideoFilesLibrary.DataAccess.Interfaces {
    public interface IFolderData {
        bool SaveFolder(FolderViewModel valFolder);
        List<FolderViewModel> GetByUser(Guid valIdUser);
    }
}
