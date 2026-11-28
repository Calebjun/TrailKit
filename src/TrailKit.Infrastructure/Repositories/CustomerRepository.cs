using Microsoft.EntityFrameworkCore;
using TrailKit.Domain.Entities.Customers;
using TrailKit.Domain.Enums;
using TrailKit.Infrastructure.Data;

namespace TrailKit.Infrastructure.Repositories;

/// <summary>
/// Author: Utsah Tamang
/// Description: Contract for the customer queries UC4 and UC8 need.
/// </summary>
public interface ICustomerRepository
{
    /// <summary>Gets one customer by key, without charges.</summary>
    Task<Customer?> GetByIdAsync(int id);

    /// <summary>Finds a customer by email (case-insensitive).</summary>
    Task<Customer?> GetByEmailAsync(string email);

    /// <summary>Checks whether an email is already registered.</summary>
    Task<bool> EmailExistsAsync(string email);

    /// <summary>Loads a customer together with all charges.</summary>
    Task<Customer?> GetWithChargesAsync(int id);

    /// <summary>Lists customers, optionally filtered by standing and a name/email search.</summary>
    Task<IReadOnlyList<Customer>> SearchAsync(CustomerStanding? standing, string? search);

    /// <summary>Counts how many accounts hold a role (used to seed the first manager).</summary>
    Task<int> CountByRoleAsync(UserRole role);

    /// <summary>Adds a new customer and saves.</summary>
    Task AddAsync(Customer customer);

    /// <summary>Saves changes to a customer (and any new or changed charges).</summary>
    Task UpdateAsync(Customer customer);
}

/// <summary>
/// Author: Utsah Tamang
/// Description: EF Core repository for customers and their charges.
/// </summary>
public class CustomerRepository : ICustomerRepository
{
    private readonly TrailKitDbContext _context;

    /// <summary>Creates the repository.</summary>
    /// <param name="context">The shared EF Core context.</param>
    public CustomerRepository(TrailKitDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<Customer?> GetByIdAsync(int id)
    {
        return await _context.Customers.FindAsync(id);
    }

    /// <inheritdoc />
    public async Task<Customer?> GetByEmailAsync(string email)
    {
        string normalized = email.Trim().ToLowerInvariant();
        return await _context.Customers.FirstOrDefaultAsync(c => c.Email == normalized);
    }

    /// <inheritdoc />
    public async Task<bool> EmailExistsAsync(string email)
    {
        string normalized = email.Trim().ToLowerInvariant();
        return await _context.Customers.AnyAsync(c => c.Email == normalized);
    }

    /// <inheritdoc />
    public async Task<Customer?> GetWithChargesAsync(int id)
    {
        return await _context.Customers.Include(c => c.Charges).FirstOrDefaultAsync(c => c.Id == id);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Customer>> SearchAsync(CustomerStanding? standing, string? search)
    {
        IQueryable<Customer> query = _context.Customers
            .Include(c => c.Charges)
            .Where(c => c.Role == UserRole.Customer);

        if (standing.HasValue)
        {
            query = query.Where(c => c.Standing == standing.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            string term = search.Trim().ToLower();
            query = query.Where(c => c.Name.ToLower().Contains(term) || c.Email.Contains(term));
        }

        return await query.OrderBy(c => c.Name).ToListAsync();
    }

    /// <inheritdoc />
    public async Task<int> CountByRoleAsync(UserRole role)
    {
        return await _context.Customers.CountAsync(c => c.Role == role);
    }

    /// <inheritdoc />
    public async Task AddAsync(Customer customer)
    {
        await _context.Customers.AddAsync(customer);
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task UpdateAsync(Customer customer)
    {
        // Customer was loaded by this context, so EF already tracks it and its charges.
        await _context.SaveChangesAsync();
    }
}
