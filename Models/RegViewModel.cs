using System.ComponentModel.DataAnnotations;

namespace carRentals.Models
{
	public class RegViewModel : BaseEntity
	{
		[Display(Name = "First Name:"), MinLength(2, ErrorMessage = "First name must be at least 2 characters in length."), Required(ErrorMessage = "First name cannot be left blank.")]
		public string first_name { get; set; }
		[Display(Name = "Last Name:"), MinLength(2, ErrorMessage = "Last name must be at least 2 characters in length."), Required(ErrorMessage = "Last name cannot be left blank.")]
		public string last_name { get; set; }
		[Display(Name = "Email:"), EmailAddress(ErrorMessage = "Email is not in the correct format."), Required(ErrorMessage = "Email address cannot be left blank.")]
		public string email { get; set; }
		[Display(Name = "Password:"), DataType(DataType.Password), Compare("confirm", ErrorMessage = "Password and confirm password do not match."), RegularExpression(@"(?=^.{8,}$)(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&amp;*()_+}{&quot;:;&#39;?/&gt;.&lt;,])(?!.*\s).*$", ErrorMessage = "Password must be at least 8 characters and include 1 lowercase letter, 1 uppercase letter, 1 number, and 1 special character."), Required(ErrorMessage = "Password cannot be left blank.")]
		public string password { get; set; }
		[Display(Name = "Confirm Password:"), DataType(DataType.Password), Required(ErrorMessage = "Confirm password cannot be left blank.")]
		public string confirm { get; set; }
	}
}