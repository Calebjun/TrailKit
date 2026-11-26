// Author: Alison
// Purpose: The current availability status of a gear item.

namespace TrailKit.Domain.Enums;

/// <summary>
/// The current availability status of a gear item.
/// </summary>
public enum GearStatus
{
    Available,
    Unavailable,
    UnderRepair,
    NeedsCleaning,
    Retired
}