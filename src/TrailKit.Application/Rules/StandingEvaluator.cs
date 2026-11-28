using TrailKit.Domain.Enums;

namespace TrailKit.Application.Rules;

/// <summary>
/// Author: Utsah Tamang
/// Description: Result of a standing evaluation: the new standing and the reason (if any).
/// </summary>
public record StandingResult(CustomerStanding Standing, string? Reason);

/// <summary>
/// Author: Utsah Tamang
/// Description: Contract for the standing rule so services and tests depend on an abstraction.
/// </summary>
public interface IStandingEvaluator
{
    /// <summary>Number of days the late-return window covers.</summary>
    int LateReturnWindowDays { get; }

    /// <summary>Decides a customer's standing.</summary>
    /// <param name="unpaidTotal">Total unpaid (not paid, not waived) charges.</param>
    /// <param name="lateReturnsInWindow">Late returns inside the window.</param>
    /// <returns>The standing and reason.</returns>
    StandingResult Evaluate(decimal unpaidTotal, int lateReturnsInWindow);
}

/// <summary>
/// Author: Utsah Tamang
/// Description: Applies the proposed suspension rule: Suspended with more than $50 in unpaid charges
/// or 2 late returns in 90 days. Pure logic with no database access, which makes it easy to unit test.
/// </summary>
public class StandingEvaluator : IStandingEvaluator
{
    /// <summary>Unpaid charges above this amount suspend the customer.</summary>
    public const decimal MaxUnpaidCharges = 50m;

    /// <summary>This many late returns inside the window suspend the customer.</summary>
    public const int MaxLateReturns = 2;

    /// <summary>Length of the late-return window in days.</summary>
    public int LateReturnWindowDays => 90;

    /// <summary>Decides a customer's standing from the two inputs.</summary>
    /// <param name="unpaidTotal">Total unpaid charges.</param>
    /// <param name="lateReturnsInWindow">Late returns inside the window.</param>
    /// <returns>The standing and a reason when suspended.</returns>
    public StandingResult Evaluate(decimal unpaidTotal, int lateReturnsInWindow)
    {
        var reasons = new List<string>();

        if (unpaidTotal > MaxUnpaidCharges)
        {
            reasons.Add($"Unpaid charges of ${unpaidTotal:0.00} are over the ${MaxUnpaidCharges:0} limit.");
        }

        if (lateReturnsInWindow >= MaxLateReturns)
        {
            reasons.Add($"{lateReturnsInWindow} late returns in the last {LateReturnWindowDays} days.");
        }

        return reasons.Count == 0
            ? new StandingResult(CustomerStanding.Good, null)
            : new StandingResult(CustomerStanding.Suspended, string.Join(" ", reasons));
    }
}
