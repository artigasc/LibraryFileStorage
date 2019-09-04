using Microsoft.IdentityModel.Protocols;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace VideoFilesLibrary.Helpers {
    public class SQLToolsLibrary {

        private string ConnectionStrings() {
            //string connection = ConfigurationManager.ConnectionStrings[valCS].ToString();
            string vConnection = "Server=tcp:sqlvideolibrary.database.windows.net,1433;Initial Catalog=sqlvideolibrary;Persist Security Info=False;User ID=Administrador;Password=w33system_sql;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            //return ConfigurationManager.ConnectionStrings[valCS].ToString();
            return vConnection;
        }

        public DataTable GetResultSql(List<SqlParameter> valParameterList, string valSql) {
            DataTable vResult = new DataTable();
            SqlConnection vConn = new SqlConnection(ConnectionStrings());
            SqlDataReader vReader;
            vConn.Open();
            SqlCommand vComm = new SqlCommand(valSql, vConn);
            foreach (SqlParameter vData in valParameterList) {
                vComm.Parameters.Add(vData);
            }
            vReader = vComm.ExecuteReader();
            vResult.Load(vReader);
            vReader.Close();
            vConn.Close();
            return vResult;
        }

        public bool ExecuteQuery(string valSql) {
            bool vResult = false;
            SqlConnection vConn = new SqlConnection(ConnectionStrings());
            vConn.Open();
            SqlCommand vComm = new SqlCommand(valSql, vConn);
            vResult = (vComm.ExecuteNonQuery() > 0);
            vConn.Close();
            return vResult;
        }

        public DataTable GetResultSql(string valSql) {
            DataTable vResult = new DataTable();
            SqlConnection vConn = new SqlConnection(ConnectionStrings());
            SqlDataReader vReader;
            vConn.Open();
            SqlCommand vComm = new SqlCommand(valSql, vConn);
            vReader = vComm.ExecuteReader();
            vResult.Load(vReader);
            vReader.Close();
            vConn.Close();
            return vResult;
        }

        public SqlParameter ParameterString(string valName, string valValue, int valSize) {
            SqlParameter vResult = new SqlParameter();
            vResult = GetParameter(valName, SqlDbType.NVarChar, valValue, valSize);
            return vResult;
        }

        public SqlParameter GetParameter(string valName, SqlDbType valSqlDbType, object valValue, int valSize) {
            SqlParameter vResult = new SqlParameter {
                ParameterName = "@" + valName,
                SqlDbType = valSqlDbType,
                Direction = ParameterDirection.Input,
                Value = valValue
            };
            if ((valSqlDbType != SqlDbType.DateTime) || (valSqlDbType != SqlDbType.UniqueIdentifier)) {
                vResult.Size = valSize;
            }
            return vResult;
        }

        public DataTable ExcecuteSelectWithStoreProcedure(List<SqlParameter> valParameterList, string valStoreProcedureName) {
            DataTable vResult = new DataTable();
            SqlConnection vConn = new SqlConnection(ConnectionStrings());
            SqlCommand vComm = new SqlCommand(valStoreProcedureName, vConn) {
                CommandType = CommandType.StoredProcedure
            };

            if (valParameterList != null && valParameterList.Count() > 0) {

                foreach (SqlParameter vData in valParameterList) {
                    vComm.Parameters.Add(vData);
                }
            }
            vConn.Open();
            vResult.Load(vComm.ExecuteReader());

            vConn.Close();
            return vResult;
        }

        public bool ExecuteIUWithStoreProcedure(List<SqlParameter> valParameterList, string valStoreProcedureName) {
            bool vResult = false;
            SqlConnection vConn = new SqlConnection(ConnectionStrings());
            SqlCommand vComm = new SqlCommand(valStoreProcedureName, vConn) {
                CommandType = CommandType.StoredProcedure
            };
            if (valParameterList != null && valParameterList.Count() > 0) {

                foreach (SqlParameter vData in valParameterList) {
                    vComm.Parameters.Add(vData);
                }
            }
            vConn.Open();
            //vResult = (vComm.ExecuteNonQuery() > 0);
            vResult = Convert.ToBoolean(vComm.ExecuteNonQuery());
            vConn.Close();
            return vResult;
        }
    }
}