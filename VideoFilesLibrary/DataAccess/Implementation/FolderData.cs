using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using VideoFilesLibrary.DataAccess.Interfaces;
using VideoFilesLibrary.Helpers;
using VideoFilesLibrary.Models;

namespace VideoFilesLibrary.DataAccess.Implementation {
    public class FolderData : IFolderData {

        public bool SaveFolder(FolderViewModel  valFolder) {
            bool vResult = false;
           
            SQLToolsLibrary vSqlTools = new SQLToolsLibrary();
            try {
                List<SqlParameter> vParameterList = new List<SqlParameter>();
                vParameterList.Add(new SqlParameter("@STRID", valFolder.Id));
                vParameterList.Add(new SqlParameter("@STRNAME", valFolder.Name));
                vParameterList.Add(new SqlParameter("@STRVISUALNAME", valFolder.VisualName));
                vParameterList.Add(new SqlParameter("@STRTHUMBNAIL", valFolder.Thumnail));
                vParameterList.Add(new SqlParameter("@STRABSOLUTEURL", valFolder.Url));
                vParameterList.Add(new SqlParameter("@STRICON", Constants.vIconFolder));
                vParameterList.Add(new SqlParameter("@INTSTATE", valFolder.State));
                vParameterList.Add(new SqlParameter("@STRUSERCREATE", valFolder.UserCreate));
                bool vInsert = vSqlTools.ExecuteIUWithStoreProcedure(vParameterList, "sp_Insert_Folder_User");
                vResult = vInsert;
            } catch (Exception vEx) {
                string vMessage = vEx.Message;
               
            }
            return vResult;
        }

        private List<FolderViewModel> DataTableToList(DataTable table) {

            List<FolderViewModel> vResult = new List<FolderViewModel>();
            try {
                foreach (DataRow row in table.Rows) {
                    FolderViewModel vFolder = new FolderViewModel {
                        Id = Guid.Parse(Convert.ToString(row["STRID"])),
                        Name = Convert.ToString(row["STRNAME"]),
                        VisualName = Convert.ToString(row["STRVISUALNAME"]),
                        Thumnail = Convert.ToString(row["STRTHUMBNAIL"]),
                        Url = Convert.ToString(row["STRABSOLUTEURL"]),
                        Icon = Convert.ToString(row["STRICON"]),
                        IdParentFolder = !string.IsNullOrEmpty(Convert.ToString(row["STRIDPARENTFOLDER"])) ? Guid.Parse(Convert.ToString(row["STRIDPARENTFOLDER"])) : Guid.Empty,
                        IsShared = Convert.ToInt16(row["INTISSHARED"]),
                        UserId = Guid.Parse(Convert.ToString(row["STRIDUSER"])),
                        State = Convert.ToInt16(row["INTSTATE"]),
                        UserCreate = Convert.ToString(row["STRUSERCREATE"]),
                        DateCreate = Convert.ToDateTime(row["DTTDATECREATE"]),
                        UserUpdate = Convert.ToString(row["STRUSERUPDATE"]),
                        DateUpdate = Convert.ToString(row["DTTDATEUPDATE"]) != string.Empty ? Convert.ToDateTime(row["DTTUSERUPDATE"]) : DateTime.MinValue
                    };
                    vResult.Add(vFolder);
                }
            } catch (Exception vEx) {
                string vMessage = vEx.Message;
                vResult = new List<FolderViewModel>();
            }
            return vResult;
        }

        
        public List<FolderViewModel> GetByUser(Guid valIdUser) {
            List<FolderViewModel> vResult = null;
            try {
                SQLToolsLibrary vSqlTools = new SQLToolsLibrary();
                List<SqlParameter> vParameterList = new List<SqlParameter>();
                vParameterList.Add(new SqlParameter("@STRIDUSER", valIdUser.ToString()));
                DataTable vDatainTable = vSqlTools.ExcecuteSelectWithStoreProcedure(vParameterList, "sp_Select_Folder_User");
                List<FolderViewModel> vData = DataTableToList(vDatainTable);
                if (vData != null && vData.Count > 0) {
                    vResult = vData;
                }
            } catch (Exception) {

                throw;
            }
            return vResult; throw new NotImplementedException();
        }
    }
}