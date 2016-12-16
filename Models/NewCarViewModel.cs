using System.ComponentModel.DataAnnotations;

namespace carRentals.Models
{
    public class NewCarViewModel : BaseEntity
    {
        [Required(ErrorMessage = "Make can not be blank")]
		[Display(Name = "Car Make:")]
        public string make { get; set; }
        [Required(ErrorMessage = "Model can not be blank")]
		[Display(Name = "Car Model:")]
        public string model { get; set; }
    }
}