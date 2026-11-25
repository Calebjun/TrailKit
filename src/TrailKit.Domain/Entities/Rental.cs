// Author: Caleb
// Purpose: One rental of one gear item, from pickup at the counter to its return.

using TrailKit.Domain.Enums;

namespace TrailKit.Domain.Entities;

/// <summary>
/// A gear item handed to a customer at the counter. Created at pickup and closed at return.
/// </summary>
public class Rental
{
    /// <summary>Database key, set by SQL Server when the rental is saved.</summary>
    private int _id;

    /// <summary>The gear item being rented.</summary>
    private int _gearId;

    /// <summary>The customer renting the item.</summary>
    private int _customerId;

    /// <summary>The reservation this rental fulfills. Null for walk-in customers.</summary>
    private int? _reservationId;

    /// <summary>The date and time the customer picked up the item.</summary>
    private DateTime _pickedUpOn;

    /// <summary>The date the item must be back.</summary>
    private DateTime _dueDate;

    /// <summary>The date and time the item came back. Null while the rental is active.</summary>
    private DateTime? _returnedOn;

    /// <summary>The condition recorded at return. Null while the rental is active.</summary>
    private Condition? _condition;

    /// <summary>True when staff marked the item as dirty at return.</summary>
    private bool _needsCleaning;

    /// <summary>
    /// Used only by Entity Framework Core when it loads a rental from the database.
    /// </summary>
    private Rental()
    {
    }

    /// <summary>
    /// Starts a new rental when staff hand gear to a customer.
    /// </summary>
    /// <param name="gearId">The gear item being rented.</param>
    /// <param name="customerId">The customer renting it.</param>
    /// <param name="reservationId">The reservation being fulfilled, or null for a walk-in.</param>
    /// <param name="pickedUpOn">When the customer picked the item up.</param>
    /// <param name="dueDate">The date the item must be returned.</param>
    /// <exception cref="ArgumentException">The due date is not after the pickup date.</exception>
    public Rental(int gearId, int customerId, int? reservationId, DateTime pickedUpOn, DateTime dueDate)
    {
        if (dueDate.Date <= pickedUpOn.Date)
        {
            throw new ArgumentException("The due date must be after the pickup date.", nameof(dueDate));
        }

        _gearId = gearId;
        _customerId = customerId;
        _reservationId = reservationId;
        _pickedUpOn = pickedUpOn;
        _dueDate = dueDate.Date;
    }

    /// <summary>Database key.</summary>
    public int Id { get => _id; private set => _id = value; }

    /// <summary>The gear item being rented.</summary>
    public int GearId { get => _gearId; private set => _gearId = value; }

    /// <summary>The customer renting the item.</summary>
    public int CustomerId { get => _customerId; private set => _customerId = value; }

    /// <summary>The reservation this rental fulfills, or null for a walk-in.</summary>
    public int? ReservationId { get => _reservationId; private set => _reservationId = value; }

    /// <summary>When the customer picked up the item.</summary>
    public DateTime PickedUpOn { get => _pickedUpOn; private set => _pickedUpOn = value; }

    /// <summary>The date the item must be back.</summary>
    public DateTime DueDate { get => _dueDate; private set => _dueDate = value; }

    /// <summary>When the item came back, or null while the rental is active.</summary>
    public DateTime? ReturnedOn { get => _returnedOn; private set => _returnedOn = value; }

    /// <summary>The condition recorded at return, or null while the rental is active.</summary>
    public Condition? Condition { get => _condition; private set => _condition = value; }

    /// <summary>True when staff marked the item as dirty at return.</summary>
    public bool NeedsCleaning { get => _needsCleaning; private set => _needsCleaning = value; }

    /// <summary>True once the item has been returned.</summary>
    public bool IsReturned => _returnedOn.HasValue;

    /// <summary>
    /// Checks whether the item is still out past its due date.
    /// </summary>
    /// <param name="today">The date to check against. Passed in so tests can control it.</param>
    /// <returns>True when the item is not returned and the due date has passed.</returns>
    public bool IsOverdue(DateTime today)
    {
        return !IsReturned && today.Date > _dueDate;
    }

    /// <summary>
    /// Closes the rental when the customer brings the item back.
    /// </summary>
    /// <param name="returnedOn">When the item came back.</param>
    /// <param name="condition">The condition staff recorded.</param>
    /// <param name="needsCleaning">True when the item came back dirty.</param>
    /// <exception cref="InvalidOperationException">The rental is already closed.</exception>
    /// <exception cref="ArgumentException">The return date is before the pickup date.</exception>
    public void Close(DateTime returnedOn, Condition condition, bool needsCleaning)
    {
        if (IsReturned)
        {
            throw new InvalidOperationException("This rental is already closed.");
        }

        if (returnedOn < _pickedUpOn)
        {
            throw new ArgumentException("The return date cannot be before the pickup date.", nameof(returnedOn));
        }

        _returnedOn = returnedOn;
        _condition = condition;
        _needsCleaning = needsCleaning;
    }
}
