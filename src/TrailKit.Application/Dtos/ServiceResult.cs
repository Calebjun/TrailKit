namespace TrailKit.Application.Dtos;

/// <summary>
/// Author: Utsah Tamang
/// Description: Simple success/failure result so services do not throw for expected business errors.
/// </summary>
public class ServiceResult
{
    /// <summary>True when the operation worked.</summary>
    public bool Success { get; protected set; }
    /// <summary>Message to show the user when it failed.</summary>
    public string? Error { get; protected set; }
    /// <summary>Optional machine-readable code (for example EmailTaken or Underage).</summary>
    public string? Code { get; protected set; }

    /// <summary>Creates a success result.</summary>
    /// <returns>A successful result.</returns>
    public static ServiceResult Ok() => new ServiceResult { Success = true };

    /// <summary>Creates a failure result.</summary>
    /// <param name="error">Message for the user.</param>
    /// <param name="code">Optional machine-readable code.</param>
    /// <returns>A failed result.</returns>
    public static ServiceResult Fail(string error, string? code = null) =>
        new ServiceResult { Success = false, Error = error, Code = code };
}

/// <summary>
/// Author: Utsah Tamang
/// Description: Result that also carries a value, such as the newly created customer.
/// </summary>
/// <typeparam name="T">Type of the value.</typeparam>
public class ServiceResult<T> : ServiceResult
{
    /// <summary>Value produced on success.</summary>
    public T? Value { get; private set; }

    /// <summary>Creates a success result with a value.</summary>
    /// <param name="value">The value to return.</param>
    /// <returns>A successful result.</returns>
    public static ServiceResult<T> Ok(T value) =>
        new ServiceResult<T> { Success = true, Value = value };

    /// <summary>Creates a failure result.</summary>
    /// <param name="error">Message for the user.</param>
    /// <param name="code">Optional machine-readable code.</param>
    /// <returns>A failed result.</returns>
    public static new ServiceResult<T> Fail(string error, string? code = null) =>
        new ServiceResult<T> { Success = false, Error = error, Code = code };
}
