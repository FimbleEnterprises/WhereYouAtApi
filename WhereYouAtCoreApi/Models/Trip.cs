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
		public long Id { get; set; }
		public string? tripcode;
		public long createdon;
		public long? createdby;
		public List<LocUpdate>? members = new();

		public Trip() {
			this.createdon = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        } 

		public string ToJson() {
			return Newtonsoft.Json.JsonConvert.SerializeObject(this);
		}

	}


}