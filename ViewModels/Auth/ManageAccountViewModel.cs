using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.ViewModels.Auth
{
    public class ManageAccountViewModel
    {
        [Required, StringLength(120)]
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}