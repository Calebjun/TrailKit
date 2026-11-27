using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrailKit.Application.Dtos;
using TrailKit.Application.Services;
using TrailKit.Domain.Enums;
using TrailKit.Web.Models.ViewModels;

namespace TrailKit.Web.Controllers.Api;

/// <summary>
/// Author: Utsah Tamang
/// Description: Web API that feeds JSON to the Customer Management view through jQuery AJAX.
/// Only managers may call it.
/// </summary>
[ApiController]
[Route("api/customers")]
[Authorize(Roles = nameof(UserRole.Manager))]
public class CustomersApiController : ControllerBase
{
    private readonly ICustomerService _customerService;

    /// <summary>Creates the controller.</summary>
    /// <param name="customerService">UC4 service.</param>
    public CustomersApiController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    /// <summary>Lists customers filtered by standing and search text.</summary>
    /// <param name="standing">Good or Suspended; omit for all.</param>
    /// <param name="search">Part of a name or email.</param>
    /// <returns>JSON list of customers.</returns>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CustomerListItemDto>>> Get(
        [FromQuery] CustomerStanding? standing, [FromQuery] string? search)
    {
        return Ok(await _customerService.GetCustomersAsync(standing, search));
    }

    /// <summary>Gets the charges of one customer.</summary>
    /// <param name="id">Customer id.</param>
    /// <returns>JSON list of charges.</returns>
    [HttpGet("{id:int}/charges")]
    public async Task<ActionResult<IReadOnlyList<ChargeDto>>> GetCharges(int id)
    {
        return Ok(await _customerService.GetChargesAsync(id));
    }

    /// <summary>Records payment of a charge.</summary>
    /// <param name="id">Customer id.</param>
    /// <param name="chargeId">Charge id.</param>
    /// <returns>The updated customer row (standing may have changed).</returns>
    [HttpPost("{id:int}/charges/{chargeId:int}/pay")]
    public async Task<IActionResult> Pay(int id, int chargeId)
    {
        ServiceResult<CustomerListItemDto> result = await _customerService.RecordPaymentAsync(id, chargeId);
        return result.Success ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    /// <summary>Waives a charge with a reason.</summary>
    /// <param name="id">Customer id.</param>
    /// <param name="chargeId">Charge id.</param>
    /// <param name="request">Body holding the reason.</param>
    /// <returns>The updated customer row.</returns>
    [HttpPost("{id:int}/charges/{chargeId:int}/waive")]
    public async Task<IActionResult> Waive(int id, int chargeId, [FromBody] WaiveRequest request)
    {
        ServiceResult<CustomerListItemDto> result =
            await _customerService.WaiveChargeAsync(id, chargeId, request.Reason);
        return result.Success ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }
}
