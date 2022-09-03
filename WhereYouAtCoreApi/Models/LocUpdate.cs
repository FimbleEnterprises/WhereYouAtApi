using Newtonsoft.Json;
using WhereYouAtCoreApi.Models.Requests;

namespace WhereYouAtCoreApi.Models {


    public class LocUpdate {
        private long Rowid { get; set; } = 0;
        public string? Tripcode { get; set; }
        public long? Memberid { get; set; }
        public string? MemberName { get; set; }
        public decimal? Lat { get; set; }
        public decimal? Lon { get; set; }
        public long? Createdon { get; set; }
        public int? Elevation { get; set; }
        public float? Speed { get; set; }
        public float? Accuracy { get; set; }
        public float? Bearing { get; set; }

        public LocUpdate() { }

        public LocUpdate(string tripcode, long memberid, decimal lat, decimal lon) {
            this.Tripcode = tripcode;
            this.Memberid = memberid;
            this.Lat =  lat;
            this.Lon = lon;
            this.Createdon = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        public LocUpdate(string tripcode, long memberid) {
            this.Tripcode = tripcode;
            this.Createdon = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            this.Memberid = memberid;
        }

        /*[JsonConstructor]
        public LocUpdate(string json) {
            LocUpdate temp = Newtonsoft.Json.JsonConvert.DeserializeObject<LocUpdate>(json)!;
            this.Tripcode = temp.Tripcode;
            this.Memberid = temp.Memberid;
            this.Lat = temp.Lat;
            this.Lon = temp.Lon;
            this.Createdon = temp.Createdon;
            this.Elevation = temp.Elevation;
        }*/
    }
}
