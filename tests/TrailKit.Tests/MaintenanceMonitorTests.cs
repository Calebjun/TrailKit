// Author: Caleb
// Purpose: Unit tests for MaintenanceMonitor.

using TrailKit.Application.Rules;
using TrailKit.Domain.Enums;

namespace TrailKit.Tests;

/// <summary>
/// Checks alert levels, remaining uses and dashboard sorting values.
/// </summary>
[TestFixture]
public class MaintenanceMonitorTests
{
    /// <summary>The monitor under test, created fresh before each test.</summary>
    private MaintenanceMonitor _monitor = null!;

    /// <summary>
    /// Creates a fresh monitor before each test.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        _monitor = new MaintenanceMonitor();
    }

    /// <summary>
    /// Checks the alert level across the range, using a kayak's threshold of 10.
    /// Each TestCase runs the test once with its own values.
    /// </summary>
    [TestCase(0, 10, MaintenanceAlertLevel.None)]
    [TestCase(7, 10, MaintenanceAlertLevel.None)]
    [TestCase(8, 10, MaintenanceAlertLevel.Approaching)]
    [TestCase(9, 10, MaintenanceAlertLevel.Approaching)]
    [TestCase(10, 10, MaintenanceAlertLevel.Due)]
    [TestCase(12, 10, MaintenanceAlertLevel.Due)]
    public void Check_KayakUseCounts_ReturnsExpectedLevel(int useCount, int threshold, MaintenanceAlertLevel expected)
    {
        Assert.That(_monitor.Check(useCount, threshold), Is.EqualTo(expected));
    }

    /// <summary>A bike with a threshold of 8 warns at 7 uses, since 7 is 87.5% of 8.</summary>
    [Test]
    public void Check_BikeSevenOfEight_ReturnsApproaching()
    {
        Assert.That(_monitor.Check(7, 8), Is.EqualTo(MaintenanceAlertLevel.Approaching));
    }

    /// <summary>An item at its threshold is due.</summary>
    [Test]
    public void IsDue_AtThreshold_ReturnsTrue()
    {
        Assert.That(_monitor.IsDue(15, 15), Is.True);
    }

    /// <summary>7 uses of 10 leaves 3 rentals before service.</summary>
    [Test]
    public void UsesUntilDue_SevenOfTen_ReturnsThree()
    {
        Assert.That(_monitor.UsesUntilDue(7, 10), Is.EqualTo(3));
    }

    /// <summary>An overdue item never shows negative uses left.</summary>
    [Test]
    public void UsesUntilDue_PastThreshold_ReturnsZero()
    {
        Assert.That(_monitor.UsesUntilDue(12, 10), Is.EqualTo(0));
    }

    /// <summary>12 uses of 10 is 2 past the threshold.</summary>
    [Test]
    public void UsesPastThreshold_TwelveOfTen_ReturnsTwo()
    {
        Assert.That(_monitor.UsesPastThreshold(12, 10), Is.EqualTo(2));
    }

    /// <summary>A threshold of zero is a setup error.</summary>
    [Test]
    public void Check_ZeroThreshold_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => _monitor.Check(5, 0));
    }

    /// <summary>A negative use count is impossible.</summary>
    [Test]
    public void Check_NegativeUseCount_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => _monitor.Check(-1, 10));
    }
}