using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Configuration;
using WhereYouAtCoreApi.Data;
using WhereYouAtCoreApi.Models.Requests;
using WhereYouAtCoreApi.Models.Results;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WhereYouAtCoreApi.Controllers
{
    [Route("api/[controller]")]
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

        // GET: api/<ValuesController>
        /*        [HttpGet]
                public IEnumerable<string> Get() {
                    return new string[] { "value1", "value2" };
                }*/

        // GET api/<ValuesController>/5
        /*        [HttpGet("{tripcode}")]
                public OperationResult Get(string tripcode) => tripsRepository.GetAllMemberLocations(tripcode);*/

        [HttpGet]
        public String Get([FromQuery(Name = "tripcode")] string tripcode) {
            return tripsRepository.GetAllMemberLocations(tripcode).ToJson();
        }

        // POST api/<ValuesController>
        [HttpPost]
        public ApiBaseResult Post([FromBody] ApiRequest request) {

            OperationResults results = new();
            ApiBaseResult result = new();


            switch (request.Function) {

                // One argument:
                //      member id   (long)
                case ApiRequest.CREATE_NEW_TRIP:
                    result = tripsRepository.CreateTrip(
                        (long)(request.Arguments[0].value));
                    result.WasSuccessful = true;
                    return result;
                // Three arguments: 
                //      tripcode:   (string)
                //      memberid:   (long)
                //      json:       (string)
                case ApiRequest.UPDATE_TRIP:
                    result = tripsRepository.UpdateTrip(
                        "1p4e",
                        1660256336756,
                        "some json value"
                    );
                    result = new() {
                        WasSuccessful = true
                    };
                    return result;
                case ApiRequest.TEST_FUNCTION:
                    string tripcode = MakePsuedoEntries();
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

        private string MakePsuedoEntries() {
            double memberid = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            ApiBaseResult result = tripsRepository.CreateTrip(memberid);

            for (int i = 0; i < 15; i++) {
                tripsRepository.UpdateTrip(result!.GenericValue!.ToString()!, memberid + i, "{ \"lat\": 43.88842, \"lon\": 16.555452 } ");
            }
            return result!.GenericValue!.ToString()!;
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
