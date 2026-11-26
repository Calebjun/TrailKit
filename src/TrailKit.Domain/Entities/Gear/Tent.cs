// Author: Alison
// Purpose: To represent a concrete Tent available for rent in the catalog.

namespace TrailKit.Domain.Entities.Gear;

/// <summary>
/// A single tent in the catalog.
/// </summary>
public class Tent : Gear
{
    #region Fields
    /// <summary>The type of tent. E.g. backpacking, winter camping, car camping, etc.</summary>
    private string _tentType;
    
    /// <summary>The maximum amount of people the tent can sleep.</summary>
    private int _sleepingCapacity;

    /// <summary>The tent's packed weight, in pounds.</summary>
    private decimal _weight;
    #endregion
    
    #region Properties
    /// <summary>Gets/sets the type of tent.</summary>
    public string TentType { get => _tentType; private set => _tentType = value; }
    
    /// <summary>Gets/sets the tent's maximum sleeping capacity.</summary>
    public int SleepingCapacity { get => _sleepingCapacity; private set => _sleepingCapacity = value; }
    
    /// <summary>Gets/sets the tent's packed weight, in pounds.</summary>
    public decimal Weight { get => _weight; private set => _weight = value; }
    #endregion
    
    #region Constructors
    /// <summary>
    /// Creates a new tent item.
    /// </summary>
    /// <param name="serialNumber">Internal serial number.</param>
    /// <param name="name">Name of gear item.</param>
    /// <param name="description">Optional description.</param>
    /// <param name="purchasePrice">Purchase price of gear in $CAD.</param>
    /// <param name="lifespan">Expected lifespan of gear in years.</param>
    /// <param name="expectedRentalDays">Expected number of days in a year it will be rented out.</param>
    /// <param name="tentType">Type of tent. E.g. backpacking, winter camping, car camping, etc.</param>
    /// <param name="sleepingCapacity">Maximum amount of people the tent can sleep.</param>
    /// <param name="weight">Tent's packed weight, in pounds.</param>
    public Tent(string serialNumber, string name, string? description, decimal purchasePrice, int lifespan,
        int expectedRentalDays, string tentType, int sleepingCapacity, decimal weight)
        : base(serialNumber, name, description, purchasePrice, lifespan, expectedRentalDays)
    {
        if (string.IsNullOrWhiteSpace(tentType))
            throw new ArgumentException("Tent type is required.");
        if (sleepingCapacity <= 0)
            throw new ArgumentException("SleepingCapacity is required.");
        if (weight <= 0)
            throw new ArgumentException("Weight is required.");
        
        TentType = tentType;
        SleepingCapacity = sleepingCapacity;
        Weight = weight;
        
        // Fixed deposit amount for all tents
        DepositAmount = 50.00m;
        
        // Fixed maintenance threshold
        MaintenanceThreshold = 15;
    }
    #endregion
}