using System.ComponentModel.DataAnnotations;

namespace Shaghaf.APIs.DTOs
{
    public class ResetPasswordDto
    {
        [Required]
        [Phone(ErrorMessage = "Invalid phone number.")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "Phone number must be 11 digits.")]
        public string PhoneNumber { get; set; } = null!;

        public string Token { get; set; }


        [Required]
        [RegularExpression("(?=^.{6,10}$)(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&amp;*()_+}{&quot;:;'?/&gt;.&lt;,])(?!.*\\s).*$",
          ErrorMessage = "Password must have 1 Uppercase, 1 Lowercase, 1 number, 1 non alphanumeric and at least 6 characters")]
        public string NewPassword { get; set; } = null!;
    }
}
