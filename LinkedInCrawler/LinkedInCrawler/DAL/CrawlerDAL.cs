using LinkedInCrawler.infra;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using LinkedInCrawler.Models;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace LinkedInCrawler.DAL
{
    public class CrawlerDAL : BaseDAL
    {
        public CrawlerDAL(): base(ConfigurationManager.ConnectionStrings["linkedInCrawler"].ConnectionString){ }
        
        internal void saveUser(User user)
        {
            var skillsStr = string.Empty;
            if (user.Skills.Count > 0)
            {
                StringBuilder builder = new StringBuilder();
                user.Skills.ForEach(s => builder.Append(s + ","));
                builder.Length = builder.Length - 1;
                skillsStr = builder.ToString();
            }
            ExecuteNonQuery(StoreProcedure.AddUser, CommandType.StoredProcedure, new SqlParameter[]
                                                                    {
                                                                        new SqlParameter() { SqlValue = user.Name, SqlDbType = SqlDbType.NVarChar, ParameterName= "@Name"},
                                                                        new SqlParameter() { SqlValue = user.Title, SqlDbType = SqlDbType.NVarChar, ParameterName= "@Title"},
                                                                        new SqlParameter() { SqlValue = user.CurrentPosition, SqlDbType = SqlDbType.NVarChar, ParameterName= "@CurrentPosition"},
                                                                        new SqlParameter() { SqlValue = skillsStr, SqlDbType = SqlDbType.NVarChar, ParameterName= "@Skills"},
                                                                        new SqlParameter() { SqlValue = user.TopSkillsCounter, SqlDbType = SqlDbType.NVarChar, ParameterName= "@TopSkillsCounter"},
                                                                        new SqlParameter() { SqlValue = user.Summary, SqlDbType = SqlDbType.Text, ParameterName= "@Summary"}
                                                                    });
        }

        internal DataTable GetUser(string name)
        {
            DataTable dt = ExecuteDataTable(StoreProcedure.GetUser, CommandType.StoredProcedure, new SqlParameter[] {
                new SqlParameter() { SqlValue = name, SqlDbType = SqlDbType.NVarChar, ParameterName= "@Name"}
            });
            return dt;
        }

        internal DataTable GetSkills(int userID)
        {
            DataTable dt = ExecuteDataTable(StoreProcedure.GetSkills,CommandType.StoredProcedure, new SqlParameter[] {
                new SqlParameter() { SqlValue = userID, SqlDbType = SqlDbType.Int, ParameterName= "@UserID"}
            });
            return dt;
        }
    }

    internal class StoreProcedure
    {
        internal static string AddUser = "Add_User";
        internal static string GetUser = "Get_User";
        internal static string GetSkills = "Get_Skills";
    }
}