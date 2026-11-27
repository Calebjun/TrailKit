using TrailKit.Application.Dtos;
using TrailKit.Application.Rules;
using TrailKit.Domain.Entities.Customers;
using TrailKit.Domain.Enums;
using TrailKit.Infrastructure.Repositories;

namespace TrailKit.Application.Services;

/// <summary>
/// Author: Utsah Tamang
/// Description: Contract for UC4 Manage Customers. Caleb's RentalService calls AddChargeAsync and
/// ReevaluateStandingAsync after a return; Layan's ReservationService calls CanReserveAsync.
/// </summary>
public interface ICustomerService
{
    /// <summary>Builds the My Account page data.</summary>
    Task<ServiceResult<AccountSummaryDto>> GetAccountSummaryAsync(int customerId);

    /// <summary>Lists customers for the manager, filtered by standing and search text.</summary>
    Task<IReadOnlyList<CustomerListItemDto>> GetCustomersAsync(CustomerStanding? standing, string? search);

    /// <summary>Gets the charges of one customer.</summary>
    Task<IReadOnlyList<ChargeDto>> GetChargesAsync(int customerId);

    /// <summary>Records payment of a charge and re-evaluates standing.</summary>
    Task<ServiceResult<CustomerListItemDto>> RecordPaymentAsync(int customerId, int chargeId);

    /// <summary>Waives a charge with a reason and re-evaluates standing.</summary>
    Task<ServiceResult<CustomerListItemDto>> WaiveChargeAsync(int customerId, int chargeId, string reason);

    /// <summary>Adds a charge (late fee, cleaning, damage) and re-evaluates standing.</summary>
    Task<ServiceResult> AddChargeAsync(int customerId, decimal amount, string reason, int? rentalId);

    /// <summary>Re-evaluates and saves standing (call after every return).</summary>
    Task<ServiceResult> ReevaluateStandingAsync(int customerId);

    /// <summary>UC1 step 4: tells the reservation flow whether the customer may reserve.</summary>
    Task<ServiceResult> CanReserveAsync(int customerId);
}

