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
        public decimal? Speed { get; set; } 
		public decimal? Bearing { get; set; }
		public decimal? Accuracy { get; set; }
		public string? DisplayName { get; set; }
		public string? GoogleId { get; set; }
		public string? AvatarUrl { get; set; }
		public string? Token { get; set; }
        public string? Email { get; set; }

		public LocUpdate() { }

        public LocUpdate(string tripcode, long memberid, decimal lat, decimal lon, decimal? speed = 0, string? displayname = null, string? token = null, string? googleid = null, string? avatarurl = null, string? email = null) {
            this.Tripcode = tripcode;
            this.Memberid = memberid;
            this.Lat =  lat;
            this.Lon = lon;
            this.Createdon = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            this.Token = token;
            this.DisplayName = displayname;
            this.GoogleId = googleid;
            this.AvatarUrl = avatarurl;
            this.Email = email;
            this.Speed = speed;
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
