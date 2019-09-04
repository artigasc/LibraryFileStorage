using System.Threading.Tasks;
using VideoFilesLibrary.Models;

namespace VideoFilesLibrary.DataAccess.Interfaces {
    public interface IAccountData {
        UserViewModel Verify(string vUser, string vPassword);
    }
}
