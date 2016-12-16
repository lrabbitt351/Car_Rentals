using System.ComponentModel.DataAnnotations;

namespace carRentals.Models
{
    public class NewCarViewModel : BaseEntity
    {
		[Display(Name = "Car Make:"), Required(ErrorMessage = "Make can not be blank")]
        public string make { get; set; }
		[Display(Name = "Car Model:"), Required(ErrorMessage = "Model can not be blank")]
        public string carmodel { get; set; }
		[Display(Name = "Inventory:")]
        public int inventory { get; set; }
    }
}