using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using VideoFilesLibrary.DataAccess.Interfaces;
using VideoFilesLibrary.Helpers;
using VideoFilesLibrary.Models;

namespace VideoFilesLibrary.DataAccess.Implementation {
    public class AccountData : IAccountData {

        public UserViewModel Verify(string vUser, string vPassword) {
            UserViewModel vResult = null;
            DataTable vDatainTable = new DataTable();
            SQLToolsLibrary vSqlTools = new SQLToolsLibrary();
            try {
                List<SqlParameter> vParameterList = new List<SqlParameter>();
                vParameterList.Add(new SqlParameter("@STRUSERNAME", vUser));
                vParameterList.Add(new SqlParameter("@STRPASSWORD", vPassword));
                vDatainTable = vSqlTools.ExcecuteSelectWithStoreProcedure(vParameterList, "sp_Select_User_Verify");
                List<UserViewModel> vData = DataTableToList(vDatainTable);
                if (vData != null && vData.Count == 1) {
                    vResult = vData.FirstOrDefault();
                }
            } catch (Exception) {
                vResult = null;
            }
            return vResult;
        }

        public List<UserViewModel> DataTableToList(DataTable table) {

            List<UserViewModel> vResult = new List<UserViewModel>();
            try {
                foreach (DataRow row in table.Rows) {
                    UserViewModel vUser = new UserViewModel {
                        Id = Guid.Parse(Convert.ToString(row["STRID"])),
                        UserName = Convert.ToString(row["STRUSERNAME"]),
                        Password = Convert.ToString(row["STRPASSWORD"]),
                        Name = Convert.ToString(row["STRNAME"]),
                        SecondName = Convert.ToString(row["STRSECONDNAME"]),
                        LastName = Convert.ToString(row["STRLASTNAME"]),
                        SecondLastName = Convert.ToString(row["STRSECONDLASTNAME"]),
                        Email = Convert.ToString(row["STREMAIL"]),
                        State = Convert.ToInt16(row["INTSTATE"]),
                        UserCreate = Convert.ToString(row["STRUSERCREATE"]),
                        DateCreate = Convert.ToDateTime(row["DTTDATECREATE"]),
                        UserUpdate = Convert.ToString(row["STRUSERUPDATE"]),
                        DateUpdate = Convert.ToString(row["DTTUSERUPDATE"]) != string.Empty ? Convert.ToDateTime(row["DTTUSERUPDATE"]) : DateTime.MinValue
                    };
                    vResult.Add(vUser);
                }
            } catch (Exception vEx) {
                string vMessage = vEx.Message;
                vResult = new List<UserViewModel>();
            }
            return vResult;
        }


    }
}