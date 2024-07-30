using System.ComponentModel.DataAnnotations;

namespace Shaghaf.APIs.DTOs
{
    public class ForgetPasswordDto
    {
        [Required]
        [Phone(ErrorMessage = "Invalid phone number.")]
        public string PhoneNumber { get; set; } = null!;
    }
}
