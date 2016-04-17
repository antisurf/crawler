using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace LinkedInCrawler.infra
{
    public class BaseDAL
    {
        private string connectionString;

        protected BaseDAL(string connectionString)
        {
            this.connectionString = connectionString;
        }


        public DataTable ExecuteDataTable(string query, CommandType commandType, params SqlParameter[] sqlParams)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    using (SqlCommand command = new SqlCommand(query, con))
                    {
                        command.CommandTimeout = 200000;
                        command.CommandType = commandType;
                        command.Parameters.AddRange(sqlParams);
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        adapter.Fill(dt);
                    }
                }
                catch (Exception)
                {
                    //TODO handle exceptions
                    throw;
                }
            }
            return dt;
        }

        public void ExecuteNonQuery(string query, CommandType commandType, SqlParameter[] parameters)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    using (SqlCommand command = new SqlCommand(query, con))
                    {
                        command.CommandType = commandType;
                        command.CommandTimeout = 200000;
                        command.Parameters.AddRange(parameters);
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception)
                {
                    //TODO handle exceptions
                    throw;
                }
            }
        }

        public object ExecuteScalar(string query, CommandType commandType, params SqlParameter[] sqlParams)
        {
            object toRet = null;
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    using (SqlCommand command = new SqlCommand(query, con))
                    {
                        command.CommandType = commandType;
                        command.CommandTimeout = 200000;
                        toRet = command.ExecuteScalar();
                    }
                }
            }
            catch (Exception)
            {
                //TODO: handle exceptions
                throw;
            }
            return toRet;
        }
    }
    
}