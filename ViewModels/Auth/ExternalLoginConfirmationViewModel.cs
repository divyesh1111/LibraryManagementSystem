using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.ViewModels.Auth
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string FullName { get; set; } = string.Empty;
        public string? ReturnUrl { get; set; }
    }
}