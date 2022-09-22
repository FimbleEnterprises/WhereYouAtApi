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

        /// <summary>
        /// Gets the desired update frequency for the clients to use when requesting/uploading locations.
        /// </summary>
        /// <param name="tripcode"></param>
        /// <returns></returns>
        [HttpGet("updaterate")]
        public String UpdateRate() {
            return tripsRepository.GetUpdateRate().ToJson();
        }

        [HttpGet("test")]
        public String Test() {
            ApiBaseResult writeTestResult = mainRepository.TestConnectivityAddRow();
            ApiBaseResult deleteTestResult = mainRepository.TestConnectivityDelRow();
            ApiBaseResult finalResult = new("TestConnectivity");
            if (writeTestResult.WasSuccessful && deleteTestResult.WasSuccessful) {
                finalResult.WasSuccessful = true;
                finalResult.GenericValue = "Successfull read/write!";
            } else {
                finalResult.WasSuccessful = false;
                finalResult.GenericValue = "Failed to read/write";
            }

            return finalResult.ToJson();
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
                    return result;

                case ApiRequest.LEAVE_TRIP:
                    result = tripsRepository.LeaveTrip(
                        (long)(request.Arguments[0].value));
                    return result;

                // One argument
                // locupdate (LocUpdate) (as created by some other language e.g. Kotlin)
                // Returns ApiBaseResult with its GenericValue property being null and the WasSuccessful property reflecting success/failure.
                case ApiRequest.UPDATE_TRIP:
                    try {
                        // Convert the passed LocUpdate object to JSON
                        string serializedLocUpdate = SerializeObject(request.Arguments[0].value);
                        // Convert the JSON into OUR version of a LocUpdate
                        LocUpdate locUpdate = DeserializeObject<LocUpdate>(serializedLocUpdate)!;
                        result = tripsRepository.UpdateTrip(locUpdate);
                        // Remove users that haven't sent an update in awhile (2 minutes I believe)
                        /*if (locUpdate != null && locUpdate.Tripcode != null) {
                            ApiBaseResult aResult = tripsRepository.AreZombiesPresent(locUpdate.Tripcode);
                            if ((bool) aResult.GenericValue == true) {
                                tripsRepository.RemoveZombieMembers(locUpdate.Tripcode);
                            }
                        }*/
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
                string token = "";
                string avatarurl = "https://lh3.googleusercontent.com/a-/AFdZucq-oHBSUBNXGXowWVOFfoeKlEqwAz6w04s4T3ymyEw=s96-c";
                string displayname = "Matt Weber";
                string email = "weber.mathew@gmail.com";
                int isbg = 1;
                string misc1 = "somedata";
                LocUpdate loc = new(
                    result.GenericValue!.ToString()!, 
                    (long)Convert.ToDouble(memberid + i), 
                    Convert.ToDecimal(lat), 
                    Convert.ToDecimal(lon),
                    0,
                    displayname,
                    token,
                    googleid,
                    avatarurl,
                    "weber.fucker@fuck.fuk",
                    0,
                    null
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
