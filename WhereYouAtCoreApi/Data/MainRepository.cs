using Microsoft.Data.SqlClient;
using MySqlConnector;
using NuGet.Common;
using System.Data;
using WhereYouAt.Api;
using WhereYouAtCoreApi.Models.Results;

namespace WhereYouAtCoreApi.Data {
   public class MainRepository {

        public IConfiguration? config { get; set; }
        public string connectionString { get; set; }

        public MainRepository(IConfiguration config) {
            this.config = config;
            this.connectionString = GetConnectionString();
            // WriteLogLine("Instantiated repository", Severity.LOW);
        }
        
        private string GetConnectionString() {
            // string constring = config.GetConnectionString("DefaultConnection");
            string constring = config.GetConnectionString("MySqlAwsConnection");
            return constring;
        }

        public enum OptionType {
            BOOL, INT, BIGINT, STRING, BLOB, MONEY, FLOAT, DATETIME
        }

        private static string getOptionsValueColumnName(OptionType type) {
            switch (type) {
                case OptionType.BIGINT:
                    return "bigIntValue";
                case OptionType.INT:
                    return "intValue";
                case OptionType.BOOL:
                    return "boolValue";
                case OptionType.STRING:
                    return "strValue";
                case OptionType.BLOB:
                    return "blobValue";
                case OptionType.MONEY:
                    return "moneyValue";
                case OptionType.FLOAT:
                    return "floatValue";
                case OptionType.DATETIME:
                    return "dtValue";
                default:
                    return "strValue";
            }
        }

        /// <summary>
        /// Returns the appropriate column name from the options table based on the data type.
        /// </summary>
        /// <param name="optionName">The OptionName value</param>
        /// <param name="type">The data type of the option you are querying.</param>
        /// <returns>The column name</returns>
        public object GetSingleOptionValue_mssql(string optionName, OptionType type) {
            SqlConnection myConn = new SqlConnection(GetConnectionString());
            myConn.Open();
            SqlCommand cmd = new SqlCommand(
                "SELECT "
                + getOptionsValueColumnName(type) + " " +
                "FROM [options] " +
                "WHERE [name] = @name", myConn
            );
            cmd.Parameters.AddWithValue("@name", optionName);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            myConn.Close();
            return ds.Tables[0].Rows[0][0];
        }

        /// <summary>
        /// Returns the appropriate column name from the options table based on the data type.
        /// </summary>
        /// <param name="optionName">The OptionName value</param>
        /// <param name="type">The data type of the option you are querying.</param>
        /// <returns>The column name</returns>
        public object GetSingleOptionValue(string optionName, OptionType type) {
            MySqlConnection myConn = new MySqlConnection(GetConnectionString());
            myConn.Open();
            MySqlCommand cmd = new MySqlCommand(
                "SELECT "
                + getOptionsValueColumnName(type) + " " +
                "FROM `options` " +
                "WHERE `name` = @name", myConn
            );
            cmd.Parameters.AddWithValue("@name", optionName);
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            myConn.Close();
            return ds.Tables[0].Rows[0][0];
        }

        public enum Severity {
            LOW, MEDIUM, HIGH
        }

        public void WriteLogLine_mssql(string value, Severity severity) {
            SqlConnection myConn = new SqlConnection(GetConnectionString());
            try {

                myConn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM [debuglogging] WHERE 1=2", myConn);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                SqlCommandBuilder cb = new SqlCommandBuilder(da);
                DataSet ds = new DataSet();
                da.Fill(ds);
                DataRow dr = ds.Tables[0].NewRow();
                dr["createdon"] = DateTime.UtcNow;
                dr["severity"] = severity;
                dr["message"] = value;
                ds.Tables[0].Rows.Add(dr);
                da.Update(ds);
                myConn.Close();
            } catch (Exception fuckyou) {
                myConn.Close();
            }
        }

        public void WriteLogLine(string value, Severity severity) {
            MySqlConnector.MySqlConnection myConn = new MySqlConnector.MySqlConnection(GetConnectionString());
            try {

                myConn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM whereyouat.debuglogging WHERE 1=2", myConn);
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                MySqlCommandBuilder cb = new MySqlCommandBuilder(da);
                DataSet ds = new DataSet();
                da.Fill(ds);
                DataRow dr = ds.Tables[0].NewRow();
                dr["createdon"] = DateTime.UtcNow;
                dr["severity"] = severity;
                dr["message"] = value;
                ds.Tables[0].Rows.Add(dr);
                da.Update(ds);
                myConn.Close();
            } catch (Exception fuckyou) {
                myConn.Close();
            } finally {
                CleanUpOldLogs();
            }
        }

        /// <summary>
        /// Removes old entries from the logging table.
        /// </summary>
        public void CleanUpOldLogs() {
            MySqlConnection myConn = new(connectionString);
            try {
                myConn.Open();
                DateTime currentTime = DateTime.UtcNow;
                DateTime cutoffTime = currentTime.AddDays(-3);
                MySqlCommand cmd = new("" +
                    "DELETE FROM `debuglogging` " +
                    "where `createdon` < " + cutoffTime.ToLongDateString()
                    , myConn);
                cmd.ExecuteNonQuery();
            } catch (Exception) {
            } finally {
                myConn.Close();
            }
        }

    }
}
