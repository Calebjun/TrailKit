// Author: Caleb
// Purpose: Charge rules for member customers.

namespace TrailKit.Domain.Policies;

/// <summary>
/// Charge rules for member customers: one grace day and lower fees.
/// </summary>
public class MemberChargePolicy : IChargePolicy
{
    /// <summary>Late fee per day for members.</summary>
    private const decimal _lateRatePerDay = 10.00m;

    /// <summary>Late days forgiven for members.</summary>
    private const int _graceDays = 1;

    /// <summary>Cleaning fee for members.</summary>
    private const decimal _cleaningFee = 10.00m;

    /// <inheritdoc />
    public decimal LateRatePerDay => _lateRatePerDay;

    /// <inheritdoc />
    public int GraceDays => _graceDays;

    /// <inheritdoc />
    public decimal CleaningFee => _cleaningFee;
}