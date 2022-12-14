#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
namespace CSharpBelt.Models;
public class LogUser
{
    [Required(ErrorMessage = "Email/Password is required")]
    [EmailAddress]
    [Display(Name = "Email")]
    public string LEmail { get; set; }

    [Required(ErrorMessage = "Email/Password is required")]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string LPassword { get; set; }
}