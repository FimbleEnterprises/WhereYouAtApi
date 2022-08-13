using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WhereYouAtCoreApi.Models.Results
{
    public class OperationResults
    {
        public List<ApiBaseResult> allResults = new List<ApiBaseResult>();

        public OperationResults() { }

        public OperationResults(ApiBaseResult result)
        {
            allResults.Add(result);
        }

        public string ToJson()
        {
            try
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(this);
            }
            catch (Exception e)
            {
                return "Failed to convert results to json: " + e.Message;
            }
        }
    }
}