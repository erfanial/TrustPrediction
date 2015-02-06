using System;
using System.Data;

//Add MySql Library
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Collections;

namespace TrustPredictionRealData
{
	public class MySqlConnector
	{
		private MySqlConnection connection;
		private string server;
		private string database;
		private string uid;
		private string password;

		//Constructor
		public MySqlConnector()
		{
			Initialize();
		}

		//Initialize values
		private void Initialize()
		{
            string connectionString;
            

            connectionString = "******";

			connection = new MySqlConnection(connectionString);
		}

		//open connection to database
		private bool OpenConnection()
		{
			try
			{
				connection.Open();
				return true;
			}
			catch (MySqlException ex)
			{
				throw(ex);
			}
		}

		//Close connection
		private bool CloseConnection()
		{
			try
			{
				connection.Close();
				return true;
			}
			catch (MySqlException ex)
			{
				throw(ex);
			}
		}

		//NonQuery statement
		public void ExecuteNonSelect(string query)
		{
			//open connection
			if (this.OpenConnection() == true)
			{
				//create command and assign the query and connection from the constructor
				MySqlCommand cmd = new MySqlCommand(query, connection);

				//Execute command
				cmd.ExecuteNonQuery();

				//close connection
				this.CloseConnection();
			}
		}

		//Select statement
        public Dictionary<string, List<string>> ExecuteSelect(string query)
		{
			try
			{
				this.OpenConnection();
				//create command and assign the query and connection from the constructor
				MySqlCommand cmd = new MySqlCommand(query, connection);
				MySqlDataReader dataReader = cmd.ExecuteReader();

                Dictionary<string, List<string>> res = new Dictionary<string, List<string>>();
				for (int i = 0; i < dataReader.FieldCount; i++)
                    res[dataReader.GetName(i)] = new List<string>();

				while (dataReader.Read())
					for (int i = 0; i < dataReader.FieldCount; i++)
						res[dataReader.GetName(i)].Add(dataReader[dataReader.GetName(i)].ToString());

				//close connection
				this.CloseConnection();
				return res;
			}
			catch
			{
				//close connection
				this.CloseConnection();
				throw (new Exception("Could not run query on database"));
			}
		}
	}
}

