using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrailKit.Application.Dtos;
using TrailKit.Application.Services;
using TrailKit.Domain.Entities.Customers;
using TrailKit.Domain.Enums;
using TrailKit.Web.Models.ViewModels;

namespace TrailKit.Web.Controllers;

/// <summary>
/// Author: Utsah Tamang
/// Description: MVC controller for UC4 (My Account, Customer Management) and UC8 (Register, Login,
/// staff creation). Contains no business rules; it calls the services and picks the view.
/// </summary>
[Authorize]
public class CustomersController : Controller
{
    private readonly ICustomerService _customerService;
    private readonly IAccountService _accountService;

    /// <summary>Creates the controller.</summary>
    /// <param name="customerService">UC4 service.</param>
    /// <param name="accountService">UC8 service.</param>
    public CustomersController(ICustomerService customerService, IAccountService accountService)
    {
        _customerService = customerService;
        _accountService = accountService;
    }

    /// <summary>Shows the registration form.</summary>
    /// <returns>The Register view.</returns>
    [AllowAnonymous, HttpGet]
    public IActionResult Register() => View(new RegisterViewModel());

    /// <summary>Registers a visitor, signs them in and opens the catalog.</summary>
    /// <param name="model">Submitted form.</param>
    /// <returns>Redirect to the Catalog, or the form with errors.</returns>
    [AllowAnonymous, HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        ServiceResult<Customer> result = await _accountService.RegisterAsync(new RegistrationRequest
        {
            Name = model.Name,
            Email = model.Email,
            Phone = model.Phone,
            BirthDate = model.BirthDate,
            Password = model.Password
        });

        if (!result.Success)
        {
            ModelState.AddModelError(result.Code == "EmailTaken" ? nameof(model.Email) : string.Empty, result.Error!);
            ViewBag.EmailTaken = result.Code == "EmailTaken";
            return View(model);
        }

        await SignInAsync(result.Value!);
        return RedirectToAction("Index", "Catalog"); // Alison's controller
    }

    /// <summary>Shows the login form.</summary>
    /// <param name="returnUrl">Page to return to afterwards.</param>
    /// <returns>The Login view.</returns>
    [AllowAnonymous, HttpGet]
    public IActionResult Login(string? returnUrl = null) => View(new LoginViewModel { ReturnUrl = returnUrl });

    /// <summary>Checks the password hash and signs the user in.</summary>
    /// <param name="model">Submitted form.</param>
    /// <returns>Redirect on success, otherwise the form with an error.</returns>
    [AllowAnonymous, HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        Customer? customer = await _accountService.ValidateLoginAsync(model.Email, model.Password);
        if (customer == null)
        {
            ModelState.AddModelError(string.Empty, "Email or password is incorrect.");
            return View(model);
        }

        await SignInAsync(customer);
        if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
        {
            return Redirect(model.ReturnUrl);
        }
        return customer.Role == UserRole.Customer
            ? RedirectToAction("Index", "Catalog")
            : RedirectToAction("Index", "Home");
    }

    /// <summary>Signs the user out.</summary>
    /// <returns>Redirect to the landing page.</returns>
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    /// <summary>Shows the signed-in customer's rentals, history, charges and deposits.</summary>
    /// <returns>The MyAccount view.</returns>
    [HttpGet]
    public async Task<IActionResult> MyAccount()
    {
        int id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        ServiceResult<AccountSummaryDto> result = await _customerService.GetAccountSummaryAsync(id);
        if (!result.Success)
        {
            return NotFound();
        }
        return View(result.Value);
    }

    /// <summary>Shows the manager's customer table (data loads through the Web API).</summary>
    /// <returns>The Manage view.</returns>
    [Authorize(Roles = nameof(UserRole.Manager)), HttpGet]
    public IActionResult Manage() => View();

    /// <summary>Shows the form for creating a staff or manager account.</summary>
    /// <returns>The CreateStaff view.</returns>
    [Authorize(Roles = nameof(UserRole.Manager)), HttpGet]
    public IActionResult CreateStaff() => View(new CreateStaffViewModel());

    /// <summary>Creates a staff or manager account with the chosen role.</summary>
    /// <param name="model">Submitted form.</param>
    /// <returns>Redirect to Manage on success.</returns>
    [Authorize(Roles = nameof(UserRole.Manager)), HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateStaff(CreateStaffViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        ServiceResult<Customer> result = await _accountService.CreateStaffAsync(new RegistrationRequest
        {
            Name = model.Name,
            Email = model.Email,
            Phone = model.Phone,
            BirthDate = model.BirthDate,
            Password = model.Password
        }, model.Role);

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Error!);
            return View(model);
        }

        TempData["Message"] = $"{model.Role} account created for {model.Name}.";
        return RedirectToAction(nameof(Manage));
    }

    /// <summary>Shown when a signed-in user opens a page their role cannot use.</summary>
    /// <returns>The AccessDenied view.</returns>
    [HttpGet]
    public IActionResult AccessDenied() => View();

    /// <summary>Creates the auth cookie with id, name, email and role claims.</summary>
    /// <param name="customer">The user to sign in.</param>
    private async Task SignInAsync(Customer customer)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, customer.Id.ToString()),
            new Claim(ClaimTypes.Name, customer.Name),
            new Claim(ClaimTypes.Email, customer.Email),
            new Claim(ClaimTypes.Role, customer.Role.ToString())
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
    }
}
