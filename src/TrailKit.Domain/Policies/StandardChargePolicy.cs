// Author: Caleb
// Purpose: Charge rules for standard customers.

namespace TrailKit.Domain.Policies;

/// <summary>
/// Charge rules for standard customers: no grace days and full fees.
/// </summary>
public class StandardChargePolicy : IChargePolicy
{
    /// <summary>Late fee per day for standard customers.</summary>
    private const decimal _lateRatePerDay = 15.00m;

    /// <summary>Late days forgiven for standard customers.</summary>
    private const int _graceDays = 0;

    /// <summary>Cleaning fee for standard customers.</summary>
    private const decimal _cleaningFee = 20.00m;

    /// <inheritdoc />
    public decimal LateRatePerDay => _lateRatePerDay;

    /// <inheritdoc />
    public int GraceDays => _graceDays;

    /// <inheritdoc />
    public decimal CleaningFee => _cleaningFee;
}