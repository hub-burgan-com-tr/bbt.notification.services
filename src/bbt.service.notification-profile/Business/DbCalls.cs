
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Notification.Profile.Enum;
using Notification.Profile.Model;
using notification_profile.Model;
namespace Notification.Profile.Business
{
    public class DbCalls
    {

        public static DataTableResponseModel ExecuteDataTable(string connString, string spName, List<DbDataEntity> paramList)
        {
            DataTableResponseModel responseModel = new DataTableResponseModel();
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connString))
            {
            
                try
                {
                    conn.Open();
                    SqlCommand command = new SqlCommand();
                    command.Connection = conn;
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = spName;
                    SqlParameter param;
                    foreach (DbDataEntity parameter in paramList)
                    {
                        param = new SqlParameter(parameter.parameterName, parameter.value);
                        param.Direction = parameter.direction;
                        param.DbType = parameter.dbType;
                        command.Parameters.Add(param);
                    }
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                    dataAdapter.Fill(dt);
                    responseModel.DataTable = dt;
                }
                catch (Exception ex)
                {
                    responseModel.Result = ResultEnum.Error;
                    responseModel.MessageList.Add (ex.Message);

                }
                finally
                {
                    if (ConnectionState.Open == conn.State)
                        conn.Close();
                }
            }
            return responseModel;
        }

    }
}