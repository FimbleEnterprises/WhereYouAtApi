using Microsoft.Data.SqlClient;
using MySqlConnector;
using NuGet.Protocol;
using System.Data;
using WhereYouAtCoreApi.Models;
using WhereYouAtCoreApi.Models.Results;

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
            string? newdTripCode = null;
            SqlConnection myConn = new(connectionString);

            // Create unique trip code then ensure that it doesn't already exist in the trip table.
            while (!codeIsUnique) {
                newdTripCode = generateTripcode(5);
                codeIsUnique = !TripCodeAlreadyExists(newdTripCode);
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
                dr["tripcode"] = newdTripCode!;
                dr["createdon"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                dr["createdby"] = createdby;
                ds.Tables[0].Rows.Add(dr);
                da.Update(ds);
                return new ApiBaseResult(true, "CreateTrip", newdTripCode);
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
            string? newdTripCode = null;
            MySqlConnection myConn = new(connectionString);

            // Create unique trip code then ensure that it doesn't already exist in the trip table.
            while (!codeIsUnique) {
                newdTripCode = generateTripcode(5);
                codeIsUnique = !TripCodeAlreadyExists(newdTripCode);
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
                dr["tripcode"] = newdTripCode!;
                dr["createdon"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                dr["createdby"] = createdby;
                ds.Tables[0].Rows.Add(dr);
                da.Update(ds);
                return new ApiBaseResult(true, "CreateTrip", newdTripCode);
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
                    ds.Tables[0].Rows.Add(dr);
                } else {
                    ds.Tables[0].Rows[0]["tripcode"] = update.Tripcode;
                    ds.Tables[0].Rows[0]["createdon"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    ds.Tables[0].Rows[0]["lat"] = update.Lat;
                    ds.Tables[0].Rows[0]["lon"] = update.Lon;
                    ds.Tables[0].Rows[0]["elevation"] = update.Elevation;
                    ds.Tables[0].Rows[0]["member_name"] = update.MemberName;
                }
                da.Update(ds);
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
                    ds.Tables[0].Rows.Add(dr);
                } else {
                    ds.Tables[0].Rows[0]["tripcode"] = update.Tripcode;
                    ds.Tables[0].Rows[0]["createdon"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    ds.Tables[0].Rows[0]["lat"] = update.Lat;
                    ds.Tables[0].Rows[0]["lon"] = update.Lon;
                    ds.Tables[0].Rows[0]["elevation"] = update.Elevation;
                    ds.Tables[0].Rows[0]["member_name"] = update.MemberName;
                }
                da.Update(ds);
                return new ApiBaseResult(true, "UpdateTrip", null);
            } catch (Exception e) {
                myConn.Close();
                return new ApiBaseResult(false, "UpdateTrip", "Failed: " + e.Message);
            } finally {
                myConn.Close();
            }
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
