using TrailKit.Domain.Policies;

namespace TrailKit.Domain.Entities.Customers;

/// <summary>
/// Author: Utsah Tamang
/// Description: Default customer type created at registration. Lower rental limit and the
/// standard late and cleaning fees.
/// </summary>
public class StandardCustomer : Customer
{
    /// <summary>Standard customers may hold two rentals at once.</summary>
    public override int MaxActiveRentals => 2;

    /// <summary>Returns the standard charge policy.</summary>
    /// <returns>A StandardChargePolicy.</returns>
    public override IChargePolicy GetPolicy() => new StandardChargePolicy();
}
