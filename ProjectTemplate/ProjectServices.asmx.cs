using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data;

namespace ProjectTemplate
{
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[System.ComponentModel.ToolboxItem(false)]
	[System.Web.Script.Services.ScriptService]

	public class ProjectServices : System.Web.Services.WebService
	{
		////////////////////////////////////////////////////////////////////////
		///replace the values of these variables with your database credentials
		////////////////////////////////////////////////////////////////////////
		private string dbID = "cis440springA2025team4";
		private string dbPass = "cis440springA2025team4";
		private string dbName = "cis440springA2025team4";
		////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////
		///call this method anywhere that you need the connection string!
		////////////////////////////////////////////////////////////////////////
		private string getConString() {
			return "SERVER=107.180.1.16; PORT=3306; DATABASE=" + dbName+"; UID=" + dbID + "; PASSWORD=" + dbPass;
		}
		////////////////////////////////////////////////////////////////////////



		/////////////////////////////////////////////////////////////////////////
		//don't forget to include this decoration above each method that you want
		//to be exposed as a web service!
		[WebMethod(EnableSession = true)]
		/////////////////////////////////////////////////////////////////////////
		public string TestConnection()
		{
			try
			{
				string testQuery = "select * from users";

				////////////////////////////////////////////////////////////////////////
				///here's an example of using the getConString method!
				////////////////////////////////////////////////////////////////////////
				MySqlConnection con = new MySqlConnection(getConString());
				////////////////////////////////////////////////////////////////////////

				MySqlCommand cmd = new MySqlCommand(testQuery, con);
				MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
				DataTable table = new DataTable();
				adapter.Fill(table);
				return "Success!";
			}
			catch (Exception e)
			{
				return "Something went wrong, please check your credentials and db name and try again.  Error: "+e.Message;
			}
		}

        [WebMethod(EnableSession = true)] // NOTICE: gotta enable session on each individual method
        public bool LogOn(string uid, string pass)
        {
            // We return this flag to tell them if they logged in or not
            bool success = false;

			// Our connection string comes from our web.config file like we talked about earlier
			string sqlConnectString = getConString();

            // Here's our query. A basic select with nothing fancy.
            // NOTICE: We added admin to what we pull, so that we can store it along with the id in the session
            string sqlSelect = "SELECT  id FROM users WHERE userid=@idValue AND pass=@passValue";

            // Set up our connection object to be ready to use our connection string
            MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);

            // Set up our command object to use our connection, and our query
            MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

            // Tell our command to replace the @parameters with real values
            // We decode them because they came to us via the web, so they were encoded for transmission (funky characters escaped, mostly)
            sqlCommand.Parameters.AddWithValue("@idValue", HttpUtility.UrlDecode(uid));
            sqlCommand.Parameters.AddWithValue("@passValue", HttpUtility.UrlDecode(pass));

            // A data adapter acts like a bridge between our command object and
            // the data we are trying to get back and put in a table object
            MySqlDataAdapter sqlDa = new MySqlDataAdapter(sqlCommand);

            // Here's the table we want to fill with the results from our query
            DataTable sqlDt = new DataTable();

            // Here we go filling it!
            sqlDa.Fill(sqlDt);

            // Check to see if any rows were returned. If they were, it means it's a legit account
            if (sqlDt.Rows.Count > 0)
            {
                // If we found an account, store the id and admin status in the session
                // so we can check those values later on other method calls to see if they
                // are 1) logged in at all, and 2) an admin or not
                Session["id"] = sqlDt.Rows[0]["id"];
                //Session["admin"] = sqlDt.Rows[0]["admin"];
                success = true;
            }

            // Return the result!
            return success;
        }
        [WebMethod(EnableSession = true)]
        public bool SignUp(string uid, string pass)
        {
            bool success = false;
            string sqlConnectString = getConString();

            try
            {
                using (MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString))
                {
                    sqlConnection.Open();

                    // Check if the username already exists
                    string checkUserQuery = "SELECT COUNT(*) FROM users WHERE userid = @idValue";
                    using (MySqlCommand checkUserCmd = new MySqlCommand(checkUserQuery, sqlConnection))
                    {
                        checkUserCmd.Parameters.AddWithValue("@idValue", HttpUtility.UrlDecode(uid));
                        int userExists = Convert.ToInt32(checkUserCmd.ExecuteScalar());

                        if (userExists > 0)
                        {
                            return false; // Username already taken
                        }
                    }

                    // If username does not exist, insert new user
                    string sqlInsert = "INSERT INTO users (userid, pass) VALUES (@idValue, @passValue)";
                    using (MySqlCommand sqlCommand = new MySqlCommand(sqlInsert, sqlConnection))
                    {
                        sqlCommand.Parameters.AddWithValue("@idValue", HttpUtility.UrlDecode(uid));
                        sqlCommand.Parameters.AddWithValue("@passValue", HttpUtility.UrlDecode(pass));

                        int rowsAffected = sqlCommand.ExecuteNonQuery();
                        success = rowsAffected > 0;
                    }
                }
            }
            catch (Exception e)
            {
                // Log the exception for debugging purposes
                Console.WriteLine("Error: " + e.Message);
            }

            return success;
        }
    }
}
