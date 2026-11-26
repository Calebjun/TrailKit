// Author: Alison
// Purpose: To represent an abstract gear item available for rent in the catalog

using TrailKit.Domain.Enums;
namespace TrailKit.Domain.Entities.Gear;

/// <summary>
/// A single gear item available for rent in the catalog.
/// </summary>
public abstract class Gear
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
    
    /// <summary>The cost per day to rent the gear item in Canadian dollars.</summary>
    private decimal _dailyRate;
    
    /// <summary>The initial cost to purchase the gear item (for internal use) in Canadian dollars.</summary>
    private decimal _purchasePrice;

    /// <summary>The expected lifespan of the gear item in years (for internal use).</summary>
    private int _lifespan;

    /// <summary>The expected number of days in a year this gear item will be rented out.</summary>
    private int _expectedRentalDays;
    
    /// <summary>The current status of the gear item. Defaults to "Available" when added to the catalog.</summary>
    private GearStatus _status;
    
    /// <summary>The amount of times this gear item has been rented out since its last maintenance.</summary>
    private int _useCount;

    /// <summary>The deposit amount of the gear item in Canadian dollars.</summary>
    private decimal _depositAmount;
    
    /// <summary>The number of uses a gear item can withstand before maintenance should be done.</summary>
    private int _maintenanceThreshold;
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
    
    /// <summary>Gets/sets the daily rate to rent the gear item in Canadian dollars.</summary>
    public decimal DailyRate { get => _dailyRate; private set => _dailyRate = value; }
    
    /// <summary>Gets/sets the initial purchase price of the gear item in Canadian dollars.</summary>
    public decimal PurchasePrice { get => _purchasePrice; private set => _purchasePrice = value; }
    
    /// <summary>Gets/sets the expected lifespan of the gear in years.</summary>
    public int Lifespan { get => _lifespan; private set => _lifespan = value; }
    
    /// <summary>Gets/sets the expected number of days in a year the gear item will be rented out.</summary>
    public int ExpectedRentalDays { get => _expectedRentalDays; private set => _expectedRentalDays = value; }
    
    /// <summary>Gets/sets the current status of the gear item.</summary>
    public GearStatus Status { get => _status; private set => _status = value; }
    
    /// <summary>Gets/sets the amount of times the gear item has been rented out since its last maintenance.</summary>
    public int UseCount { get => _useCount; private set => _useCount = value; }
    
    /// <summary>Gets/sets the deposit amount of the gear item.</summary>
    public decimal DepositAmount { get => _depositAmount; protected set => _depositAmount = value; }
    
    /// <summary>Gets/sets the number of uses a gear item can withstand before maintenance should be done.</summary>
    public int MaintenanceThreshold { get => _maintenanceThreshold; protected set => _maintenanceThreshold = value; }
    #endregion
    
    #region Constructors
    /// <summary>
    /// Parent constructor to create a new gear item.
    /// </summary>
    /// <param name="serialNumber">Internal serial number.</param>
    /// <param name="name">Name of gear item.</param>
    /// <param name="description">Optional description.</param>
    /// <param name="purchasePrice">Purchase price of gear in $CAD.</param>
    /// <param name="lifespan">Expected lifespan of gear in years.</param>
    /// <param name="expectedRentalDays">Expected number of days in a year it will be rented out.</param>
    protected Gear(string serialNumber, string name, string? description,
        decimal purchasePrice, int lifespan, int expectedRentalDays)
    {
        if (string.IsNullOrWhiteSpace(serialNumber))
            throw new ArgumentException("Serial number is required.");

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Gear name is required.");

        if (purchasePrice <= 0)
            throw new ArgumentException("Purchase price must be greater than 0.");

        if (lifespan <= 0)
            throw new ArgumentException("Lifespan must be greater than 0.");

        if (expectedRentalDays is < 1 or > 365)
            throw new ArgumentException("Expected rental days must be between 1 and 365.");
        
        SerialNumber = serialNumber;
        Name = name;
        Description = description;
        PurchasePrice = purchasePrice;
        Lifespan = lifespan;
        ExpectedRentalDays = expectedRentalDays;

        Status = GearStatus.Available;
        UseCount = 0;
    }
    #endregion
    
    #region Methods
    /// <summary>
    /// Calculates and sets the daily rate to charge for a gear item.
    /// Calculated as (annual depreciation / expected rental days in a year) * 1 + profit margin.
    /// </summary>
    /// <param name="profitMargin">The profit margin in decimal form.
    /// E.g., 30% profit margin = 0.30, 150% profit margin = 1.50. </param>
    public void CalculateDailyRate(decimal profitMargin)
    {
        if (profitMargin < 0)
        {
            throw new ArgumentException("Profit margin cannot be negative.");
        }
        decimal annualDepreciation = PurchasePrice / Lifespan;
        decimal baseRate = annualDepreciation / ExpectedRentalDays;
        DailyRate = Math.Round(baseRate * (1 + profitMargin), 2);
    }

    /// <summary>
    /// Changes the status of a gear item.
    /// </summary>
    /// <param name="status">The new status. Can be Available, Unavailable, UnderRepair, NeedsCleaning, or Retired.</param>
    public void UpdateStatus(GearStatus status)
    {
        if (!Enum.IsDefined(status))
            throw new ArgumentException("Invalid gear status.");
        
        Status = status;
    }
    
    /// <summary>
    /// Increments the use count of the gear item since its last maintenance by 1.
    /// </summary>
    public void IncrementUseCount()
    {
        UseCount++;
    }
    
    /// <summary>
    /// Resets the use count of the gear item to 0. To be used when maintenance is completed.
    /// </summary>
    public void ResetUseCount()
    {
        UseCount = 0;
    }

    #endregion
}