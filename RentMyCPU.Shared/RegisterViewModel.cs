using System.ComponentModel.DataAnnotations;

namespace RentMyCPU.Shared
{
    public class RegisterViewModel
    {
        [Required]
        public string Password { get; set; }
        [Compare(nameof(Password)), Required]
        public string ConfirmPassword { get; set; }
        [EmailAddress, Required]
        public string UserName { get; set; }
    }
}
