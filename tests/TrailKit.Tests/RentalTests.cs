// Author: Caleb
// Purpose: Unit tests for the Rental entity.

using TrailKit.Domain.Entities;
using TrailKit.Domain.Enums;

namespace TrailKit.Tests;

/// <summary>
/// Checks the rules a rental enforces on its own dates and status.
/// </summary>
[TestFixture]
public class RentalTests
{
    /// <summary>A rental picked up March 1 and due March 4, created fresh before each test.</summary>
    private Rental _rental = null!;

    /// <summary>The date the test rental is due.</summary>
    private readonly DateTime _dueDate = new DateTime(2027, 3, 4);

    /// <summary>
    /// Creates a fresh rental before each test.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        _rental = new Rental(1, 1, null, new DateTime(2027, 3, 1, 10, 0, 0), _dueDate);
    }

    /// <summary>A due date before the pickup date is rejected.</summary>
    [Test]
    public void Constructor_DueBeforePickup_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
            new Rental(1, 1, null, new DateTime(2027, 3, 4), new DateTime(2027, 3, 1)));
    }

    /// <summary>An unreturned item past its due date is overdue.</summary>
    [Test]
    public void IsOverdue_PastDueAndNotReturned_ReturnsTrue()
    {
        Assert.That(_rental.IsOverdue(_dueDate.AddDays(1)), Is.True);
    }

    /// <summary>An item is not overdue on its due date.</summary>
    [Test]
    public void IsOverdue_OnDueDate_ReturnsFalse()
    {
        Assert.That(_rental.IsOverdue(_dueDate), Is.False);
    }

    /// <summary>A returned item is never overdue, even if checked after the due date.</summary>
    [Test]
    public void IsOverdue_AfterReturn_ReturnsFalse()
    {
        _rental.Close(_dueDate.AddDays(2), Condition.Good, false);

        Assert.That(_rental.IsOverdue(_dueDate.AddDays(5)), Is.False);
    }

    /// <summary>Closing records the return date, condition and cleaning flag.</summary>
    [Test]
    public void Close_ActiveRental_RecordsReturn()
    {
        _rental.Close(_dueDate, Condition.Worn, true);

        Assert.That(_rental.IsReturned, Is.True);
        Assert.That(_rental.Condition, Is.EqualTo(Condition.Worn));
        Assert.That(_rental.NeedsCleaning, Is.True);
    }

    /// <summary>A rental cannot be closed twice.</summary>
    [Test]
    public void Close_AlreadyClosed_Throws()
    {
        _rental.Close(_dueDate, Condition.Good, false);

        Assert.Throws<InvalidOperationException>(() => _rental.Close(_dueDate, Condition.Good, false));
    }
}