#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Text.RegularExpressions;
namespace CSharpBelt.Models;

public class User
{
    [Key]
    public int UserId {get;set;}

    [Required]
    [MinLength(2, ErrorMessage = "Must be atleast 2 characters")]
    public string Name {get;set;}

    [Required]
    [EmailAddress]
    [UniqueEmail]
    public string Email {get;set;}

    [Required]
    [DataType(DataType.Password)]
    [MinLength(8, ErrorMessage = "Must be atleast 8 characters")]
    [ValidPassword]
    public string Password {get;set;}

    [NotMapped]
    [Compare("Password")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirm Password")]
    public string Confirm {get;set;}

    public List<Post> CreatedPosts {get;set;} = new List<Post>();

    public List<Like> LikesU {get;set;} = new List<Like>();

    public DateTime CreatedAt {get;set;} = DateTime.Now;

    public DateTime UpdatedAt {get;set;} = DateTime.Now;
}

public class UniqueEmailAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if(value == null)
        {
            return new ValidationResult("Email is required");
        }

        MyContext _context = (MyContext)validationContext.GetService(typeof(MyContext));
        if(_context.Users.Any(u => u.Email == value.ToString()))
        {
            return new ValidationResult("Email must be unique!");
        }
        else
        {
            return ValidationResult.Success;
        }
    }
}

public class ValidPasswordAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if(value==null)
        {
            return new ValidationResult("Password is Required");
        }
        Match matched = Regex.Match( (string)value , @"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{8,}$");
        if(matched.Success)
        {
            return ValidationResult.Success;
        }
        else
        {
            return new ValidationResult("Must contain a number, a letter and special character");
        }
    }
}