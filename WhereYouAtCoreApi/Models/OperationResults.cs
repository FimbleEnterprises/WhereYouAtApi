using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WhereYouAt.Api {
	public class OperationResults {
		public List<OperationResult> allResults = new List<OperationResult>();

		public OperationResults() { }

		public OperationResults(OperationResult result) {
			this.allResults.Add(result);
		}

		public string toJson() {
			try {
				return Newtonsoft.Json.JsonConvert.SerializeObject(this);
			} catch (Exception e) {
				return "Failed to convert results to json: " + e.Message;
			}
		}
	}

	public class OperationResult {
		public bool WasSuccessful { get; set; } = false;
		public string? OperationSummary { get; set; }
		public object? Result { get; set; }

		public OperationResult() { }

		public OperationResult(bool wasSuccessful, string operationSummary, object result) {
			this.WasSuccessful = wasSuccessful;
			this.OperationSummary = operationSummary;
			this.Result = result;
		}

		public OperationResult(bool wasSuccessful, string operationSummary) {
			this.WasSuccessful = wasSuccessful;
			this.OperationSummary = operationSummary;
			this.Result = "";
		}
	}

	public class FcmUpsertResult : OperationResult {
		public string Userid { get; set; }
		public string FcmToken { get; set; }

		public FcmUpsertResult(bool wasSuccessful, string userid, string fcmtoken) {
			this.Userid = userid;
			this.FcmToken = fcmtoken;
			this.WasSuccessful = wasSuccessful;
		}

		public string ToJson() {
			return Newtonsoft.Json.JsonConvert.SerializeObject(this);
		}
	}

}