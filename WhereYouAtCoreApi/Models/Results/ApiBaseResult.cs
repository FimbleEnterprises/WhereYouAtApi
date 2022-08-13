namespace WhereYouAtCoreApi.Models.Results
{
    public class ApiBaseResult {
        public string? Operation { get; set; }
        public bool WasSuccessful { get; set; } = false;
        public object? GenericValue { get; set; }

        public ApiBaseResult() { }

        public ApiBaseResult(bool wasSuccessful, string operationSummary, object? result) {
            Operation = operationSummary;
            WasSuccessful = wasSuccessful;
            GenericValue = result;
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
