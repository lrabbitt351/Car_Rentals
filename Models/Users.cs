using System;
using System.Collections.Generic;

namespace carRentals.Models
{
 public abstract class BaseEntity{}
 public class User : BaseEntity
 {      
        public long user_id { get; set; }
		public string first_name { get; set; }
		public string last_name { get; set; }
		public string email { get; set; }
		public string password { get; set; }
		public DateTime created_at { get; set; }
		public DateTime updated_at { get; set; }
		public ICollection<Rental> rentals { get; set; }
        public bool admin { get; set; }
        public User()
		{
			rentals = new List<Rental>();
		}
 }
}