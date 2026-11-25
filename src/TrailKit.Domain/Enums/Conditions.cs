// Author: Caleb
// Purpose: The condition counter staff record when a customer returns gear.

namespace TrailKit.Domain.Enums;

/// <summary>
/// The state a gear item is in when it comes back to the counter.
/// </summary>
public enum Condition
{
    /// <summary>No damage. The item goes back to Available.</summary>
    Good,

    /// <summary>Normal wear from use. The item goes back to Available.</summary>
    Worn,

    /// <summary>Broken or damaged. The item goes to Under Repair and a damage charge applies.</summary>
    Damaged
}