using System.Text.Json.Serialization;

namespace WhereYouAtCoreApi.Models.Results
{
    public class ApiBaseResult {
        public string? Operation { get; set; }
        public bool WasSuccessful { get; set; } = false;
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
            this.Operation = operationSummary;
            this.WasSuccessful = wasSuccessful;
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
