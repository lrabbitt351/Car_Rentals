using System.ComponentModel.DataAnnotations;

namespace carRentals.Models
{
	public class LogViewModel : BaseEntity 
	{ //creates class to validate the login fields against
		[Display(Name = "Email:"), Required(ErrorMessage = "Email address cannot be left blank")]
		public string logemail { get; set; }
		[DataType(DataType.Password), Display(Name = "Password:"), Required(ErrorMessage = "Password cannot be left blank")]
		public string logpassword { get; set; }
	}
}