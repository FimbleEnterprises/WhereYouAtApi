using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Configuration;
using WhereYouAtCoreApi.Data;
using WhereYouAtCoreApi.Models;
using WhereYouAtCoreApi.Models.Requests;
using WhereYouAtCoreApi.Models.Results;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using static Newtonsoft.Json.JsonConvert;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WhereYouAtCoreApi.Controllers
{
    [Route("api/trips")]
    [ApiController]
    public class TripsController : ControllerBase {

        private readonly IConfiguration config;
        private readonly MainRepository mainRepository;
        private readonly TripsRepository tripsRepository;

        public TripsController(IConfiguration config) {
            this.config = config;
            this.mainRepository = new MainRepository(config);
            this.tripsRepository = new TripsRepository(config);

        }

        /// <summary>
        /// Gets all entries for a supplied trip code.
        /// </summary>
        /// <param name="tripcode">All associated trip entry rows 
        /// from the tripentries table for this trip code.
        /// </param>
        /// <returns>MemberLocationsResult object with all associated entries.</returns>
        [HttpGet("entries/{tripcode}")]
        public String Get(string tripcode) {
            return tripsRepository.GetAllMemberLocations(tripcode).ToJson();
        }

        /// <summary>
        /// Checks if the trip exists in the trips table and if so, that it has 
        /// been updated recently enough to be considered active (two hours by default).
        /// </summary>
        /// <param name="tripcode">The tripcode to evaluate.</param>
        /// <returns>ApiBaseResult object with GenericValue == true if
        /// trip is active - this property will be false if the tripcode
        /// is not found in the trips table.</returns>
        [HttpGet("isactive/{tripcode}")]
        public String Trip(string tripcode) {
            return tripsRepository.TripIsActive(tripcode).ToJson();
        }

        // POST api/<ValuesController>
        [HttpPost]
        public ApiBaseResult Post([FromBody] ApiRequest request) {

            // Instantiate the eventual return object.
            OperationResults results = new();
            ApiBaseResult result = new();

            // Evaluate the Function property of the passed ApiRequest object and perform operations accordingly.
            switch (request.Function) {

                // One argument:
                //      member id   (long)
                // Returns ApiBaseResult with its GenericValue property reflecting the created trip's tripcode.
                case ApiRequest.CREATE_NEW_TRIP:
                    result = tripsRepository.CreateTrip(
                        (long)(request.Arguments[0].value));
                    result.WasSuccessful = true;
                    return result;

                // One argument
                // locupdate (LocUpdate) (as created by some other language e.g. Kotlin)
                // Returns ApiBaseResult with its GenericValue property being null and the WasSuccessful property reflecting success/failure.
                case ApiRequest.UPDATE_TRIP:
                    // LocUpdate locUpdate = new("{\"Createdon\":\"2022-08-13T20:30:01.113\",\"Elevation\":45,\"Lat\":12.445554,\"Lon\":5.444333,\"Memberid\":1660440576512,\"Tripcode\":\"SEAVI\"}"!);
                    mainRepository.WriteLogLine("UPDATE_TRIP: " + request.Arguments[0], MainRepository.Severity.LOW);
                    try {
                        // Convert the passed LocUpdate object to JSON
                        string serializedLocUpdate = SerializeObject(request.Arguments[0].value);
                        // Convert the JSON into OUR version of a LocUpdate
                        LocUpdate locUpdate = DeserializeObject<LocUpdate>(serializedLocUpdate)!;
                        result = tripsRepository.UpdateTrip(locUpdate);
                    } catch (Exception e1) {
                        mainRepository.WriteLogLine(e1.Message + "\n" + e1.StackTrace, MainRepository.Severity.HIGH);
                    }
                    return result;

                // one argument
                //      (string) the trip code
                // Returns a MemberLocationsResult with its Value property representing a List<LocUpdate>
                // This can also be accomplished with a GET request instead of POST.
                case ApiRequest.LOCATION_UPDATE_REQUESTED:
                    return tripsRepository.GetAllMemberLocations(request.Arguments[0].ToString()!);

                case ApiRequest.CLEANUP_TRIPS_TABLE:
                    return tripsRepository.CleanUpTripTable();

                case ApiRequest.TEST_FUNCTION:
                    int num = 15;
                    if (request.Arguments.Count > 0) {
                        num = Convert.ToInt16(request.Arguments[0].value);
                    }
                    string tripcode = MakePsuedoEntries(num);
                    result = new();
                    result.WasSuccessful = true;
                    result.Operation = "CreateFakeTripWithFakeEntries";
                    result.GenericValue = tripcode;
                    return result;

                default:
                    result = new();
                    result.WasSuccessful = true;
                    return result;
            }

        }

        /*
         {"Createdon":"2022-08-13T20:30:01.113","Elevation":45,"Lat":12.445554,"Lon":5.444333,"Memberid":1660440576512,"Tripcode":"SEAVI"}
        */

        private string MakePsuedoEntries(int number) {
            double memberid = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            ApiBaseResult result = tripsRepository.CreateTrip(memberid);

            for (int i = 0; i < number; i++) {
                double lat = GetRandomNumberInRange(45.2804722, 45.2504722);
                double lon = GetRandomNumberInRange(-93.716118, -93.719118);
                string googleid = "116830684150145127689";
                string token = "eyJhbGciOiJSUzI1NiIsImtpZCI6ImU4NDdkOTk0OGU4NTQ1OTQ4ZmE4MTU3YjczZTkxNWM1NjczMDJkNGUiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL2FjY291bnRzLmdvb2dsZS5jb20iLCJhenAiOiIzOTUwOTAxNjgwNzAtY2l2cHNvNnZqbWE2a3I5b2p2YmhsbmxsZmN0cjNucXEuYXBwcy5nb29nbGV1c2VyY29udGVudC5jb20iLCJhdWQiOiIzOTUwOTAxNjgwNzAtZG8zMHB0Yzk0b2M4MXR1NTdjY283YThwcDMzbG5hZ2suYXBwcy5nb29nbGV1c2VyY29udGVudC5jb20iLCJzdWIiOiIxMTY4MzA2ODQxNTAxNDUxMjc2ODkiLCJlbWFpbCI6IndlYmVyLm1hdGhld0BnbWFpbC5jb20iLCJlbWFpbF92ZXJpZmllZCI6dHJ1ZSwibmFtZSI6Ik1hdHQgV2ViZXIiLCJwaWN0dXJlIjoiaHR0cHM6Ly9saDMuZ29vZ2xldXNlcmNvbnRlbnQuY29tL2EtL0FGZFp1Y3Etb0hCU1VCTlhHWG93V1ZPRmZvZUtsRXF3QXo2dzA0czRUM3lteUV3PXM5Ni1jIiwiZ2l2ZW5fbmFtZSI6Ik1hdHQiLCJmYW1pbHlfbmFtZSI6IldlYmVyIiwibG9jYWxlIjoiZW4iLCJpYXQiOjE2NjIzODY3NzUsImV4cCI6MTY2MjM5MDM3NX0.lGoktYE84tYqKq5N19Xc_Yz-kNWyXTgHwxPgbW-5pG8L-BQv2sKNGRnpXuDb5PA1443LIao93WZDNgLaEfIXDBcnzmP6N1MrLBsY6RJcZpIThSR-mkWB_Uojn4zTYyKO68tM6ja6lEIAtJWcbPVyi-fVrdiDR9pKLJOd__fuUzwj33_uoKL6m57x56Wors0YJ9E_Ais191kD4XnEbDUM3XSGFAEjvK2Wt3YZoR6Z9ttG6nMxm0EqVCSBa42fIIrzBFF6cVRraFmMODA6Lvr2lMCM42wyEZ4Gr_Y6aFP4gBd3SWfwxvizziCMFPqt8XgHoq7KF2pUe-gUJwqbHAA4vw";
                string avatarurl = "https://lh3.googleusercontent.com/a-/AFdZucq-oHBSUBNXGXowWVOFfoeKlEqwAz6w04s4T3ymyEw=s96-c";
                string displayname = "Matt Weber";
                string email = "weber.mathew@gmail.com";
				LocUpdate loc = new(
                    result.GenericValue!.ToString()!, 
                    (long)Convert.ToDouble(memberid + i), 
                    Convert.ToDecimal(lat), 
                    Convert.ToDecimal(lon),
                    0,
                    displayname,
                    token,
                    googleid,
                    avatarurl
                );
                loc.Elevation = 34;
                loc.MemberName = "Member " + i;
                tripsRepository.UpdateTrip(loc);
            }
            return result!.GenericValue!.ToString()!;
        }

        private double GetRandomNumberInRange(double minNumber, double maxNumber) {
            return new Random().NextDouble() * (maxNumber - minNumber) + minNumber;
        }

        // PUT api/<ValuesController>/5
        /*        [HttpPut("{tripcode}")]
                public void Put(int id, [FromBody] string value) {
                }*/

        // DELETE api/<ValuesController>/5
        /*        [HttpDelete("{id}")]
                public void Delete(int id) {
                }*/
    }
}
