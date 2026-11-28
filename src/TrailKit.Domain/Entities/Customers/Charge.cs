namespace TrailKit.Domain.Entities.Customers;

/// <summary>
/// Author: Utsah Tamang
/// Description: A fee raised against a customer (late fee, cleaning fee, damage). It is paid by the
/// customer or waived by a manager with a reason.
/// </summary>
public class Charge
{
    /// <summary>Primary key.</summary>
    public int Id { get; set; }

    /// <summary>Foreign key to the customer.</summary>
    public int CustomerId { get; set; }

    /// <summary>Navigation to the customer.</summary>
    public Customer? Customer { get; set; }

    /// <summary>Optional link to the rental that caused the charge (owned by UC3).</summary>
    public int? RentalId { get; set; }

    /// <summary>Amount in dollars.</summary>
    public decimal Amount { get; set; }

    /// <summary>Why the charge was raised.</summary>
    public string Reason { get; set; } = string.Empty;

    /// <summary>When the charge was created (UTC).</summary>
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    /// <summary>True once the customer has paid.</summary>
    public bool IsPaid { get; private set; }

    /// <summary>When the payment was recorded (UTC).</summary>
    public DateTime? PaidAtUtc { get; private set; }

    /// <summary>True when a manager waived the charge.</summary>
    public bool IsWaived { get; private set; }

    /// <summary>Manager's reason for waiving.</summary>
    public string? WaivedReason { get; private set; }

    /// <summary>True while the charge still counts toward the customer's unpaid total.</summary>
    public bool IsOutstanding => !IsPaid && !IsWaived;

    /// <summary>Records a payment. Does nothing harmful if the charge is already settled.</summary>
    /// <returns>True if the charge was outstanding and is now paid.</returns>
    public bool MarkPaid()
    {
        if (!IsOutstanding)
        {
            return false;
        }
        IsPaid = true;
        PaidAtUtc = DateTime.UtcNow;
        return true;
    }

    /// <summary>Waives the charge with a reason.</summary>
    /// <param name="reason">Required explanation from the manager.</param>
    /// <returns>True if the charge was outstanding and is now waived.</returns>
    public bool Waive(string reason)
    {
        if (!IsOutstanding || string.IsNullOrWhiteSpace(reason))
        {
            return false;
        }
        IsWaived = true;
        WaivedReason = reason.Trim();
        return true;
    }
}
