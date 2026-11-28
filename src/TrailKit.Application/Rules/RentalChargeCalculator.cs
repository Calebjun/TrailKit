// Author: Caleb
// Purpose: Works out late and cleaning charges when gear is returned.

using TrailKit.Domain.Entities;
using TrailKit.Domain.Policies;

namespace TrailKit.Application.Rules;

/// <summary>
/// Calculates the charges owed on a rental at return.
/// The rates come from the customer's charge policy, so one calculator serves every customer type.
/// </summary>
public class RentalChargeCalculator
{
    /// <summary>
    /// Counts the late days that will be charged, after grace days.
    /// </summary>
    /// <param name="rental">The rental being returned.</param>
    /// <param name="returnedOn">When the item came back, or the date to preview.</param>
    /// <param name="policy">The customer's charge policy.</param>
    /// <returns>The chargeable late days. Zero when the item is on time or within grace days.</returns>
    public int LateDays(Rental rental, DateTime returnedOn, IChargePolicy policy)
    {
        ArgumentNullException.ThrowIfNull(rental);
        ArgumentNullException.ThrowIfNull(policy);

        int daysPastDue = (returnedOn.Date - rental.DueDate.Date).Days;
        int chargeableDays = daysPastDue - policy.GraceDays;

        return Math.Max(0, chargeableDays);
    }

    /// <summary>
    /// Calculates the late fee for a rental.
    /// </summary>
    /// <param name="rental">The rental being returned.</param>
    /// <param name="returnedOn">When the item came back, or the date to preview.</param>
    /// <param name="policy">The customer's charge policy.</param>
    /// <returns>The late fee in dollars.</returns>
    public decimal LateFee(Rental rental, DateTime returnedOn, IChargePolicy policy)
    {
        return LateDays(rental, returnedOn, policy) * policy.LateRatePerDay;
    }

    /// <summary>
    /// Calculates the total charge owed at return: the late fee plus any cleaning fee.
    /// Used both to preview charges on the Return form and to post them when the rental closes.
    /// </summary>
    /// <param name="rental">The rental being returned.</param>
    /// <param name="returnedOn">When the item came back, or the date to preview.</param>
    /// <param name="needsCleaning">True when the item came back dirty.</param>
    /// <param name="policy">The customer's charge policy.</param>
    /// <returns>The total charge in dollars.</returns>
    public decimal Calculate(Rental rental, DateTime returnedOn, bool needsCleaning, IChargePolicy policy)
    {
        decimal total = LateFee(rental, returnedOn, policy);

        if (needsCleaning)
        {
            total += policy.CleaningFee;
        }

        return total;
    }
}