using TrailKit.Domain.Policies;

namespace TrailKit.Domain.Entities.Customers;

/// <summary>
/// Author: Utsah Tamang
/// Description: Customer type with a higher rental limit and friendlier fees.
/// </summary>
public class MemberCustomer : Customer
{
    /// <summary>Members may hold four rentals at once.</summary>
    public override int MaxActiveRentals => 4;

    /// <summary>Returns the member charge policy.</summary>
    /// <returns>A MemberChargePolicy.</returns>
    public override IChargePolicy GetPolicy() => new MemberChargePolicy();
}
