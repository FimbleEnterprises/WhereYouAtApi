using Microsoft.Data.SqlClient;
using System.Data;
using WhereYouAt.Api;

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
            myConn.Open();
            SqlCommand cmd = new SqlCommand("SELECT [tripcode] FROM [trips] WHERE [tripcode] = @code", myConn);
            cmd.Parameters.AddWithValue("@code", tripcode);
            SqlDataAdapter da = new SqlDataAdapter(cmd);

            DataSet ds = new DataSet();
            da.Fill(ds);
            myConn.Close();
            return ds.Tables[0].Rows.Count > 0;
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
        public OperationResult CreateTrip(double createdby, bool testmode) {
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
                myConn.Close();

                return new OperationResult(true, "Trip was created.  Check operationSummary for the tripcode", potentialTripcode);

            } catch (Exception e) {
                myConn.Close();
                return new OperationResult(false, "Failed to create trip - see operationSummary for any messages", e.Message);
            }
        }
    }
}
