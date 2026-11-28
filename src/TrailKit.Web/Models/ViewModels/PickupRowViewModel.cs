// Author: Caleb
// Purpose: One reservation waiting for pickup, as shown in a row on the Pickup Desk.

using System.ComponentModel.DataAnnotations;
using TrailKit.Domain.Enums;

namespace TrailKit.Web.Models.ViewModels;

/// <summary>
/// The details staff need to hand one reserved item to a customer.
/// </summary>
public class PickupRowViewModel
{
    /// <summary>The reservation being picked up.</summary>
    public int ReservationId { get; set; }

    /// <summary>The customer's full name.</summary>
    [Display(Name = "Customer")]
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>The reserved gear item's name, such as "2-Person Dome Tent".</summary>
    [Display(Name = "Gear")]
    public string GearName { get; set; } = string.Empty;

    /// <summary>The serial number staff must match against the item on the shelf.</summary>
    [Display(Name = "Serial")]
    public string GearSerial { get; set; } = string.Empty;

    /// <summary>The reserved pickup date.</summary>
    [Display(Name = "Pickup")]
    [DataType(DataType.Date)]
    public DateTime PickupDate { get; set; }

    /// <summary>The reserved return date, which becomes the rental's due date.</summary>
    [Display(Name = "Due back")]
    [DataType(DataType.Date)]
    public DateTime ReturnDate { get; set; }

    /// <summary>The deposit to collect for this gear type.</summary>
    [Display(Name = "Deposit")]
    [DataType(DataType.Currency)]
    public decimal DepositAmount { get; set; }

    /// <summary>The item's maintenance alert, shown as a badge beside it.</summary>
    public MaintenanceAlertLevel MaintenanceAlert { get; set; }

    /// <summary>Rentals left before the item needs maintenance.</summary>
    public int UsesUntilDue { get; set; }

    /// <summary>
    /// False when the item is due for maintenance. The Pick Up button is disabled
    /// and staff swap in another item of the same type.
    /// </summary>
    public bool CanPickUp => MaintenanceAlert != MaintenanceAlertLevel.Due;
}