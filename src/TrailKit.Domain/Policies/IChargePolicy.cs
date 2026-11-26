// Author: Caleb
// Purpose: The charge rules that differ between customer types.

namespace TrailKit.Domain.Policies;

/// <summary>
/// The late and cleaning charge rules for one type of customer.
/// Each customer type returns its own policy, so the calculator never checks customer types itself.
/// </summary>
public interface IChargePolicy
{
    /// <summary>The late fee charged for each day past the due date, after grace days.</summary>
    decimal LateRatePerDay { get; }

    /// <summary>The number of late days forgiven before the late fee starts.</summary>
    int GraceDays { get; }

    /// <summary>The flat fee charged when gear comes back dirty.</summary>
    decimal CleaningFee { get; }
}