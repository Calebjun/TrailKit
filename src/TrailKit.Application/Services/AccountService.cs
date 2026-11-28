using System.Net.Mail;
using Microsoft.AspNetCore.Identity;
using TrailKit.Application.Dtos;
using TrailKit.Domain.Entities.Customers;
using TrailKit.Domain.Enums;
using TrailKit.Infrastructure.Repositories;

namespace TrailKit.Application.Services;

/// <summary>
/// Author: Utsah Tamang
/// Description: Contract for UC8 Create Account: register, staff creation and login.
/// </summary>
public interface IAccountService
{
    /// <summary>Registers a new customer (UC8 main flow).</summary>
    Task<ServiceResult<Customer>> RegisterAsync(RegistrationRequest request);

    /// <summary>Lets a manager create a Counter Staff or Manager account.</summary>
    Task<ServiceResult<Customer>> CreateStaffAsync(RegistrationRequest request, UserRole role);

    /// <summary>Checks email and password; returns the customer or null.</summary>
    Task<Customer?> ValidateLoginAsync(string email, string password);

    /// <summary>Creates a first manager account if none exists yet.</summary>
    Task EnsureDefaultManagerAsync(string email, string password);
}

/// <summary>
/// Author: Utsah Tamang
/// Description: Business logic for UC8. Validates input, hashes passwords with the ASP.NET Core Identity
/// password hasher and creates a StandardCustomer with Good standing.
/// </summary>
public class AccountService : IAccountService
{
    /// <summary>Minimum age to register.</summary>
    public const int MinimumAge = 18;

    /// <summary>Minimum password length.</summary>
    public const int MinimumPasswordLength = 8;

    private readonly ICustomerRepository _customerRepository;
    private readonly IPasswordHasher<Customer> _passwordHasher;

    /// <summary>Creates the service.</summary>
    /// <param name="customerRepository">Customer data access.</param>
    /// <param name="passwordHasher">ASP.NET Core Identity password hasher.</param>
    public AccountService(ICustomerRepository customerRepository, IPasswordHasher<Customer> passwordHasher)
    {
        _customerRepository = customerRepository;
        _passwordHasher = passwordHasher;
    }

    /// <inheritdoc />
    public Task<ServiceResult<Customer>> RegisterAsync(RegistrationRequest request) =>
        CreateAccountAsync(request, UserRole.Customer);

    /// <inheritdoc />
    public Task<ServiceResult<Customer>> CreateStaffAsync(RegistrationRequest request, UserRole role) =>
        CreateAccountAsync(request, role);

    /// <inheritdoc />
    public async Task<Customer?> ValidateLoginAsync(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrEmpty(password))
        {
            return null;
        }

        Customer? customer = await _customerRepository.GetByEmailAsync(email);
        if (customer == null)
        {
            return null;
        }

        PasswordVerificationResult result =
            _passwordHasher.VerifyHashedPassword(customer, customer.PasswordHash, password);
        return result == PasswordVerificationResult.Failed ? null : customer;
    }

    /// <inheritdoc />
    public async Task EnsureDefaultManagerAsync(string email, string password)
    {
        if (await _customerRepository.CountByRoleAsync(UserRole.Manager) > 0)
        {
            return;
        }

        await CreateAccountAsync(new RegistrationRequest
        {
            Name = "Store Manager",
            Email = email,
            Phone = "000-000-0000",
            BirthDate = new DateTime(1990, 1, 1),
            Password = password
        }, UserRole.Manager);
    }

    /// <summary>Validates the request and saves the new account.</summary>
    /// <param name="request">Registration data.</param>
    /// <param name="role">Role to give the account.</param>
    /// <returns>The created customer or an error.</returns>
    private async Task<ServiceResult<Customer>> CreateAccountAsync(RegistrationRequest request, UserRole role)
    {
        ServiceResult<Customer>? invalid = Validate(request);
        if (invalid != null)
        {
            return invalid;
        }

        string email = request.Email.Trim().ToLowerInvariant();
        if (await _customerRepository.EmailExistsAsync(email))
        {
            return ServiceResult<Customer>.Fail("That email is already registered.", "EmailTaken");
        }

        var customer = new StandardCustomer
        {
            Name = request.Name.Trim(),
            Email = email,
            Phone = request.Phone.Trim(),
            BirthDate = request.BirthDate.Date,
            Role = role
        };
        customer.PasswordHash = _passwordHasher.HashPassword(customer, request.Password);

        // Loyalty account (UC7) is created by Layan's code; standing starts as Good by default.
        await _customerRepository.AddAsync(customer);
        return ServiceResult<Customer>.Ok(customer);
    }

    /// <summary>Checks the UC8 rules that do not need the database.</summary>
    /// <param name="request">Registration data.</param>
    /// <returns>A failure result, or null when the data is valid.</returns>
    private static ServiceResult<Customer>? Validate(RegistrationRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return ServiceResult<Customer>.Fail("Name is required.");
        }
        if (!IsValidEmail(request.Email))
        {
            return ServiceResult<Customer>.Fail("Enter a valid email address.");
        }
        if (!IsStrongPassword(request.Password))
        {
            return ServiceResult<Customer>.Fail(
                $"Password must be at least {MinimumPasswordLength} characters and contain a number.");
        }
        if (!IsOldEnough(request.BirthDate, DateTime.Today))
        {
            return ServiceResult<Customer>.Fail($"You must be {MinimumAge} or older to register.", "Underage");
        }
        return null;
    }

    /// <summary>Checks the email format.</summary>
    /// <param name="email">Email text.</param>
    /// <returns>True when it looks like an email address.</returns>
    public static bool IsValidEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }
        return MailAddress.TryCreate(email.Trim(), out MailAddress? parsed) && parsed.Address == email.Trim();
    }

    /// <summary>Checks that the password has 8+ characters and at least one digit.</summary>
    /// <param name="password">Password text.</param>
    /// <returns>True when strong enough.</returns>
    public static bool IsStrongPassword(string? password)
    {
        return !string.IsNullOrEmpty(password)
            && password.Length >= MinimumPasswordLength
            && password.Any(char.IsDigit);
    }

    /// <summary>Checks the 18+ rule on a given day.</summary>
    /// <param name="birthDate">Birth date.</param>
    /// <param name="today">Day to measure against.</param>
    /// <returns>True when the person is old enough.</returns>
    public static bool IsOldEnough(DateTime birthDate, DateTime today)
    {
        int age = today.Year - birthDate.Year;
        if (birthDate.Date > today.AddYears(-age).Date)
        {
            age--;
        }
        return age >= MinimumAge;
    }
}
