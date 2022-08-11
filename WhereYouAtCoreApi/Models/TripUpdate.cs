namespace WhereYouAtCoreApi.Models {


    public class TripUpdate {
        private long Rowid { get; set; } = 0;
        public string Tripcode { get; set; }
        public double Memberid { get; set; }
        public decimal? Lat { get; set; }
        public decimal? Lon { get; set; }
        public double Createdon { get; set; }
        public int? Elevation { get; set; }

        public TripUpdate(string tripcode, double memberid, decimal lat, decimal lon) {
            this.Tripcode = tripcode;
            this.Memberid = memberid;
            this.Lat =  lat;
            this.Lon = lon;
            this.Createdon = DateTime.Now.ToOADate();
        }

        public TripUpdate(string tripcode, double memberid) {
            this.Tripcode = tripcode;
            this.Createdon = DateTime.Now.ToOADate();
            this.Memberid = memberid;
        }

    }
}
