// Author: Caleb
// Purpose: The deposit held on a rental from pickup until return.

using TrailKit.Domain.Enums;

namespace TrailKit.Domain.Entities;

/// <summary>
/// Money held at pickup. At return it is either released in full or applied to charges.
/// </summary>
public class DepositHold
{
    /// <summary>Database key, set by the database when the hold is saved.</summary>
    private int _id;

    /// <summary>The rental this deposit secures.</summary>
    private int _rentalId;

    /// <summary>The deposit amount taken at pickup.</summary>
    private decimal _amount;

    /// <summary>Where the deposit stands.</summary>
    private HoldState _state;

    /// <summary>The part of the deposit used to pay charges. Zero until the deposit is applied.</summary>
    private decimal _appliedAmount;

    /// <summary>
    /// Used only by Entity Framework Core when it loads a deposit from the database.
    /// </summary>
    private DepositHold()
    {
    }

    /// <summary>
    /// Takes a deposit when staff hand gear to a customer.
    /// </summary>
    /// <param name="rentalId">The rental this deposit secures.</param>
    /// <param name="amount">The deposit amount for the gear type.</param>
    /// <exception cref="ArgumentOutOfRangeException">The amount is zero or negative.</exception>
    public DepositHold(int rentalId, decimal amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "The deposit must be more than zero.");
        }

        _rentalId = rentalId;
        _amount = amount;
        _state = HoldState.Held;
    }

    /// <summary>Database key.</summary>
    public int Id { get => _id; private set => _id = value; }

    /// <summary>The rental this deposit secures.</summary>
    public int RentalId { get => _rentalId; private set => _rentalId = value; }

    /// <summary>The deposit amount taken at pickup.</summary>
    public decimal Amount { get => _amount; private set => _amount = value; }

    /// <summary>Where the deposit stands.</summary>
    public HoldState State { get => _state; private set => _state = value; }

    /// <summary>The part of the deposit used to pay charges.</summary>
    public decimal AppliedAmount { get => _appliedAmount; private set => _appliedAmount = value; }

    /// <summary>The amount going back to the customer once the deposit is settled.</summary>
    public decimal RefundAmount => _state == HoldState.Held ? 0m : _amount - _appliedAmount;

    /// <summary>
    /// Gives the whole deposit back. Used when the item comes back on time, clean and undamaged.
    /// </summary>
    /// <exception cref="InvalidOperationException">The deposit was already settled.</exception>
    public void Release()
    {
        EnsureHeld();
        _state = HoldState.Released;
    }

    /// <summary>
    /// Uses the deposit to pay charges owed at return.
    /// </summary>
    /// <param name="charges">The total charges owed.</param>
    /// <returns>
    /// The part of the charges the deposit could not cover. Zero when the deposit covers everything.
    /// The caller posts any remainder to the customer's account.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">The charges are zero or negative.</exception>
    /// <exception cref="InvalidOperationException">The deposit was already settled.</exception>
    public decimal Apply(decimal charges)
    {
        EnsureHeld();

        if (charges <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(charges), "Charges must be more than zero. Use Release when nothing is owed.");
        }

        _appliedAmount = Math.Min(charges, _amount);
        _state = HoldState.Applied;

        return charges - _appliedAmount;
    }

    /// <summary>
    /// Stops a deposit from being settled twice.
    /// </summary>
    /// <exception cref="InvalidOperationException">The deposit is no longer held.</exception>
    private void EnsureHeld()
    {
        if (_state != HoldState.Held)
        {
            throw new InvalidOperationException("This deposit has already been settled.");
        }
    }
}