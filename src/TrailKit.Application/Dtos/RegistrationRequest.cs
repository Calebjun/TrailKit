namespace TrailKit.Application.Dtos;

/// <summary>
/// Author: Utsah Tamang
/// Description: Data needed to register a new account.
/// </summary>
public class RegistrationRequest
{
    /// <summary>Full name.</summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>Email address.</summary>
    public string Email { get; set; } = string.Empty;
    /// <summary>Phone number.</summary>
    public string Phone { get; set; } = string.Empty;
    /// <summary>Birth date.</summary>
    public DateTime BirthDate { get; set; }
    /// <summary>Plain-text password (hashed before saving, never stored).</summary>
    public string Password { get; set; } = string.Empty;
}
