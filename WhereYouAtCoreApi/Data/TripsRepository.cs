﻿using Microsoft.Data.SqlClient;
using System.Data;
using WhereYouAt.Api;
using WhereYouAtCoreApi.Models;

namespace WhereYouAtCoreApi.Data {
    public class TripsRepository : MainRepository {
        
        public TripsRepository(IConfiguration config) : base(config) {
            this.config = config;
        }

        /// <summary>
        /// Checks if a valid trip code exists in the TripTable.  Valid trips have a validUntil value that is greater than the current system date/time.
        /// </summary>
        /// <param name="tripcode"></param>
        /// <returns></returns>
        public bool TripCodeAlreadyExists(string tripcode) {
            SqlConnection myConn = new SqlConnection(connectionString);
            try {
                myConn.Open();
                SqlCommand cmd = new SqlCommand("SELECT [tripcode] FROM [trips] WHERE [tripcode] = @code", myConn);
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
        /// Generates a random string of characters of the given length.  Uses the the GetRandomFileName() method from System.IO which leverages the crypto library to ensure better random results.  
        /// </summary>
        /// <param name="length">The character count of the resultant string</param>
        /// <returns></returns>
        private string generateTripcode(int length) {
            string path = Path.GetRandomFileName();
            path = path.Replace(".", ""); // Remove period.
            return path.Substring(0, length);  // Return 8 character string
        }

        /// <summary>
        /// Generates a new entry in the trip table.  Entry is guaranteed to have a unique trip code.
        /// </summary>
        /// <param name="createdby"></param>
        /// <returns></returns>
        public OperationResult CreateTrip(double createdby, bool testmode = false) {
            bool isunique = false;
            string potentialTripcode = "";

            if (testmode) {
                potentialTripcode = "0000";
            } else {
                // Create unique trip code then ensure that it doesn't already exist in the trip table.
                while (!isunique) {
                    potentialTripcode = generateTripcode(4);
                    isunique = !TripCodeAlreadyExists(potentialTripcode);
                }
            }

            SqlConnection myConn = new SqlConnection(connectionString);

            try {
                myConn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM [trips] WHERE 1=2", myConn);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                SqlCommandBuilder cb = new SqlCommandBuilder(da);
                DataSet ds = new DataSet();
                da.Fill(ds);
                DataRow dr = ds.Tables[0].NewRow();
                dr["tripcode"] = potentialTripcode;
                dr["createdon"] = DateTime.UtcNow;
                dr["createdby"] = createdby;
                ds.Tables[0].Rows.Add(dr);
                da.Update(ds);

                return new OperationResult(true, "CreateTrip", potentialTripcode);

            } catch (Exception e) {
                myConn.Close();
                return new OperationResult(false, "CreateTrip", e.Message);
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
		public OperationResult UpdateTrip(string tripcode, double userid, string? locationJson = null) {
            SqlConnection myConn = new (connectionString);
            try {
                myConn.Open();
                SqlCommand cmd = new("SELECT * FROM [tripentries] where [memberid] = @memberid", myConn);
                cmd.Parameters.AddWithValue("@memberid", userid);
                SqlDataAdapter da = new(cmd);
                SqlCommandBuilder cb = new(da);
                DataSet ds = new();
                da.Fill(ds);
                if (ds.Tables[0].Rows.Count == 0) {
                    DataRow dr = ds.Tables[0].NewRow();
                    dr["tripcode"] = tripcode;
                    dr["memberid"] = userid;
                    dr["createdon"] = DateTime.UtcNow;
                    dr["json"] = locationJson;
                    ds.Tables[0].Rows.Add(dr);
                } else {
                    ds.Tables[0].Rows[0]["tripcode"] = tripcode;
                    ds.Tables[0].Rows[0]["createdon"] = DateTime.UtcNow;
                    ds.Tables[0].Rows[0]["json"] = locationJson;
                }
                da.Update(ds);
                return new OperationResult(true, "UpdateTrip", null);
            } catch (Exception e) {
                myConn.Close();
                return new OperationResult(false, "UpdateTrip", "Failed: " + e.Message);
            } finally {
                myConn.Close();
            }
        }

        /// <summary>
        /// Get all members of a trip's locations.
        /// </summary>
        /// <param name="tripcode">The tripcode to find updates for.</param>
        /// <returns>Returns an OperationResult with a Result payload of a List<LocUpdate></returns>
        public OperationResult GetAllMemberLocations(string tripcode) {
            OperationResult result = new();
            SqlConnection myConn = new(connectionString);
            List<LocUpdate> updates = new();

            try {
                myConn.Open();
                SqlCommand cmd = new("SELECT * FROM [tripentries] where [tripcode] = @tripcode", myConn);
                cmd.Parameters.AddWithValue("@tripcode", tripcode);
                SqlDataAdapter da = new(cmd);
                DataSet ds = new();
                da.Fill(ds);
                foreach (DataRow row in ds.Tables[0].Rows) {
                    LocUpdate loc = new(tripcode, Double.Parse(row["memberid"].ToString()!));
                    loc.Createdon = Convert.ToDateTime(row["createdon"]);
                    if (row["lat"] != DBNull.Value) { loc.Lat = Convert.ToDecimal(row["lat"]); }
                    if (row["lon"] != DBNull.Value) { loc.Lon = Convert.ToDecimal(row["lon"]); }
                    if (row["elevation"] != DBNull.Value) { loc.Elevation = Convert.ToInt16(row["elevation"]); }
                    if (row["json"] != DBNull.Value) { loc.Json = row["lon"].ToString(); }
                    updates.Add(loc);
                   
                }
                result.WasSuccessful = true;
                result.OperationSummary = "GetAllMemberLocations";
                result.Result = updates;
            } catch (Exception e) {
                result.WasSuccessful = false;
                result.OperationSummary = "GetAllMemberLocations";
                result.Result = e.Message;
            } finally {
                myConn.Close();
            }
            return result;
        }
    }
}
