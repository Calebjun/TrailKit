// Author: Alison
// Purpose: To represent an abstract gear item available for rent in the catalog

using TrailKit.Domain.Enums;
namespace TrailKit.Domain.Entities.Gear;

/// <summary>
/// A single gear item available for rent in the catalog.
/// </summary>
abstract class Gear
{
    #region Fields
    /// <summary>The database key set by SQL.</summary>
    private int _id;
    
    /// <summary>Internal serial number set by manufacturer.</summary>
    private string _serialNumber;
    
    /// <summary>The name of the gear item in the catalog.</summary>
    private string _name;
    
    /// <summary>Optional description of the gear item.</summary>
    private string? _description;
    
    /// <summary>Maximum amount of people that can use the gear at a time.</summary>
    private int _capacity;
    
    /// <summary>The cost per day to rent the gear item in Canadian dollars.</summary>
    private decimal _dailyRate;
    
    /// <summary>The initial cost to purchase the gear item (for internal use) in Canadian dollars.</summary>
    private decimal _purchasePrice;
    
    /// <summary>The current status of the gear item. Defaults to "Available" when added to the catalog.</summary>
    private GearStatus _status;
    
    /// <summary>The amount of times this gear item has been rented out.</summary>
    private int _useCount;
    #endregion
    
    #region Properties
    /// <summary>Gets/sets the database key.</summary>
    public int Id { get  => _id; private set => _id = value; }
    
    /// <summary>Gets/sets the internal serial number.</summary>
    public string SerialNumber { get => _serialNumber; private set => _serialNumber = value; }
    
    /// <summary>Gets/sets the name of the gear item in the catalog.</summary>
    public string Name { get => _name; private set => _name = value; }
    
    /// <summary>Gets/sets the optional description of the gear item.</summary>
    public string? Description { get => _description; private set => _description = value; }
    
    /// <summary>Gets/sets the maximum amount of people that can use the gear at a time.</summary>
    public int Capacity { get => _capacity; private set => _capacity = value; }
    
    /// <summary>Gets/sets the daily rate to rent the gear item in Canadian dollars.</summary>
    public decimal DailyRate { get => _dailyRate; private set => _dailyRate = value; }
    
    /// <summary>Gets/sets the initial purchase price of the gear item in Canadian dollars.</summary>
    public decimal PurchasePrice { get => _purchasePrice; private set => _purchasePrice = value; }
    
    /// <summary>Gets/sets the current status of the gear item.</summary>
    public GearStatus Status { get => _status; private set => _status = value; }
    
    /// <summary>Gets/sets the amount of times the gear item has been rented out.</summary>
    public int UseCount { get => _useCount; private set => _useCount = value; }
    #endregion
}