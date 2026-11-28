// Author: Caleb
// Purpose: Decides when gear needs maintenance, based on how many times it has been rented.

using TrailKit.Domain.Enums;
using TrailKit.Domain.Entities;

namespace TrailKit.Application.Rules;

/// <summary>
/// Compares a gear item's use count with its maintenance threshold and raises alerts.
/// Each gear type sets its own threshold, so this class works for tents, kayaks and bikes alike.
/// </summary>
public class MaintenanceMonitor
{
    /// <summary>The share of the threshold, in percent, at which a warning starts.</summary>
    private const int _approachingPercent = 80;

    /// <summary>
    /// Works out the alert level for an item.
    /// </summary>
    /// <param name="useCount">Rentals since the item was last serviced.</param>
    /// <param name="threshold">Rentals allowed between services for this gear type.</param>
    /// <returns>Due at or past the threshold, Approaching from 80% of it, otherwise None.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The use count is negative or the threshold is not positive.</exception>
    public MaintenanceAlertLevel Check(int useCount, int threshold)
    {
        Validate(useCount, threshold);

        if (useCount >= threshold)
        {
            return MaintenanceAlertLevel.Due;
        }

        // Whole-number math avoids rounding: 7 of 8 uses is 87.5%, so 700 >= 640.
        if (useCount * 100 >= threshold * _approachingPercent)
        {
            return MaintenanceAlertLevel.Approaching;
        }

        return MaintenanceAlertLevel.None;
    }

    /// <summary>
    /// Works out the alert level for a gear item, using its type's threshold.
    /// </summary>
    /// <param name="gear">The gear item to check.</param>
    /// <returns>The item's alert level.</returns>
    public MaintenanceAlertLevel Check(Gear gear) => Check(gear.UseCount, gear.MaintenanceThreshold);

    /// <summary>
    /// Checks whether an item must be serviced before it can be rented again.
    /// </summary>
    /// <param name="useCount">Rentals since the item was last serviced.</param>
    /// <param name="threshold">Rentals allowed between services for this gear type.</param>
    /// <returns>True when the item has reached its threshold.</returns>
    public bool IsDue(int useCount, int threshold)
    {
        return Check(useCount, threshold) == MaintenanceAlertLevel.Due;
    }

    /// <summary>
    /// Counts the rentals left before an item is due. Shown on the Pickup Desk.
    /// </summary>
    /// <param name="useCount">Rentals since the item was last serviced.</param>
    /// <param name="threshold">Rentals allowed between services for this gear type.</param>
    /// <returns>The rentals left, or zero when the item is already due.</returns>
    public int UsesUntilDue(int useCount, int threshold)
    {
        Validate(useCount, threshold);

        return Math.Max(0, threshold - useCount);
    }

    /// <summary>
    /// Counts how far an item has gone past its threshold. Used to sort the Maintenance Dashboard,
    /// so the most overused items appear first.
    /// </summary>
    /// <param name="useCount">Rentals since the item was last serviced.</param>
    /// <param name="threshold">Rentals allowed between services for this gear type.</param>
    /// <returns>The rentals past the threshold, or zero when the item is not past it.</returns>
    public int UsesPastThreshold(int useCount, int threshold)
    {
        Validate(useCount, threshold);

        return Math.Max(0, useCount - threshold);
    }

    /// <summary>
    /// Rejects impossible inputs before any rule runs.
    /// </summary>
    /// <param name="useCount">Rentals since the item was last serviced.</param>
    /// <param name="threshold">Rentals allowed between services for this gear type.</param>
    /// <exception cref="ArgumentOutOfRangeException">The use count is negative or the threshold is not positive.</exception>
    private static void Validate(int useCount, int threshold)
    {
        if (useCount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(useCount), "The use count cannot be negative.");
        }

        if (threshold <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(threshold), "The threshold must be more than zero.");
        }
    }
}