using System;

namespace carRentals.Models
{
    public class Rental : BaseEntity
    {
        public long carid { get; set; }
        public long userid { get; set; }
        public User user { get; set; }
        public DateTime rented_at { get; set; }
        public DateTime return_at { get; set; }
        public DateTime created_at{ get; set; }
		public DateTime updated_at{ get; set; }
    }
}