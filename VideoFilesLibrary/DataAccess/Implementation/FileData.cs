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
    public class FileData : IFileData {

        public bool SaveFile(FileViewModel valFile) {
            bool vResult = false;
           
            SQLToolsLibrary vSqlTools = new SQLToolsLibrary();
            try {
                List<SqlParameter> vParameterList = new List<SqlParameter>();
                vParameterList.Add(new SqlParameter("@STRID", valFile.Id.ToString()));
                vParameterList.Add(new SqlParameter("@STRNAME", valFile.Name));
                vParameterList.Add(new SqlParameter("@STRVISUALNAME", valFile.VisualName));
                vParameterList.Add(new SqlParameter("@STRTHUMBNAIL", valFile.Thumbnail));
                vParameterList.Add(new SqlParameter("@STRABSOLUTEURL", valFile.Url));
                vParameterList.Add(new SqlParameter("@STRICON", valFile.Icon));
                vParameterList.Add(new SqlParameter("@STREXTENSION", valFile.Extension));
                vParameterList.Add(new SqlParameter("@INTSIZE", valFile.Size));
                vParameterList.Add(new SqlParameter("@STRIDFOLDER", valFile.IdFolder.ToString()));
                vParameterList.Add(new SqlParameter("@INTSTATE", valFile.State));
                vParameterList.Add(new SqlParameter("@STRUSERCREATE", valFile.UserCreate));
                bool vInsert = vSqlTools.ExecuteIUWithStoreProcedure(vParameterList, "sp_Insert_File_User");
                vResult = vInsert;
            } catch (Exception vEx) {
                string vMessage = vEx.Message;
               
            }
            return vResult;
        }

        public List<FileViewModel> DataTableToList(DataTable table) {

            List<FileViewModel> vResult = new List<FileViewModel>();
            try {
                foreach (DataRow row in table.Rows) {
                    FileViewModel vFolder = new FileViewModel {
                        Id = Guid.Parse(Convert.ToString(row["STRID"])),
                        Name = Convert.ToString(row["STRNAME"]),
                        VisualName = Convert.ToString(row["STRVISUALNAME"]),
                        Thumbnail = Convert.ToString(row["STRTHUMBNAIL"]),
                        Url = Convert.ToString(row["STRABSOLUTEURL"]),
                        Icon = Convert.ToString(row["STRICON"]),
                        Extension = Convert.ToString(row["STREXTENSION"]),
                        Size = Convert.ToInt64(row["INTSIZE"]),
                        IdFolder = Guid.Parse(Convert.ToString(row["STRIDFOLDER"])),
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
                vResult = new List<FileViewModel>();
            }
            return vResult;
        }

        public List<FileViewModel> GetByUser(Guid valIdUser) {
            List<FileViewModel> vResult = null;
            try {
                SQLToolsLibrary vSqlTools = new SQLToolsLibrary();
                List<SqlParameter> vParameterList = new List<SqlParameter>();
                vParameterList.Add(new SqlParameter("@STRIDUSER", valIdUser.ToString()));
                DataTable vDatainTable = vSqlTools.ExcecuteSelectWithStoreProcedure(vParameterList, "sp_Select_File_User");
                List<FileViewModel> vData = DataTableToList(vDatainTable);
                if (vData != null && vData.Count > 0) {
                    vResult = vData;
                }
            } catch (Exception) {

                throw;
            }
            return vResult;
        }

        public void UpdateUrlFile(Guid valGuidName, string valUrl) {
            bool vResult = false;

            SQLToolsLibrary vSqlTools = new SQLToolsLibrary();
            try {
                List<SqlParameter> vParameterList = new List<SqlParameter>();
                vParameterList.Add(new SqlParameter("@STRID", valGuidName.ToString()));
                vParameterList.Add(new SqlParameter("@STRURL", valUrl));
                bool vUpdate = vSqlTools.ExecuteIUWithStoreProcedure(vParameterList, "sp_Update_File_Url");
            } catch (Exception) {
                throw;
            }
            
        }
    }
}