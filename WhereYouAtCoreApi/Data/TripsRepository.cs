using Microsoft.Data.SqlClient;
using MySqlConnector;
using NuGet.Protocol;
using System.Data;
using WhereYouAtCoreApi.Models;
using WhereYouAtCoreApi.Models.Results;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace WhereYouAtCoreApi.Data
{
    public class TripsRepository : MainRepository {
        
        public TripsRepository(IConfiguration config) : base(config) {
            this.config = config;
        }

        /// <summary>
        /// Checks if a valid trip code exists in the TripTable.  Valid trips have a validUntil value that is greater than the current system date/time.
        /// </summary>
        /// <param name="tripcode"></param>
        /// <returns></returns>
        public bool TripCodeAlreadyExists_mssql(string tripcode) {
            SqlConnection myConn = new SqlConnection(connectionString);
            try {
                myConn.Open();
                SqlCommand cmd = new SqlCommand(
                    "SELECT `tripcode` " +
                    "FROM `trips` " +
                    "WHERE `tripcode` = @code", myConn
                );
                cmd.Parameters.AddWithValue("@code", tripcode);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);
                return ds.Tables[0].Rows.Count > 0;
            } catch (Exception e) {
                return true;
            } finally {
                myConn.Close();
            }
        }

        /// <summary>
        /// Checks if a valid trip code exists in the TripTable.  Valid trips have a validUntil value that is greater than the current system date/time.
        /// </summary>
        /// <param name="tripcode"></param>
        /// <returns></returns>
        public bool TripCodeAlreadyExists(string tripcode) {
            MySqlConnection myConn = new(connectionString);
            try {
                myConn.Open();
                MySqlCommand cmd = new(
                    "SELECT `tripcode` " +
                    "FROM `trips` " +
                    "WHERE `tripcode` = @code", myConn
                );
                cmd.Parameters.AddWithValue("@code", tripcode);
                MySqlDataAdapter da = new (cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);
                return ds.Tables[0].Rows.Count > 0;
            } catch (Exception e) {
                return true;
            } finally {
                myConn.Close();
            }
        }

        public ApiBaseResult TripIsActive(string tripcode) {
            MySqlConnection myConn = new(connectionString);
            ApiBaseResult result = new ApiBaseResult("TripIsActive");
            myConn.Open();
            MySqlCommand cmd = new("SELECT * FROM `trips` where `tripcode` = @tripcode", myConn);
            cmd.Parameters.AddWithValue("@tripcode", tripcode);
            MySqlDataAdapter da = new(cmd);
            MySqlCommandBuilder cb = new(da);
            DataSet ds = new();
            da.Fill(ds);
           
            if (ds.Tables[0].Rows.Count == 0) { // not found -> trip not active.
                result.GenericValue = false;
                result.WasSuccessful = true;
            } else { // trip found => check if it's been updated recently enough to be valid.
                DataRow row = ds.Tables[0].Rows[0];
                double lastUpdated = 0d;
                if (row["lastupdated"] != DBNull.Value) { lastUpdated = Convert.ToDouble(row["lastupdated"]); }
                double twoHours = 7200000;
                double currentTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                double cutoffTime = currentTime - twoHours;
                result.WasSuccessful = true;
                if(lastUpdated < cutoffTime) { 
                    // Not valid; too old.
                    result.GenericValue = false;
                } else {
                    // Valid
                    result.GenericValue = true;
                }
            }
            return result;
        }

        /// <summary>
        /// Generates a random string of characters of the given length.  Uses the the GetRandomFileName() method from System.IO which leverages the crypto library to ensure better random results.  
        /// </summary>
        /// <param name="length">The character count of the resultant string</param>
        /// <returns></returns>
        private string generateTripcode(int length) {
            string path = Path.GetRandomFileName().ToUpper();
            path = path.Replace(".", ""); // Remove period.
            return path.Substring(0, length);  // Return 8 character string
        }

        /// <summary>
        /// Generates a new entry in the trip table.  Entry is guaranteed to have a unique trip code.
        /// </summary>
        /// <param name="createdby"></param>
        /// <returns></returns>
        public ApiBaseResult CreateTrip_mssql(double createdby) {
            bool codeIsUnique = false;
            string? newTripCode = null;
            SqlConnection myConn = new(connectionString);

            // Create unique trip code then ensure that it doesn't already exist in the trip table.
            while (!codeIsUnique) {
                newTripCode = generateTripcode(5);
                codeIsUnique = !TripCodeAlreadyExists(newTripCode);
            }

            try {
                myConn.Open();
                SqlCommand cmd = new(
                    "SELECT * " +
                    "FROM [trips] " +
                    "WHERE 1=2", myConn);
                SqlDataAdapter da = new(cmd);
                SqlCommandBuilder cb = new(da);
                DataSet ds = new();
                da.Fill(ds);
                DataRow dr = ds.Tables[0].NewRow();
                dr["tripcode"] = newTripCode!;
                dr["createdon"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                dr["createdby"] = createdby;
                dr["lastupdated"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                ds.Tables[0].Rows.Add(dr);
                da.Update(ds);
                TimestampTripTable(newTripCode!);
                return new ApiBaseResult(true, "CreateTrip", newTripCode);
            } catch (Exception e) {
                myConn.Close();
                return new ApiBaseResult(false, "CreateTrip", e.Message);
            } finally {
                myConn.Close();
            }
        }

        /// <summary>
        /// Generates a new entry in the trip table.  Entry is guaranteed to have a unique trip code.
        /// </summary>
        /// <param name="createdby"></param>
        /// <returns></returns>
        public ApiBaseResult CreateTrip(double createdby) {
            bool codeIsUnique = false;
            string? newTripCode = null;
            MySqlConnection myConn = new(connectionString);

            // Create unique trip code then ensure that it doesn't already exist in the trip table.
            while (!codeIsUnique) {
                newTripCode = generateTripcode(5);
                codeIsUnique = !TripCodeAlreadyExists(newTripCode);
            }

            try {
                myConn.Open();
                MySqlCommand cmd = new(
                    "SELECT * " +
                    "FROM `trips` " +
                    "WHERE 1=2", myConn);
                MySqlDataAdapter da = new(cmd);
                MySqlCommandBuilder cb = new(da);
                DataSet ds = new();
                da.Fill(ds);
                DataRow dr = ds.Tables[0].NewRow();
                dr["tripcode"] = newTripCode!;
                dr["createdon"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                dr["createdby"] = createdby;
                dr["lastupdated"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                ds.Tables[0].Rows.Add(dr);
                da.Update(ds);
                TimestampTripTable(newTripCode!);
                return new ApiBaseResult(true, "CreateTrip", newTripCode);
            } catch (Exception e) {
                myConn.Close();
                return new ApiBaseResult(false, "CreateTrip", e.Message);
            } finally {
                myConn.Close();
            }
        }

        /// <summary>
		/// Adds or updates a record in the tripentries table.
		/// </summary>
		/// <param name="tripcode"></param>
		/// <param name="userid"></param>
		/// <returns>An OperationResult object that will really only contain the userid of the user that prompted the update (or an error if applicable).</returns>
		public ApiBaseResult UpdateTrip_mssql(LocUpdate update) {
            SqlConnection myConn = new (connectionString);
            try {
                myConn.Open();
                SqlCommand cmd = new("SELECT * FROM [tripentries] where [memberid] = @memberid", myConn);
                cmd.Parameters.AddWithValue("@memberid", update.Memberid);
                SqlDataAdapter da = new(cmd);
                SqlCommandBuilder cb = new(da);
                DataSet ds = new();
                da.Fill(ds);
                if (ds.Tables[0].Rows.Count == 0) {
                    DataRow dr = ds.Tables[0].NewRow();
                    dr["tripcode"] = update.Tripcode;
                    dr["memberid"] = update.Memberid;
                    dr["createdon"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    dr["lat"] = update.Lat;
                    dr["lon"] = update.Lon;
                    dr["elevation"] = update.Elevation;
                    dr["member_name"] = update.MemberName;
					dr["speed"] = update.Speed;
					dr["bearing"] = update.Bearing;
					dr["accuracy"] = update.Accuracy;
					dr["name"] = update.DisplayName;
					dr["googleid"] = update.GoogleId;
					dr["token"] = update.Token;
					dr["avatarurl"] = update.AvatarUrl;
					dr["email"] = update.Email;
					ds.Tables[0].Rows.Add(dr);
                } else {
                    ds.Tables[0].Rows[0]["tripcode"] = update.Tripcode;
                    ds.Tables[0].Rows[0]["createdon"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    ds.Tables[0].Rows[0]["lat"] = update.Lat;
                    ds.Tables[0].Rows[0]["lon"] = update.Lon;
                    ds.Tables[0].Rows[0]["elevation"] = update.Elevation;
                    ds.Tables[0].Rows[0]["member_name"] = update.MemberName;
					ds.Tables[0].Rows[0]["speed"] = update.Speed;
					ds.Tables[0].Rows[0]["bearing"] = update.Bearing;
					ds.Tables[0].Rows[0]["accuracy"] = update.Accuracy;
					ds.Tables[0].Rows[0]["name"] = update.DisplayName;
					ds.Tables[0].Rows[0]["googleid"] = update.GoogleId;
					ds.Tables[0].Rows[0]["token"] = update.Token;
					ds.Tables[0].Rows[0]["avatarurl"] = update.AvatarUrl;
					ds.Tables[0].Rows[0]["email"] = update.Email;
				}
                da.Update(ds);
                TimestampTripTable(update.Tripcode!);
                return new ApiBaseResult(true, "UpdateTrip", null);
            } catch (Exception e) {
                myConn.Close();
                return new ApiBaseResult(false, "UpdateTrip", "Failed: " + e.Message);
            } finally {
                myConn.Close();
            }
        }

        /// <summary>
		/// Adds or updates a record in the tripentries table.
		/// </summary>
		/// <param name="tripcode"></param>
		/// <param name="userid"></param>
		/// <returns>An OperationResult object that will really only contain the userid of the user that prompted the update (or an error if applicable).</returns>
		public ApiBaseResult UpdateTrip(LocUpdate update) {
            MySqlConnection myConn = new(connectionString);
            try {
                myConn.Open();
                MySqlCommand cmd = new("SELECT * FROM `tripentries` where `memberid` = @memberid", myConn);
                cmd.Parameters.AddWithValue("@memberid", update.Memberid);
                MySqlDataAdapter da = new(cmd);
                MySqlCommandBuilder cb = new(da);
                DataSet ds = new();
                da.Fill(ds);
                if (ds.Tables[0].Rows.Count == 0) {
                    DataRow dr = ds.Tables[0].NewRow();
                    dr["tripcode"] = update.Tripcode;
                    dr["memberid"] = update.Memberid;
                    dr["createdon"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    dr["lat"] = update.Lat;
                    dr["lon"] = update.Lon;
                    dr["elevation"] = update.Elevation;
                    dr["member_name"] = update.MemberName;
                    if (update.Speed != null) {
						dr["speed"] = update.Speed;
					}
					if(update.Bearing != null)
                    {
						dr["bearing"] = update.Bearing;
					}
					if (update.Accuracy != null)
                    {
						dr["accuracy"] = update.Accuracy;
					}
                    dr["name"] = update.DisplayName;
                    dr["googleid"] = update.GoogleId;
                    dr["token"] = update.Token;
                    dr["avatarurl"] = update.AvatarUrl;
                    dr["email"] = update.Email;
					ds.Tables[0].Rows.Add(dr);
                } else {
                    ds.Tables[0].Rows[0]["tripcode"] = update.Tripcode;
                    ds.Tables[0].Rows[0]["createdon"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    ds.Tables[0].Rows[0]["lat"] = update.Lat;
                    ds.Tables[0].Rows[0]["lon"] = update.Lon;
                    ds.Tables[0].Rows[0]["elevation"] = update.Elevation;
                    ds.Tables[0].Rows[0]["member_name"] = update.MemberName;
                    if (update.Speed != null) {
						ds.Tables[0].Rows[0]["speed"] = update.Speed;
					}
                    if (update.Bearing != null)
                    {
                        ds.Tables[0].Rows[0]["bearing"] = update.Bearing;
                    }
                    if (update.Accuracy != null)
                    {
                        ds.Tables[0].Rows[0]["accuracy"] = update.Accuracy;
                    }
					ds.Tables[0].Rows[0]["name"] = update.DisplayName;
					ds.Tables[0].Rows[0]["googleid"] = update.GoogleId;
					ds.Tables[0].Rows[0]["token"] = update.Token;
					ds.Tables[0].Rows[0]["avatarurl"] = update.AvatarUrl;
					ds.Tables[0].Rows[0]["email"] = update.Email;
				}
                da.Update(ds);
                TimestampTripTable(update.Tripcode!);
                return new ApiBaseResult(true, "UpdateTrip", null);
            } catch (Exception e) {
                myConn.Close();
                return new ApiBaseResult(false, "UpdateTrip", "Failed: " + e.Message);
            } finally {
                myConn.Close();
            }
        }

        private bool TimestampTripTable(string tripcode) {
            MySqlConnection myConn = new(connectionString);
            try {
                myConn.Open();
                MySqlCommand cmd = new("SELECT * FROM `trips` where `tripcode` = @tripcode", myConn);
                cmd.Parameters.AddWithValue("@tripcode", tripcode);
                MySqlDataAdapter da = new(cmd);
                MySqlCommandBuilder cb = new(da);
                DataSet ds = new();
                da.Fill(ds);
                if (ds.Tables[0].Rows.Count > 0) {
                    DataRow dr = ds.Tables[0].Rows[0];
                    dr["lastupdated"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    da.Update(ds);
                    return true;
                }
                return false;
            } catch (Exception e) {
                myConn.Close();
                return false;
            } finally {
                myConn.Close();
            }
        }

        /// <summary>
        /// Checks the database for trips that haven't received in update
        /// in a while and removes them from the trips table.
        /// </summary>
        /// <returns></returns>
        public ApiBaseResult CleanUpTripTable() {
            ApiBaseResult apiBaseResult = new ApiBaseResult("CleanUpTripTable");
            MySqlConnection myConn = new(connectionString);
            try {
                myConn.Open();
                double twoHours = 7200000;
                double currentTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                double cutoffTime = currentTime - twoHours;
                MySqlCommand cmd = new("DELETE FROM `trips` where `lastupdated` < " + cutoffTime, myConn);
                int rowsDeleted = cmd.ExecuteNonQuery();
                apiBaseResult.GenericValue = rowsDeleted;
                apiBaseResult.WasSuccessful = true;
                WriteLogLine("Cleaned up " + rowsDeleted + " rows from the 'trips' table.", Severity.LOW);
            } catch (Exception e) {
                apiBaseResult.WasSuccessful = false;
                apiBaseResult.GenericValue = e.Message;
                WriteLogLine("Failed to cleanup rows from the 'trips' table.  " +
                    "Error: " + e.Message, Severity.LOW);
            } finally {
                myConn.Close();
            }
            return apiBaseResult;
        }

        /*
      ,[tripcode]
      ,[memberid]
      ,[lat]
      ,[lon]
      ,[createdon]
      ,[elevation]
      ,[json]
         */

        /// <summary>
        /// Get all members of a trip's locations.
        /// </summary>
        /// <param name="tripcode">The tripcode to find updates for.</param>
        /// <returns>Returns an OperationResult with a Result payload of a List<LocUpdate></returns>
        public MemberLocationsResult GetAllMemberLocations_mssql(string tripcode) {
            MemberLocationsResult result = new();
            SqlConnection myConn = new(connectionString);
            List<LocUpdate> updates = new();

            try {
                myConn.Open();
                SqlCommand cmd = new(
                    "SELECT * " +
                    "FROM [tripentries] " +
                    "where [tripcode] = @tripcode", myConn
                );
                cmd.Parameters.AddWithValue("@tripcode", tripcode);
                SqlDataAdapter da = new(cmd);
                DataSet ds = new();
                da.Fill(ds);
                int i = 0;
                foreach (DataRow row in ds.Tables[0].Rows) {
                    try {
                        i++;
                        LocUpdate loc = new(tripcode, (long)Convert.ToDouble(row["memberid"]));
                        loc.Createdon = (long)Convert.ToDouble(row["createdon"]);
                        if (row["lat"] != DBNull.Value) { loc.Lat = Convert.ToDecimal(row["lat"]); }
                        if (row["lon"] != DBNull.Value) { loc.Lon = Convert.ToDecimal(row["lon"]); }
                        if (row["elevation"] != DBNull.Value) { loc.Elevation = Convert.ToInt16(row["elevation"]); }
                        if (row["member_name"] != DBNull.Value) { loc.MemberName = row["member_name"].ToString(); }
						if (row["speed"] != DBNull.Value) { loc.Speed = Convert.ToDecimal(row["speed"]); }
						if (row["bearing"] != DBNull.Value) { loc.Bearing = Convert.ToDecimal(row["bearing"]); }
						if (row["accuracy"] != DBNull.Value) { loc.Accuracy = Convert.ToDecimal(row["accuracy"]); }
						updates.Add(loc);
                    } catch (Exception e1) {
                        string error = e1.Message;
                    }                 
                }
                result.WasSuccessful = true;
                result.Operation = "GetAllMemberLocations";
                result.MemberLocations = updates;
                result.GenericValue = updates.Count;
            } catch (Exception e) {
                result.WasSuccessful = false;
                result.Operation = "GetAllMemberLocations";
                result.GenericValue = e.Message;
            } finally {
                myConn.Close();
            }
            return result;
        }

        public MemberLocationsResult GetAllMemberLocations(string tripcode) {

            MemberLocationsResult result = new();
            MySqlConnection command = new(connectionString);
            List<LocUpdate> updates = new();

            try { 
                command.Open();
                MySqlCommand cmd = new(
                    "SELECT * " +
                    "FROM `tripentries` " +
                    "where `tripcode` = @tripcode", command
                );
                cmd.Parameters.AddWithValue("@tripcode", tripcode);

                using (MySqlDataReader reader = cmd.ExecuteReader()) {
                    while (reader.Read()) {
                        // access your record colums by using reader
                        LocUpdate loc = new(tripcode, (long)Convert.ToDouble(reader["memberid"]));
                        loc.Createdon = (long)Convert.ToDouble(reader["createdon"]);
                        if (reader["lat"] != DBNull.Value) { loc.Lat = Convert.ToDecimal(reader["lat"]); }
                        if (reader["lon"] != DBNull.Value) { loc.Lon = Convert.ToDecimal(reader["lon"]); }
                        if (reader["elevation"] != DBNull.Value) { loc.Elevation = Convert.ToInt16(reader["elevation"]); }
                        if (reader["member_name"] != DBNull.Value) { loc.MemberName = reader["member_name"].ToString(); }
						if (reader["speed"] != DBNull.Value) { loc.Speed = Convert.ToDecimal(reader["speed"]); }
						if (reader["bearing"] != DBNull.Value) { loc.Bearing = Convert.ToDecimal(reader["bearing"]); }
						if (reader["accuracy"] != DBNull.Value) { loc.Accuracy = Convert.ToDecimal(reader["accuracy"]); }
						if (reader["name"] != DBNull.Value) { loc.DisplayName = reader["name"].ToString(); }
						if (reader["googleid"] != DBNull.Value) { loc.GoogleId = reader["googleid"].ToString(); }
						if (reader["token"] != DBNull.Value) { loc.Token = reader["token"].ToString(); }
						if (reader["avatarurl"] != DBNull.Value) { loc.AvatarUrl = reader["avatarurl"].ToString(); }
						if (reader["email"] != DBNull.Value) { loc.Email = reader["email"].ToString(); }
						updates.Add(loc);
                    }
                }
                result.WasSuccessful = true;
                result.Operation = "GetAllMemberLocations";
                result.MemberLocations = updates;
                result.GenericValue = updates.Count;
            } catch (Exception ex) {
                result.WasSuccessful = false;
                result.Operation = "GetAllMemberLocations";
                result.GenericValue = ex.Message;
            } finally {
                command.Close();
            }
            return result;
        }
    }
}
