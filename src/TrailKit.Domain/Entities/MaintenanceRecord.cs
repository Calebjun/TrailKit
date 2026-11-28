// Author: Caleb
// Purpose: One maintenance job done on one gear item.

namespace TrailKit.Domain.Entities;

/// <summary>
/// A record of maintenance work on a gear item: what was done, when, and what it cost.
/// Business insights add up these costs to compare against rental revenue.
/// </summary>
public class MaintenanceRecord
{
    /// <summary>Database key, set by the database when the record is saved.</summary>
    private int _id;

    /// <summary>The gear item that was serviced.</summary>
    private int _gearId;

    /// <summary>The date the work was done.</summary>
    private DateTime _servicedOn;

    /// <summary>A short description of the work.</summary>
    private string _workDone = string.Empty;

    /// <summary>What the work cost.</summary>
    private decimal _cost;

    /// <summary>
    /// Used only by Entity Framework Core when it loads a record from the database.
    /// </summary>
    private MaintenanceRecord()
    {
    }

    /// <summary>
    /// Records maintenance work when staff finish servicing an item.
    /// </summary>
    /// <param name="gearId">The gear item that was serviced.</param>
    /// <param name="servicedOn">The date the work was done.</param>
    /// <param name="workDone">A short description of the work.</param>
    /// <param name="cost">What the work cost. Zero is allowed for a free inspection.</param>
    /// <exception cref="ArgumentException">The description is empty.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The cost is negative.</exception>
    public MaintenanceRecord(int gearId, DateTime servicedOn, string workDone, decimal cost)
    {
        if (string.IsNullOrWhiteSpace(workDone))
        {
            throw new ArgumentException("Describe the work that was done.", nameof(workDone));
        }

        if (cost < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(cost), "The cost cannot be negative.");
        }

        _gearId = gearId;
        _servicedOn = servicedOn.Date;
        _workDone = workDone.Trim();
        _cost = cost;
    }

    /// <summary>Database key.</summary>
    public int Id { get => _id; private set => _id = value; }

    /// <summary>The gear item that was serviced.</summary>
    public int GearId { get => _gearId; private set => _gearId = value; }

    /// <summary>The date the work was done.</summary>
    public DateTime ServicedOn { get => _servicedOn; private set => _servicedOn = value; }

    /// <summary>A short description of the work.</summary>
    public string WorkDone { get => _workDone; private set => _workDone = value; }

    /// <summary>What the work cost.</summary>
    public decimal Cost { get => _cost; private set => _cost = value; }
}