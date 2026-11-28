// Author: Caleb
// Purpose: The Return form: what staff see about the rental and what they enter when gear comes back.

using System.ComponentModel.DataAnnotations;
using TrailKit.Domain.Enums;

namespace TrailKit.Web.Models.ViewModels;

/// <summary>
/// Shows an active rental and collects the return details.
/// The display fields are filled by the controller. The input fields come back from the form.
/// </summary>
public class ReturnFormViewModel : IValidatableObject
{
    /// <summary>The rental being returned. Sent back in a hidden field.</summary>
    public int RentalId { get; set; }

    /// <summary>The customer's full name. Display only.</summary>
    [Display(Name = "Customer")]
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>The gear item's name and serial number. Display only.</summary>
    [Display(Name = "Gear")]
    public string GearName { get; set; } = string.Empty;

    /// <summary>When the item was picked up. Display only.</summary>
    [Display(Name = "Picked up")]
    [DataType(DataType.Date)]
    public DateTime PickedUpOn { get; set; }

    /// <summary>The date the item was due. Display only.</summary>
    [Display(Name = "Due")]
    [DataType(DataType.Date)]
    public DateTime DueDate { get; set; }

    /// <summary>The deposit held on this rental. Display only.</summary>
    [Display(Name = "Deposit held")]
    [DataType(DataType.Currency)]
    public decimal DepositAmount { get; set; }

    /// <summary>When the item came back. Defaults to now and staff can correct it.</summary>
    [Required]
    [Display(Name = "Returned on")]
    [DataType(DataType.DateTime)]
    public DateTime ReturnedOn { get; set; } = DateTime.Now;

    /// <summary>The condition staff recorded. Nullable so the form starts with no choice made.</summary>
    [Required(ErrorMessage = "Choose the item's condition.")]
    [Display(Name = "Condition")]
    public Condition? Condition { get; set; }

    /// <summary>True when the item came back dirty and needs cleaning.</summary>
    [Display(Name = "Needs cleaning")]
    public bool NeedsCleaning { get; set; }

    /// <summary>The repair cost charged when the item is damaged. Required only for damaged items.</summary>
    [Display(Name = "Damage charge")]
    [DataType(DataType.Currency)]
    [Range(0.01, 5000, ErrorMessage = "Enter a damage charge between $0.01 and $5,000.")]
    public decimal? DamageCharge { get; set; }

    /// <summary>Optional notes about the return, such as what was damaged.</summary>
    [Display(Name = "Notes")]
    [StringLength(500)]
    public string? Notes { get; set; }

    /// <summary>
    /// Checks rules that involve more than one field, after the single-field rules pass.
    /// </summary>
    /// <param name="validationContext">Supplied by ASP.NET Core during model validation.</param>
    /// <returns>One error for each rule that fails.</returns>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Condition == Domain.Enums.Condition.Damaged && DamageCharge is null)
        {
            yield return new ValidationResult(
                "Enter a damage charge for a damaged item.",
                new[] { nameof(DamageCharge) });
        }

        if (Condition != Domain.Enums.Condition.Damaged && DamageCharge is not null)
        {
            yield return new ValidationResult(
                "Only damaged items get a damage charge.",
                new[] { nameof(DamageCharge) });
        }

        if (ReturnedOn < PickedUpOn)
        {
            yield return new ValidationResult(
                "The return date cannot be before the pickup date.",
                new[] { nameof(ReturnedOn) });
        }

        if (ReturnedOn > DateTime.Now.AddMinutes(5))
        {
            yield return new ValidationResult(
                "The return date cannot be in the future.",
                new[] { nameof(ReturnedOn) });
        }
    }
}