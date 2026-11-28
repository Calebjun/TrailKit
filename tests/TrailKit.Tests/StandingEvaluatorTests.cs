using TrailKit.Application.Rules;
using TrailKit.Domain.Enums;

namespace TrailKit.Tests;

/// <summary>
/// Author: Utsah Tamang
/// Description: NUnit tests for the standing rule: suspended with more than $50 unpaid
/// or 2 late returns in 90 days.
/// </summary>
[TestFixture]
public class StandingEvaluatorTests
{
    private StandingEvaluator _evaluator = null!;

    /// <summary>Creates a fresh evaluator before each test.</summary>
    [SetUp]
    public void SetUp()
    {
        _evaluator = new StandingEvaluator();
    }

    /// <summary>No charges and no late returns keeps the customer in good standing.</summary>
    [Test]
    public void Evaluate_NoChargesNoLateReturns_IsGood()
    {
        StandingResult result = _evaluator.Evaluate(0m, 0);
        Assert.That(result.Standing, Is.EqualTo(CustomerStanding.Good));
        Assert.That(result.Reason, Is.Null);
    }

    /// <summary>Exactly $50 is not "more than $50", so the customer stays Good.</summary>
    [Test]
    public void Evaluate_ExactlyFiftyDollars_IsGood()
    {
        Assert.That(_evaluator.Evaluate(50m, 0).Standing, Is.EqualTo(CustomerStanding.Good));
    }

    /// <summary>One cent over $50 suspends the customer.</summary>
    [Test]
    public void Evaluate_OverFiftyDollars_IsSuspended()
    {
        StandingResult result = _evaluator.Evaluate(50.01m, 0);
        Assert.That(result.Standing, Is.EqualTo(CustomerStanding.Suspended));
        Assert.That(result.Reason, Does.Contain("Unpaid charges"));
    }

    /// <summary>One late return is allowed.</summary>
    [Test]
    public void Evaluate_OneLateReturn_IsGood()
    {
        Assert.That(_evaluator.Evaluate(0m, 1).Standing, Is.EqualTo(CustomerStanding.Good));
    }

    /// <summary>Two late returns suspend the customer.</summary>
    [Test]
    public void Evaluate_TwoLateReturns_IsSuspended()
    {
        StandingResult result = _evaluator.Evaluate(0m, 2);
        Assert.That(result.Standing, Is.EqualTo(CustomerStanding.Suspended));
        Assert.That(result.Reason, Does.Contain("late returns"));
    }

    /// <summary>Both problems at once mention both reasons.</summary>
    [Test]
    public void Evaluate_BothRulesBroken_ReasonMentionsBoth()
    {
        StandingResult result = _evaluator.Evaluate(75m, 3);
        Assert.That(result.Standing, Is.EqualTo(CustomerStanding.Suspended));
        Assert.That(result.Reason, Does.Contain("Unpaid").And.Contain("late returns"));
    }

    /// <summary>Paying or waiving down to $50 restores good standing.</summary>
    [Test]
    public void Evaluate_AfterPaymentBelowLimit_ReturnsToGood()
    {
        Assert.That(_evaluator.Evaluate(80m, 0).Standing, Is.EqualTo(CustomerStanding.Suspended));
        Assert.That(_evaluator.Evaluate(30m, 0).Standing, Is.EqualTo(CustomerStanding.Good));
    }
}
