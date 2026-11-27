namespace TrailKit.Infrastructure.Repositories;

/// <summary>
/// Author: Utsah Tamang
/// Description: One rental as needed by the My Account page. A flat read-only record so the Application
/// layer does not depend on the Rental and Gear entities.
/// </summary>
public class RentalRecord
{
    /// <summary>Name of the rented gear item.</summary>
    public string GearName { get; set; } = string.Empty;

    /// <summary>When the item was picked up.</summary>
    public DateTime PickedUpOn { get; set; }

    /// <summary>The date the item is due back.</summary>
    public DateTime DueDate { get; set; }

    /// <summary>When the item came back; null while the rental is active.</summary>
    public DateTime? ReturnedOn { get; set; }

    /// <summary>Deposit amount taken at pickup.</summary>
    public decimal DepositAmount { get; set; }

    /// <summary>Held, Released or Applied.</summary>
    public string DepositState { get; set; } = string.Empty;
}

/// <summary>
/// Author: Utsah Tamang
/// Description: Read-only rental queries used by UC4 (account page and the late-return count for standing).
/// Caleb or Utsah replaces the placeholder once Rental, DepositHold and Gear are mapped in TrailKitDbContext.
/// </summary>
public interface IRentalQueryRepository
{
    /// <summary>Gets all rentals of a customer, newest first.</summary>
    /// <param name="customerId">The customer id.</param>
    /// <returns>Rental records for the account page.</returns>
    Task<IReadOnlyList<RentalRecord>> GetForCustomerAsync(int customerId);

    /// <summary>
    /// Counts rentals returned after their due date since a given date
    /// (ReturnedOn.Date is later than DueDate.Date).
    /// </summary>
    /// <param name="customerId">The customer id.</param>
    /// <param name="sinceUtc">Start of the window (for example 90 days ago).</param>
    /// <returns>Number of late returns.</returns>
    Task<int> CountLateReturnsSinceAsync(int customerId, DateTime sinceUtc);
}

/// <summary>
/// Author: Utsah Tamang
/// Description: TEMPORARY. Returns no rentals and no late returns so the app runs before the rental
/// tables are mapped. Replace with a real EF Core implementation and change the registration in Program.cs.
/// </summary>
public class PlaceholderRentalQueryRepository : IRentalQueryRepository
{
    /// <inheritdoc />
    public Task<IReadOnlyList<RentalRecord>> GetForCustomerAsync(int customerId) =>
        Task.FromResult<IReadOnlyList<RentalRecord>>(new List<RentalRecord>());

    /// <inheritdoc />
    public Task<int> CountLateReturnsSinceAsync(int customerId, DateTime sinceUtc) =>
        Task.FromResult(0);
}
