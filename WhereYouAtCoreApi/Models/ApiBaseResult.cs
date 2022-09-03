using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace WhereYouAtCoreApi.Models.Results
{
    public class ApiBaseResult {
		[JsonProperty("Operation")]
		public string? Operation { get; set; }
		[JsonProperty("WasSuccessful")]
		public bool WasSuccessful { get; set; } = false;
		[JsonProperty("GenericValue")]
		public object? GenericValue { get; set; }

        public ApiBaseResult() { }

        public ApiBaseResult(string operationSummary) {
            this.Operation = operationSummary;
        }

        public ApiBaseResult(bool wasSuccessful, string operationSummary, object? result) {
            this.Operation = operationSummary;
			this.WasSuccessful = wasSuccessful;
			this.GenericValue = result;
        }

        public ApiBaseResult(bool wasSuccessful, string operationSummary) {
            Operation = operationSummary;
            WasSuccessful = wasSuccessful;
        }

        public string ToJson() {
            try {
                return Newtonsoft.Json.JsonConvert.SerializeObject(this);
            } catch (Exception e) {
                return "Failed to convert results to json: " + e.Message;
            }
        }
    }
}
