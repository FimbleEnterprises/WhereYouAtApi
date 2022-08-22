namespace WhereYouAtCoreApi.Models.Results
{
    public class FcmUpsertResult : ApiBaseResult {
        public string Userid { get; set; }
        public string FcmToken { get; set; }

        public FcmUpsertResult(bool wasSuccessful, string userid, string fcmtoken) {
            Userid = userid;
            FcmToken = fcmtoken;
            WasSuccessful = wasSuccessful;
        }
    }
}
