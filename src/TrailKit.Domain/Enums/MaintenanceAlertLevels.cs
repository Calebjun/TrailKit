// Author: Caleb
// Purpose: How urgently a gear item needs maintenance.

namespace TrailKit.Domain.Enums;

/// <summary>
/// The maintenance alert shown for a gear item on the Maintenance Dashboard and Pickup Desk.
/// </summary>
public enum MaintenanceAlertLevel
{
    /// <summary>The item is well below its threshold. No alert.</summary>
    None,

    /// <summary>The item has used 80% or more of its threshold. Shown as a warning.</summary>
    Approaching,

    /// <summary>The item has reached its threshold and must be serviced before it goes out again.</summary>
    Due
}