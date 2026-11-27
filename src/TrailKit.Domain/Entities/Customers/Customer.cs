using TrailKit.Domain.Enums;
using TrailKit.Domain.Policies;

namespace TrailKit.Domain.Entities.Customers;

/// <summary>
/// Author: Utsah Tamang
/// Description: Abstract base entity for every account holder (customers, counter staff and managers).
/// Subclasses decide the rental limit and the charge policy. Standing can only be changed through.
/// </summary>
public abstract class Customer
{
    /// <summary>Primary key.</summary>
    public int Id { get; set; }

    /// <summary>Full name of the account holder.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Unique, lower-cased email used to sign in.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Contact phone number.</summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>Birth date</summary>
    public DateTime BirthDate { get; set; }

    /// <summary>Password hash produced by the ASP.NET Core Identity password hasher.</summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>Role that decides which pages the user may open.</summary>
    public UserRole Role { get; set; } = UserRole.Customer;

    /// <summary>When the account was created (UTC).</summary>
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    /// <summary>Current standing. Changed only through ApplyStanding.</summary>
    public CustomerStanding Standing { get; private set; } = CustomerStanding.Good;

    /// <summary>Why the customer is suspended; null when in good standing.</summary>
    public string? SuspensionReason { get; private set; }

    /// <summary>All charges ever raised against this customer.</summary>
    public ICollection<Charge> Charges { get; set; } = new List<Charge>();

    /// <summary>Maximum number of rentals the customer may hold at the same time.</summary>
    public abstract int MaxActiveRentals { get; }

    /// <summary>Returns the late-fee and cleaning-fee policy for this customer type.</summary>
    /// <returns>The charge policy used by RentalChargeCalculator.</returns>
    public abstract IChargePolicy GetPolicy();

    /// <summary>Sum of charges that are neither paid nor waived.</summary>
    public decimal UnpaidChargesTotal =>
        Charges.Where(c => c.IsOutstanding).Sum(c => c.Amount);

    /// <summary>Sets the standing and its reason together.</summary>
    /// <param name="standing">New standing.</param>
    /// <param name="reason">Reason shown to the customer when suspended; ignored when Good.</param>
    public void ApplyStanding(CustomerStanding standing, string? reason)
    {
        Standing = standing;
        SuspensionReason = standing == CustomerStanding.Suspended ? reason : null;
    }

    /// <summary>Calculates the age in whole years on a given day.</summary>
    /// <param name="today">The day to measure against.</param>
    /// <returns>Age in years.</returns>
    public int GetAge(DateTime today)
    {
        int age = today.Year - BirthDate.Year;
        if (BirthDate.Date > today.AddYears(-age).Date)
        {
            age--;
        }
        return age;
    }
}
