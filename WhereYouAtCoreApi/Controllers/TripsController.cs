using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Configuration;
using WhereYouAt.Api;
using WhereYouAtCoreApi.Data;
using WhereYouAtCoreApi.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WhereYouAtCoreApi.Controllers {
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
        [HttpGet("{tripcode}")]
        public OperationResult Get(string tripcode) => tripsRepository.GetAllMemberLocations(tripcode);

        // POST api/<ValuesController>
        [HttpPost]
        public OperationResult Post([FromBody] ApiRequest request) {

            OperationResults results = new();
            OperationResult result = new();
            request.Function = ApiRequest.TEST_FUNCTION;
            long memberid = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            request.Arguments.Add(new("memberid", memberid));

            switch (request.Function) {
                case ApiRequest.CREATE_NEW_TRIP:
                    result = tripsRepository.CreateTrip(
                        DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), 
                        false
                    );
                    result.WasSuccessful = true;
                    return result;
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
                    result.Result = "Created fake trip: " + tripcode;
                    return result;
                default:
                    result = new();
                    result.WasSuccessful = true;
                    return result;
            }

        }

        private string MakePsuedoEntries() {
            double memberid = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            OperationResult result = tripsRepository.CreateTrip(memberid);

            for (int i = 0; i < 15; i++) {
                tripsRepository.UpdateTrip(result!.Result!.ToString()!, memberid + i, "{ \"lat\": 43.88842, \"lon\": 16.555452 } ");
            }
            return result!.Result!.ToString()!;
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
