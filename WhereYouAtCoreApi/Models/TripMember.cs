namespace WhereYouAtCoreApi.Models {
    public class TripMember {

        public long Id { get; set; }
        public long memberid;

        public TripMember(long memberid) {
            this.memberid = memberid;
        }

    }
}
