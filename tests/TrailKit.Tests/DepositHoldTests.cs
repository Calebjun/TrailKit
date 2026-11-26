// Author: Caleb
// Purpose: Unit tests for DepositHold.

using TrailKit.Domain.Entities;
using TrailKit.Domain.Enums;

namespace TrailKit.Tests;

/// <summary>
/// Checks how a deposit is taken, released and applied to charges.
/// </summary>
[TestFixture]
public class DepositHoldTests
{
    /// <summary>A $100 deposit, created fresh before each test.</summary>
    private DepositHold _hold = null!;

    /// <summary>
    /// Creates a fresh $100 deposit before each test.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        _hold = new DepositHold(rentalId: 1, amount: 100m);
    }

    /// <summary>A new deposit starts as Held with nothing refunded yet.</summary>
    [Test]
    public void Constructor_NewDeposit_IsHeld()
    {
        Assert.That(_hold.State, Is.EqualTo(HoldState.Held));
        Assert.That(_hold.RefundAmount, Is.EqualTo(0m));
    }

    /// <summary>A deposit of zero is rejected.</summary>
    [Test]
    public void Constructor_ZeroAmount_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new DepositHold(1, 0m));
    }

    /// <summary>Releasing a deposit refunds all of it.</summary>
    [Test]
    public void Release_HeldDeposit_RefundsFullAmount()
    {
        _hold.Release();

        Assert.That(_hold.State, Is.EqualTo(HoldState.Released));
        Assert.That(_hold.RefundAmount, Is.EqualTo(100m));
    }

    /// <summary>$30 of charges on a $100 deposit leaves nothing owed and refunds $70.</summary>
    [Test]
    public void Apply_ChargesBelowDeposit_RefundsDifference()
    {
        decimal remainder = _hold.Apply(30m);

        Assert.That(remainder, Is.EqualTo(0m));
        Assert.That(_hold.AppliedAmount, Is.EqualTo(30m));
        Assert.That(_hold.RefundAmount, Is.EqualTo(70m));
    }

    /// <summary>$150 of charges on a $100 deposit uses it all and leaves $50 owed.</summary>
    [Test]
    public void Apply_ChargesAboveDeposit_ReturnsRemainder()
    {
        decimal remainder = _hold.Apply(150m);

        Assert.That(remainder, Is.EqualTo(50m));
        Assert.That(_hold.RefundAmount, Is.EqualTo(0m));
    }

    /// <summary>A deposit cannot be settled twice.</summary>
    [Test]
    public void Release_AlreadyApplied_Throws()
    {
        _hold.Apply(30m);

        Assert.Throws<InvalidOperationException>(() => _hold.Release());
    }

    /// <summary>Zero charges must use Release instead of Apply.</summary>
    [Test]
    public void Apply_ZeroCharges_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => _hold.Apply(0m));
    }
}