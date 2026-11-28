// Author: Caleb
// Purpose: Unit tests for RentalChargeCalculator.

using TrailKit.Application.Rules;
using TrailKit.Domain.Entities;
using TrailKit.Domain.Policies;

namespace TrailKit.Tests;

/// <summary>
/// Checks late days, late fees and cleaning fees for both customer types.
/// </summary>
[TestFixture]
public class RentalChargeCalculatorTests
{
    /// <summary>The calculator under test, created fresh before each test.</summary>
    private RentalChargeCalculator _calculator = null!;

    /// <summary>Rules for standard customers: $15 a day, no grace days, $20 cleaning.</summary>
    private IChargePolicy _standard = null!;

    /// <summary>Rules for members: $10 a day, 1 grace day, $10 cleaning.</summary>
    private IChargePolicy _member = null!;

    /// <summary>A 3-day rental picked up March 1 and due March 4.</summary>
    private Rental _rental = null!;

    /// <summary>The date the test rental is due.</summary>
    private readonly DateTime _dueDate = new DateTime(2027, 3, 4);

    /// <summary>
    /// Creates a fresh calculator, policies and rental before each test.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        _calculator = new RentalChargeCalculator();
        _standard = new StandardChargePolicy();
        _member = new MemberChargePolicy();
        _rental = new Rental(gearId: 1, customerId: 1, reservationId: null,
            pickedUpOn: new DateTime(2027, 3, 1, 10, 0, 0), dueDate: _dueDate);
    }

    /// <summary>An on-time return costs nothing.</summary>
    [Test]
    public void Calculate_ReturnedOnDueDate_ReturnsZero()
    {
        decimal total = _calculator.Calculate(_rental, _dueDate.AddHours(16), false, _standard);

        Assert.That(total, Is.EqualTo(0m));
    }

    /// <summary>An early return never produces negative late days.</summary>
    [Test]
    public void LateDays_ReturnedEarly_ReturnsZero()
    {
        int days = _calculator.LateDays(_rental, _dueDate.AddDays(-1), _standard);

        Assert.That(days, Is.EqualTo(0));
    }

    /// <summary>A standard customer 2 days late pays 2 × $15.</summary>
    [Test]
    public void LateFee_StandardTwoDaysLate_Returns30()
    {
        decimal fee = _calculator.LateFee(_rental, _dueDate.AddDays(2), _standard);

        Assert.That(fee, Is.EqualTo(30m));
    }

    /// <summary>A member 1 day late is inside the grace day and pays nothing.</summary>
    [Test]
    public void LateFee_MemberOneDayLate_ReturnsZero()
    {
        decimal fee = _calculator.LateFee(_rental, _dueDate.AddDays(1), _member);

        Assert.That(fee, Is.EqualTo(0m));
    }

    /// <summary>A member 3 days late pays for 2 days after the grace day: 2 × $10.</summary>
    [Test]
    public void LateFee_MemberThreeDaysLate_Returns20()
    {
        decimal fee = _calculator.LateFee(_rental, _dueDate.AddDays(3), _member);

        Assert.That(fee, Is.EqualTo(20m));
    }

    /// <summary>A dirty on-time return pays only the cleaning fee.</summary>
    [Test]
    public void Calculate_StandardOnTimeDirty_Returns20()
    {
        decimal total = _calculator.Calculate(_rental, _dueDate, true, _standard);

        Assert.That(total, Is.EqualTo(20m));
    }

    /// <summary>A standard customer 2 days late and dirty pays $30 late plus $20 cleaning.</summary>
    [Test]
    public void Calculate_StandardTwoDaysLateDirty_Returns50()
    {
        decimal total = _calculator.Calculate(_rental, _dueDate.AddDays(2), true, _standard);

        Assert.That(total, Is.EqualTo(50m));
    }

    /// <summary>A missing policy is a programming error and must fail loudly.</summary>
    [Test]
    public void LateDays_NullPolicy_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => _calculator.LateDays(_rental, _dueDate, null!));
    }
}