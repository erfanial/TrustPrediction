#define DEBUG
using System;
using System.Linq;
using System.Web;

using System.Data.SqlClient;

using System.Collections;
using System.Collections.Generic;
using System.IO;


namespace DataSimulation
{
    public class SqlServerConnector
    {
        private static SqlConnection con;
        private static string connectionString = "Data Source=E3-201-F6RJSR1;Initial Catalog=DissertationSimDB;Integrated Security=True";
        //private static string connectionString = "Data Source=(LocalDB)\v11.0;AttachDbFilename=\"../DissertationSimDB.mdf\";Integrated Security=True;Connect Timeout=30";

        public SqlServerConnector()
        {
            con = new SqlConnection(connectionString);
        }

        public bool ExecuteNonQuery(string query)
        {
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand(query, con);
                com.ExecuteNonQuery();
                con.Close();
                return true;
            }
            catch
            {
                con.Close();
                return false;
            }
        }

        public Dictionary<string, List<string>> ExecuteQuery(string query)
        {
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand(query, con);
                SqlDataReader reader = com.ExecuteReader();

                Dictionary<string, List<string>> res = new Dictionary<string, List<string>>();
                for (int i = 0; i < reader.FieldCount; i++)
                    res[reader.GetName(i)] = new List<string>();

                while (reader.Read())
                    for (int i = 0; i < reader.FieldCount; i++)
                        res[reader.GetName(i)].Add(reader[reader.GetName(i)].ToString());

                con.Close();
                return res;
            }
            catch
            {
                con.Close();
                return null;
            }
        }
    }
}