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
        [HttpGet]
        public IEnumerable<string> Get() {
            return new string[] { "value1", "value2" };
        }

        // GET api/<ValuesController>/5
        [HttpGet("{id}")]
        public string Get(int id) {
            return "value";
        }

        // POST api/<ValuesController>
        [HttpPost]
        public OperationResults Post([FromBody] ApiRequest request) {



            OperationResults results = new OperationResults();
            request = new ApiRequest();
            request.Function = ApiRequest.CREATE_NEW_TRIP;
            
            // DataRepository repo = new DataRepository(this);
            // string constring = .GetConnectionString();

            switch (request.Function) {
                case ApiRequest.CREATE_NEW_TRIP:
                    OperationResult result = tripsRepository.CreateTrip(
                        DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), 
                        false
                    );
                    result.WasSuccessful = true;
                    results.allResults.Add(result);
                    return results;
                case ApiRequest.UPDATE_TRIP:
                    result = new OperationResult();
                    result.WasSuccessful = true;
                    results.allResults.Add(result);
                    return results;
                default:
                    result = new OperationResult();
                    result.WasSuccessful = true;
                    results.allResults.Add(result);
                    return results;
            }

        }

        // PUT api/<ValuesController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value) {
        }

        // DELETE api/<ValuesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id) {
        }
    }
}
