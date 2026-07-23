using System.ComponentModel.DataAnnotations;
namespace Xenon.Web.Models.ViewModels.Login
{
    public class LoginModel
    {
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Password { get; set; }
        public string ReturnUrl { get; set; } = "/";
    }
}