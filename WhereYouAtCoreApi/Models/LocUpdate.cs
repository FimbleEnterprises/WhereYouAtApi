namespace WhereYouAtCoreApi.Models {


    public class LocUpdate {
        private long Rowid { get; set; } = 0;
        public string Tripcode { get; set; }
        public double Memberid { get; set; }
        public decimal? Lat { get; set; }
        public decimal? Lon { get; set; }
        public DateTime Createdon { get; set; }
        public int? Elevation { get; set; }
        public string? Json{ get; set; }

        public LocUpdate(string tripcode, double memberid, decimal lat, decimal lon) {
            this.Tripcode = tripcode;
            this.Memberid = memberid;
            this.Lat =  lat;
            this.Lon = lon;
            this.Createdon = DateTime.UtcNow;
        }

        public LocUpdate(string tripcode, double memberid) {
            this.Tripcode = tripcode;
            this.Createdon = DateTime.UtcNow;
            this.Memberid = memberid;
        }

    }
}
