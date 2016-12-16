using System;

namespace carRentals.Models
{
    public class Rental : BaseEntity
    {
        public long rental_id { get; set; }
        public DateTime rented_at { get; set; }
        public DateTime return_at { get; set; }
        public DateTime created_at{ get; set; }
		public DateTime updated_at{ get; set; }
        public long car_id { get; set; }
        public long user_id { get; set; }
        public User user { get; set; }
        public Car car { get; set; }
    }
}