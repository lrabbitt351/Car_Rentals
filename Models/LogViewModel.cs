using System.ComponentModel.DataAnnotations;

namespace carRentals.Models
{
	public class LogViewModel : BaseEntity 
	{ //creates class to validate the login fields against
		[Required(ErrorMessage = "Email address cannot be left blank")]
		[Display(Name = "Email:")]
		public string logemail { get; set; }
		[Required(ErrorMessage = "Password cannot be left blank")]
		[Display(Name = "Password:")]
		[DataType(DataType.Password)]
		public string logpassword { get; set; }
	}
}