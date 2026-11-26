// Author: Caleb
// Purpose: The stages a rental deposit moves through.

namespace TrailKit.Domain.Enums;

/// <summary>
/// Where a deposit stands, from pickup until it is settled at return.
/// </summary>
public enum HoldState
{
    /// <summary>Taken at pickup and waiting for the item to come back.</summary>
    Held,

    /// <summary>Given back in full at return.</summary>
    Released,

    /// <summary>Used, fully or partly, to pay charges at return.</summary>
    Applied
}