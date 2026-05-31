using System.ComponentModel.DataAnnotations;

namespace SmartLib.Option
{
    public class SmtpOption
    {
        public const string Smtp = "Smtp";

        [Required]
        public required string Host { get; set; }

        [Range(1, 65535)]
        public int Port { get; set; }

        [Required]
        [EmailAddress]
        public required string SenderEmail { get; set; }

        [Required]
        public string SenderName { get; set; } = "SmartLib";

        public string? UserName { get; set; }
        public string? Password { get; set; }
    }
}
