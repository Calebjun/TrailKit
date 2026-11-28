using System.ComponentModel.DataAnnotations;
using TrailKit.Domain.Enums;

namespace TrailKit.Web.Models.ViewModels;

/// <summary>
/// Author: Utsah Tamang
/// Description: Form model for the Register view (UC8). Data annotations give client-side validation.
/// </summary>
public class RegisterViewModel
{
    /// <summary>Full name.</summary>
    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>Email used to sign in.</summary>
    [Required, EmailAddress, StringLength(200)]
    public string Email { get; set; } = string.Empty;

    /// <summary>Phone number.</summary>
    [Required, Phone, StringLength(30)]
    public string Phone { get; set; } = string.Empty;

    /// <summary>Birth date (must be 18 or older).</summary>
    [Required, DataType(DataType.Date), Display(Name = "Birth date")]
    public DateTime BirthDate { get; set; } = DateTime.Today.AddYears(-18);

    /// <summary>Password (8+ characters with a number).</summary>
    [Required, DataType(DataType.Password), MinLength(8)]
    public string Password { get; set; } = string.Empty;

    /// <summary>Password typed a second time.</summary>
    [DataType(DataType.Password), Compare(nameof(Password)), Display(Name = "Confirm password")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

/// <summary>
/// Author: Utsah Tamang
/// Description: Form model for the Login view.
/// </summary>
public class LoginViewModel
{
    /// <summary>Email address.</summary>
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    /// <summary>Password.</summary>
    [Required, DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    /// <summary>Page to return to after login.</summary>
    public string? ReturnUrl { get; set; }
}

/// <summary>
/// Author: Utsah Tamang
/// Description: Form model for a manager creating a staff or manager account.
/// </summary>
public class CreateStaffViewModel : RegisterViewModel
{
    /// <summary>Role to give the new account.</summary>
    [Required]
    public UserRole Role { get; set; } = UserRole.CounterStaff;
}
