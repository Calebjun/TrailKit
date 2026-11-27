// Author: Utsah
// Purpose: The role of a signed-in user, used for authorization.

namespace TrailKit.Domain.Enums;

/// <summary>
/// Role of a signed-in user. Decides which pages and API calls are allowed.
/// </summary>
public enum UserRole
{
    /// <summary>A normal customer.</summary>
    Customer = 0,

    /// <summary>Counter staff who hand out and inspect gear.</summary>
    CounterStaff = 1,

    /// <summary>Store manager who maintains the catalog and customers.</summary>
    Manager = 2
}
