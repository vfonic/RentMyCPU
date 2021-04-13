using System.ComponentModel.DataAnnotations;

namespace RentMyCPU.Shared
{
    public class TokenAuthViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
