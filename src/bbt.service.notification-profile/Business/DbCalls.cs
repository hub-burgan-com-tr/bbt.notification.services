
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using bbt.service.notification_profile.Model;
namespace bbt.service.notification_profile.Business
{
    public class DbCalls
    {

        public static DataTable ExecuteDataTable(string connString, string spName, List<DbDataEntity> paramList)
        {
            DataTable dt = new DataTable();
            SqlConnection conn = new SqlConnection(connString);
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
            }
            catch (Exception ex)
            {
            }
            finally
            {
                if (ConnectionState.Open == conn.State)
                    conn.Close();
            }
            return dt;
        }

    }
}