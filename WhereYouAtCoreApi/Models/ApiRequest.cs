
namespace WhereYouAtCoreApi.Models {
    public class ApiRequest {
        // OPERATIONS
        public const string CREATE_NEW_TRIP = "createnewtrip";
        public const string JOIN_TRIP = "jointrip";
        public const string UPSERT_USER = "upsertuser";
        public const string UPDATE_TRIP = "updatetrip";
        public const string UPSERT_FCMTOKEN = "upsertfcmtoken";
        public const string UPSERT_AVATAR = "upsertavatar";
        public const string LOCATION_UPDATE_REQUESTED = "locationupdaterequested";
        public const string LEAVE_TRIP = "leavetrip";
        public const string REQUEST_JOIN = "requestjoin";
        public const string TRIP_EXISTS = "tripexists";
        public const string SEND_MESSAGE = "sendmsg";
        public const string GET_TRIP_MESSAGES = "gettripmessages";
        public const string TEST_FUNCTION = "TEST_FUNCTION";
        public const string TEST_RESPONSE = "testresponse";

        /*public enum Function {
            CREATE_NEW_TRIP,JOIN_TRIP,UPSERT_USER,UPDATE_TRIP,UPSERT_FCMTOKEN,
            UPSERT_AVATAR,LOCATION_UPDATE_REQUESTED,LEAVE_TRIP,REQUEST_JOIN,
            TRIP_EXISTS,SEND_MESSAGE,GET_TRIP_MESSAGES,TEST_FUNCTION,TEST_RESPONSE
        }

        public string GetFunctionName() {
            return RequestFunction switch {
                Function.CREATE_NEW_TRIP => CREATE_NEW_TRIP,
                Function.JOIN_TRIP => JOIN_TRIP,
                Function.UPSERT_USER => UPSERT_USER,
                Function.UPDATE_TRIP => UPDATE_TRIP,
                Function.UPSERT_FCMTOKEN => UPSERT_FCMTOKEN,
                Function.UPSERT_AVATAR => UPSERT_AVATAR,
                Function.LOCATION_UPDATE_REQUESTED => LOCATION_UPDATE_REQUESTED,
                Function.LEAVE_TRIP => LEAVE_TRIP,
                Function.REQUEST_JOIN => REQUEST_JOIN,
                Function.TRIP_EXISTS => TRIP_EXISTS,
                Function.SEND_MESSAGE => SEND_MESSAGE,
                Function.GET_TRIP_MESSAGES => GET_TRIP_MESSAGES,
                Function.TEST_FUNCTION => TEST_FUNCTION,
                Function.TEST_RESPONSE => TEST_RESPONSE,
                _ => CREATE_NEW_TRIP,
            };
        }*/

        public ApiRequest() { 
            this.Function = CREATE_NEW_TRIP;
        }
        public string Function{ get; set; }
        public List<Argument> Arguments { get; set; } = new List<Argument>();

    }
}
