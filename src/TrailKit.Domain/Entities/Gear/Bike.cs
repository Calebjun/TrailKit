// Author: Alison
// Purpose: To represent a concrete Bike available for rent in the catalog.

namespace TrailKit.Domain.Entities.Gear;

/// <summary>
/// A single bike in the catalog.
/// </summary>
public class Bike : Gear
{
    #region Fields
    /// <summary>The type of bike. E.g. mountain, road, gravel.</summary>
    private string _bikeType;

    /// <summary>The intended age group for the bike. Can be infant, child, youth, or adult.</summary>
    private string _ageGroup;

    /// <summary>The size of the bike, between x-small to x-large.</summary>
    private string _size;
    #endregion
    
    #region Properties
    /// <summary>Gets/sets the type of bike. E.g. mountain, road, gravel</summary>
    public string BikeType { get => _bikeType; private set => _bikeType = value; }
    
    /// <summary>Gets/sets the intended age group for the bike. Can be infant, child, youth, or adult.</summary>
    public string AgeGroup { get => _ageGroup; private set => _ageGroup = value; }
    
    /// <summary>Gets/sets the size of the bike, between x-small to x-large.</summary>
    public string Size { get => _size; private set => _size = value; }
    #endregion
    
    #region Constructors
    /// <summary>
    /// Creates a new bike item.
    /// </summary>
    /// <param name="serialNumber">Internal serial number.</param>
    /// <param name="name">Name of gear item.</param>
    /// <param name="description">Optional description.</param>
    /// <param name="purchasePrice">Purchase price of gear in $CAD.</param>
    /// <param name="lifespan">Expected lifespan of gear in years.</param>
    /// <param name="expectedRentalDays">Expected number of days in a year it will be rented out.</param>
    /// <param name="bikeType">Type of bike. E.g. mountain, road, gravel.</param>
    /// <param name="ageGroup">Intended age group of bike. I.e. infant, child, youth, or adult.</param>
    /// <param name="size">Size of the bike, x-small to x-large.</param>
    public Bike(string serialNumber, string name, string? description, decimal purchasePrice, int lifespan,
        int expectedRentalDays, string bikeType, string ageGroup, string size)
        : base(serialNumber, name, description, purchasePrice, lifespan, expectedRentalDays)
    {
        if (string.IsNullOrWhiteSpace(bikeType))
            throw new ArgumentException("BikeType is required.");
        
        if  (string.IsNullOrWhiteSpace(ageGroup))
            throw new ArgumentException("AgeGroup is required.");
        
        if (string.IsNullOrWhiteSpace(size))
            throw new ArgumentException("Size is required.");
        
        BikeType = bikeType;
        AgeGroup = ageGroup;
        Size = size;
        
        // Fixed deposit amount for all bikes
        DepositAmount = 50.00m;
        
        // Fixed maintenance threshold
        MaintenanceThreshold = 8;
    }
    #endregion
}