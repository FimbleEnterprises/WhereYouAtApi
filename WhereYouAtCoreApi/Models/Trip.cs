using MessagePack;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WhereYouAtCoreApi.Models;
using KeyAttribute = System.ComponentModel.DataAnnotations.KeyAttribute;

namespace WhereYouAt.Api {

	public class Trip {

		[Key]
		public long id { get; set; }
		public string? tripcode;
		public double? createdon;
		public double? createdby;
		public List<LocUpdate>? members = new();

		public Trip() {}

		public Trip(string tripcode) {
			this.tripcode = tripcode;
			this.createdon = DateTime.Now.ToOADate();
		}

		public Trip(string tripcode, long memberid, decimal lat, decimal lon) {
			this.tripcode = tripcode;
			this.members.Add(new LocUpdate(tripcode, memberid, lat, lon));
		}

		public string ToJson() {
			return Newtonsoft.Json.JsonConvert.SerializeObject(this);
		}

	}


}