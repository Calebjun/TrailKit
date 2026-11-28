// Author: Caleb
// Purpose: Everything the Pickup Desk page shows.

using System.ComponentModel.DataAnnotations;

namespace TrailKit.Web.Models.ViewModels;

/// <summary>
/// The Pickup Desk: a customer search box and the reservations ready to hand out today.
/// </summary>
public class PickupDeskViewModel
{
    /// <summary>The customer name, email or reservation number staff searched for.</summary>
    [Display(Name = "Customer or reservation #")]
    [StringLength(100)]
    public string? Search { get; set; }

    /// <summary>The date shown on the desk, normally today.</summary>
    [DataType(DataType.Date)]
    public DateTime Date { get; set; } = DateTime.Today;

    /// <summary>The reservations matching the search, or all of today's when there is no search.</summary>
    public List<PickupRowViewModel> Reservations { get; set; } = new();

    /// <summary>A confirmation shown after a successful pickup, such as "Kayak K-102 handed to Sam Lee".</summary>
    public string? StatusMessage { get; set; }

    /// <summary>True when the list is empty, so the page can show a friendly message instead of a blank table.</summary>
    public bool HasNoResults => Reservations.Count == 0;
}