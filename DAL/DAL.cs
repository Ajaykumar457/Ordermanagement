using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace DAL
{
    public class DAL
    {
        string Sql_Con = ConfigurationSettings.AppSettings["Connectionstring"].ToString();

        private static PropertyInfo[] GetProperties(object obj)
        {
            return obj.GetType().GetProperties();
        } 
        public DataSet Db_Bll<Tinput>(Tinput obj, string ProcedureName)
        {
            var properties = GetProperties(obj);
            SqlParameter[] sql = new SqlParameter[properties.Count()];
            for (int i = 0; i < properties.Count(); i++)
            {
                string name = properties[i].Name;
                var value = properties[i].GetValue(obj, null);
                //value 
                sql[i] = new SqlParameter("@" + name, value);
            }
            return DbConncet(sql, ProcedureName);
        }

        public DataSet Db_Bll(string commandtype, string ProcedureortextName)
        {
            return DbConncet(commandtype, ProcedureortextName);
        }
         
        public DataSet Db_Bll<Tinput>(Tinput obj, string ProcedureName, int count)
        {
            var properties = GetProperties(obj);
            SqlParameter[] sql = new SqlParameter[count];
            for (int i = 0; i < count; i++)
            {
                string name = properties[i].Name;
                var value = properties[i].GetValue(obj, null);
                sql[i] = new SqlParameter("" + name, value);
            }
            return  DbConncet(sql, ProcedureName);
        } 

        public DataSet DbConncet(SqlParameter[] args, string ProcedureName)
        {
            return SqlHelper.ExecuteDataset(Sql_Con, CommandType.StoredProcedure, ProcedureName, args);
        }

        public DataSet DbConncet(string commandtype, string ProcedureortextName)
        {
            if (commandtype == "text")
                return SqlHelper.ExecuteDataset(Sql_Con, CommandType.Text, ProcedureortextName);
            else
                return SqlHelper.ExecuteDataset(Sql_Con, CommandType.StoredProcedure, ProcedureortextName);
        }


    }
}
