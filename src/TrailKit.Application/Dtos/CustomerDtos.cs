using TrailKit.Domain.Enums;

namespace TrailKit.Application.Dtos;

/// <summary>
/// Author: Utsah Tamang
/// Description: One charge as shown on My Account and Customer Management.
/// </summary>
public class ChargeDto
{
    /// <summary>Charge id.</summary>
    public int Id { get; set; }
    /// <summary>Amount in dollars.</summary>
    public decimal Amount { get; set; }
    /// <summary>Reason the charge was raised.</summary>
    public string Reason { get; set; } = string.Empty;
    /// <summary>Date raised (UTC).</summary>
    public DateTime CreatedAtUtc { get; set; }
    /// <summary>Paid, Waived or Unpaid.</summary>
    public string Status { get; set; } = string.Empty;
    /// <summary>Waive reason when waived.</summary>
    public string? WaivedReason { get; set; }
}

/// <summary>
/// Author: Utsah Tamang
/// Description: Read-only summary of a rental, built from Caleb's rentals through IRentalQueryRepository.
/// </summary>
public class RentalSummary
{
    /// <summary>Gear item name.</summary>
    public string GearName { get; set; } = string.Empty;
    /// <summary>Pickup date.</summary>
    public DateTime PickupDate { get; set; }
    /// <summary>Due date.</summary>
    public DateTime DueDate { get; set; }
    /// <summary>Return date; null while the rental is active.</summary>
    public DateTime? ReturnedDate { get; set; }
    /// <summary>Deposit amount.</summary>
    public decimal DepositAmount { get; set; }
    /// <summary>Held, Released or Applied.</summary>
    public string DepositStatus { get; set; } = string.Empty;
}

/// <summary>
/// Author: Utsah Tamang
/// Description: Everything the My Account page shows.
/// </summary>
public class AccountSummaryDto
{
    /// <summary>Customer id.</summary>
    public int CustomerId { get; set; }
    /// <summary>Customer name.</summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>Customer email.</summary>
    public string Email { get; set; } = string.Empty;
    /// <summary>Good or Suspended.</summary>
    public CustomerStanding Standing { get; set; }
    /// <summary>Reason shown when suspended.</summary>
    public string? SuspensionReason { get; set; }
    /// <summary>Total of unpaid charges.</summary>
    public decimal UnpaidTotal { get; set; }
    /// <summary>Rentals currently out.</summary>
    public IReadOnlyList<RentalSummary> ActiveRentals { get; set; } = new List<RentalSummary>();
    /// <summary>Past rentals.</summary>
    public IReadOnlyList<RentalSummary> RentalHistory { get; set; } = new List<RentalSummary>();
    /// <summary>All charges.</summary>
    public IReadOnlyList<ChargeDto> Charges { get; set; } = new List<ChargeDto>();
}

/// <summary>
/// Author: Utsah Tamang
/// Description: One row of the Customer Management table (JSON from the Web API).
/// </summary>
public class CustomerListItemDto
{
    /// <summary>Customer id.</summary>
    public int Id { get; set; }
    /// <summary>Customer name.</summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>Customer email.</summary>
    public string Email { get; set; } = string.Empty;
    /// <summary>Standing as text (Good or Suspended).</summary>
    public string Standing { get; set; } = string.Empty;
    /// <summary>Reason when suspended.</summary>
    public string? SuspensionReason { get; set; }
    /// <summary>Total of unpaid charges.</summary>
    public decimal UnpaidTotal { get; set; }
}
