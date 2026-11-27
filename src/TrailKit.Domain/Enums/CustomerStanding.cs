// Author: Utsah
// Purpose: The standing of a customer account.

namespace TrailKit.Domain.Enums;

/// <summary>
/// Standing of a customer account. Suspended customers cannot reserve gear.
/// </summary>
public enum CustomerStanding
{
    /// <summary>Customer may reserve and rent gear.</summary>
    Good = 0,

    /// <summary>Customer is blocked from reserving gear.</summary>
    Suspended = 1
}
