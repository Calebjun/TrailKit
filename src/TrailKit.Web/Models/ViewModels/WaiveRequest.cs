using System.ComponentModel.DataAnnotations;

namespace TrailKit.Web.Models.ViewModels;

/// <summary>
/// Author: Utsah Tamang
/// Description: Body of the waive request sent by jQuery to the Web API.
/// </summary>
public class WaiveRequest
{
    /// <summary>Manager's reason for waiving the charge.</summary>
    [Required, StringLength(200)]
    public string Reason { get; set; } = string.Empty;
}