/// <summary>
/// Author: Utsah Tamang
/// Description: Business logic for UC4: account summary, payments, waivers and standing updates.
/// Depends only on abstractions (repository, evaluator, rental lookup) injected by constructor.
/// </summary>
public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IStandingEvaluator _standingEvaluator;
    private readonly IRentalQueryRepository _rentalQueries;

    /// <summary>Creates the service.</summary>
    /// <param name="customerRepository">Customer data access.</param>
    /// <param name="standingEvaluator">Suspension rule.</param>
    /// <param name="rentalQueries">Read-only rental data (Caleb's rentals) for the account page and late-return count.</param>
    public CustomerService(
        ICustomerRepository customerRepository,
        IStandingEvaluator standingEvaluator,
        IRentalQueryRepository rentalQueries)
    {
        _customerRepository = customerRepository;
        _standingEvaluator = standingEvaluator;
        _rentalQueries = rentalQueries;
    }

    /// <inheritdoc />
    public async Task<ServiceResult<AccountSummaryDto>> GetAccountSummaryAsync(int customerId)
    {
        Customer? customer = await _customerRepository.GetWithChargesAsync(customerId);
        if (customer == null)
        {
            return ServiceResult<AccountSummaryDto>.Fail("Account not found.");
        }

        IReadOnlyList<RentalRecord> rentals = await _rentalQueries.GetForCustomerAsync(customerId);

        var summary = new AccountSummaryDto
        {
            CustomerId = customer.Id,
            Name = customer.Name,
            Email = customer.Email,
            Standing = customer.Standing,
            SuspensionReason = customer.SuspensionReason,
            UnpaidTotal = customer.UnpaidChargesTotal,
            ActiveRentals = rentals.Where(r => r.ReturnedOn == null).Select(ToRentalSummary).ToList(),
            RentalHistory = rentals.Where(r => r.ReturnedOn != null).Select(ToRentalSummary).ToList(),
            Charges = customer.Charges.OrderByDescending(c => c.CreatedAtUtc).Select(ToChargeDto).ToList()
        };
        return ServiceResult<AccountSummaryDto>.Ok(summary);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<CustomerListItemDto>> GetCustomersAsync(CustomerStanding? standing, string? search)
    {
        IReadOnlyList<Customer> customers = await _customerRepository.SearchAsync(standing, search);
        return customers.Select(ToListItem).ToList();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ChargeDto>> GetChargesAsync(int customerId)
    {
        Customer? customer = await _customerRepository.GetWithChargesAsync(customerId);
        if (customer == null)
        {
            return new List<ChargeDto>();
        }
        return customer.Charges.OrderByDescending(c => c.CreatedAtUtc).Select(ToChargeDto).ToList();
    }

    /// <inheritdoc />
    public async Task<ServiceResult<CustomerListItemDto>> RecordPaymentAsync(int customerId, int chargeId)
    {
        Customer? customer = await _customerRepository.GetWithChargesAsync(customerId);
        Charge? charge = customer?.Charges.FirstOrDefault(c => c.Id == chargeId);
        if (customer == null || charge == null)
        {
            return ServiceResult<CustomerListItemDto>.Fail("Charge not found.");
        }
        if (!charge.MarkPaid())
        {
            return ServiceResult<CustomerListItemDto>.Fail("That charge is already paid or waived.");
        }

        await ApplyStandingAsync(customer);
        return ServiceResult<CustomerListItemDto>.Ok(ToListItem(customer));
    }

    /// <inheritdoc />
    public async Task<ServiceResult<CustomerListItemDto>> WaiveChargeAsync(int customerId, int chargeId, string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            return ServiceResult<CustomerListItemDto>.Fail("A reason is required to waive a charge.");
        }

        Customer? customer = await _customerRepository.GetWithChargesAsync(customerId);
        Charge? charge = customer?.Charges.FirstOrDefault(c => c.Id == chargeId);
        if (customer == null || charge == null)
        {
            return ServiceResult<CustomerListItemDto>.Fail("Charge not found.");
        }
        if (!charge.Waive(reason))
        {
            return ServiceResult<CustomerListItemDto>.Fail("That charge is already paid or waived.");
        }

        await ApplyStandingAsync(customer);
        return ServiceResult<CustomerListItemDto>.Ok(ToListItem(customer));
    }

    /// <inheritdoc />
    public async Task<ServiceResult> AddChargeAsync(int customerId, decimal amount, string reason, int? rentalId)
    {
        if (amount <= 0 || string.IsNullOrWhiteSpace(reason))
        {
            return ServiceResult.Fail("A charge needs an amount above zero and a reason.");
        }

        Customer? customer = await _customerRepository.GetWithChargesAsync(customerId);
        if (customer == null)
        {
            return ServiceResult.Fail("Customer not found.");
        }

        customer.Charges.Add(new Charge { Amount = amount, Reason = reason.Trim(), RentalId = rentalId });
        await ApplyStandingAsync(customer);
        return ServiceResult.Ok();
    }

    /// <inheritdoc />
    public async Task<ServiceResult> ReevaluateStandingAsync(int customerId)
    {
        Customer? customer = await _customerRepository.GetWithChargesAsync(customerId);
        if (customer == null)
        {
            return ServiceResult.Fail("Customer not found.");
        }
        await ApplyStandingAsync(customer);
        return ServiceResult.Ok();
    }

    /// <inheritdoc />
    public async Task<ServiceResult> CanReserveAsync(int customerId)
    {
        Customer? customer = await _customerRepository.GetByIdAsync(customerId);
        if (customer == null)
        {
            return ServiceResult.Fail("Customer not found.");
        }
        return customer.Standing == CustomerStanding.Good
            ? ServiceResult.Ok()
            : ServiceResult.Fail(customer.SuspensionReason ?? "Your account is suspended.", "Suspended");
    }

    /// <summary>Runs the standing rule, stores the result and saves the customer.</summary>
    /// <param name="customer">Customer loaded with charges.</param>
    private async Task ApplyStandingAsync(Customer customer)
    {
        DateTime since = DateTime.UtcNow.AddDays(-_standingEvaluator.LateReturnWindowDays);
        int lateReturns = await _rentalQueries.CountLateReturnsSinceAsync(customer.Id, since);

        StandingResult result = _standingEvaluator.Evaluate(customer.UnpaidChargesTotal, lateReturns);
        customer.ApplyStanding(result.Standing, result.Reason);
        await _customerRepository.UpdateAsync(customer);
    }

    /// <summary>Maps a rental record from the data tier to the DTO shown on My Account.</summary>
    /// <param name="record">The rental record.</param>
    /// <returns>The DTO.</returns>
    private static RentalSummary ToRentalSummary(RentalRecord record) => new RentalSummary
    {
        GearName = record.GearName,
        PickupDate = record.PickedUpOn,
        DueDate = record.DueDate,
        ReturnedDate = record.ReturnedOn,
        DepositAmount = record.DepositAmount,
        DepositStatus = record.DepositState
    };

    /// <summary>Maps a charge entity to its DTO.</summary>
    /// <param name="charge">The entity.</param>
    /// <returns>The DTO.</returns>
    private static ChargeDto ToChargeDto(Charge charge) => new ChargeDto
    {
        Id = charge.Id,
        Amount = charge.Amount,
        Reason = charge.Reason,
        CreatedAtUtc = charge.CreatedAtUtc,
        Status = charge.IsWaived ? "Waived" : charge.IsPaid ? "Paid" : "Unpaid",
        WaivedReason = charge.WaivedReason
    };

    /// <summary>Maps a customer entity to a table row DTO.</summary>
    /// <param name="customer">The entity (charges must be loaded).</param>
    /// <returns>The DTO.</returns>
    private static CustomerListItemDto ToListItem(Customer customer) => new CustomerListItemDto
    {
        Id = customer.Id,
        Name = customer.Name,
        Email = customer.Email,
        Standing = customer.Standing.ToString(),
        SuspensionReason = customer.SuspensionReason,
        UnpaidTotal = customer.UnpaidChargesTotal
    };
}
