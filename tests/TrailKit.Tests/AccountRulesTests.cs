using TrailKit.Application.Services;

namespace TrailKit.Tests;

/// <summary>
/// Author: Utsah Tamang
/// Description: NUnit tests for the UC8 registration rules: password strength, age and email format.
/// </summary>
[TestFixture]
public class AccountRulesTests
{
    /// <summary>Passwords need 8+ characters and a digit.</summary>
    [TestCase("abcdefg1", true)]
    [TestCase("abcdefgh", false)]
    [TestCase("abc1", false)]
    [TestCase("", false)]
    public void IsStrongPassword_ChecksLengthAndDigit(string password, bool expected)
    {
        Assert.That(AccountService.IsStrongPassword(password), Is.EqualTo(expected));
    }

    /// <summary>Turning 18 today is allowed; turning 18 tomorrow is not.</summary>
    [Test]
    public void IsOldEnough_BoundaryOnEighteenthBirthday()
    {
        var today = new DateTime(2026, 11, 27);
        Assert.That(AccountService.IsOldEnough(new DateTime(2008, 11, 27), today), Is.True);
        Assert.That(AccountService.IsOldEnough(new DateTime(2008, 11, 28), today), Is.False);
    }

    /// <summary>Email must look like an address.</summary>
    [TestCase("a@b.com", true)]
    [TestCase("not-an-email", false)]
    [TestCase("", false)]
    public void IsValidEmail_ChecksFormat(string email, bool expected)
    {
        Assert.That(AccountService.IsValidEmail(email), Is.EqualTo(expected));
    }
}
