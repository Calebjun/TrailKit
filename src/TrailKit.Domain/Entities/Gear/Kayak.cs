// Author: Alison
// Purpose: To represent a concrete Kayak available for rent in the catalog.

namespace TrailKit.Domain.Entities;

/// <summary>
/// A single kayak in the catalog.
/// </summary>
public class Kayak : Gear
{
    #region Fields
    /// <summary>The type of kayak. For example, recreational or whitewater.</summary>
    private string _kayakType;
    
    /// <summary>The seating style of the kayak. Typically, sit-inside or sit-on-top.</summary>
    private string _seatingStyle;
    
    /// <summary>The number of seats available in the kayak.</summary>
    private int _numSeats;

    /// <summary>The maximum weight capacity allowed for the kayak, in pounds (lbs).</summary>
    private int _weightCapacity;
    
    /// <summary>The length of the kayak, in feet.</summary>
    private decimal _length;
    #endregion
    
    #region Properties
    /// <summary>Gets/sets the type of kayak.</summary>
    public string KayakType { get => _kayakType; private set => _kayakType = value; }
    
    /// <summary>Gets/sets the seating style of kayak.</summary>
    public string SeatingStyle { get => _seatingStyle; private set => _seatingStyle = value; }
    
    /// <summary>Gets/sets the number of seats available in the kayak.</summary>
    public int NumSeats { get => _numSeats; private set => _numSeats = value; }
    
    /// <summary>Gets/sets the kayak's weight capacity in pounds.</summary>
    public int WeightCapacity { get => _weightCapacity; private set => _weightCapacity = value; }
    
    /// <summary>Gets/sets the kayak's length in feet.</summary>
    public decimal Length { get => _length; private set => _length = value; }
    #endregion
    
    #region Constructors
    /// <summary>
    /// Creates a new Kayak item.
    /// </summary>
    /// <param name="serialNumber">Internal serial number.</param>
    /// <param name="name">Name of gear item.</param>
    /// <param name="description">Optional description.</param>
    /// <param name="purchasePrice">Purchase price of gear in $CAD.</param>
    /// <param name="lifespan">Expected lifespan of gear in years.</param>
    /// <param name="expectedRentalDays">Expected number of days in a year it will be rented out.</param>
    /// <param name="kayakType">Type of kayak, such as whitewater.</param>
    /// <param name="seatingStyle">Seating style of kayak, such as sit-inside.</param>
    /// <param name="numSeats">Number of seats in kayak.</param>
    /// <param name="weightCapacity">Weight limit of kayak in pounds.</param>
    /// <param name="length">Length of kayak in feet.</param>
    public Kayak(string serialNumber, string name, string? description, decimal purchasePrice, int lifespan, 
        int expectedRentalDays, string kayakType, string seatingStyle, int numSeats, int weightCapacity, decimal length)
        : base(serialNumber, name, description, purchasePrice, lifespan, expectedRentalDays)
    {
        if (string.IsNullOrWhiteSpace(kayakType))
            throw new ArgumentException("Kayak type is required.");
        
        if (string.IsNullOrWhiteSpace(seatingStyle))
            throw new ArgumentException("Seating style is required.");

        if (numSeats < 1)
            throw new ArgumentException("Number of seats must at least 1.");

        if (weightCapacity <= 0)
            throw new ArgumentException("Weight capacity is required and must be greater than zero.");

        if (length <= 0)
            throw new ArgumentException("Length must be greater than zero.");
        
        KayakType = kayakType;
        SeatingStyle = seatingStyle;
        NumSeats = numSeats;
        WeightCapacity = weightCapacity;
        Length = length;
        
        // Fixed deposit amount for all Kayaks
        DepositAmount = 70.00m;
        
        // Fixed maintenance threshold
        MaintenanceThreshold = 10;
    }
    #endregion
}