using System.ComponentModel.DataAnnotations;

namespace SmartLib.Models.Dto.Create
{
    public class ResendConfirmationEmailDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
