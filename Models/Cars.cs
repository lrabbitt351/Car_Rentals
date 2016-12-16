using System;
using System.Collections.Generic;

namespace carRentals.Models
{
    public class Car : BaseEntity
    {
        public long car_id { get; set; }
        public string make { get; set; }
        public string model { get; set; }
        public int inventory { get; set; }
       	public DateTime created_at { get; set; }
		public DateTime updated_at { get; set; }
		public ICollection<Rental> rentals { get; set; }
        public Car()
		{
			rentals = new List<Rental>();
		}
    }
}