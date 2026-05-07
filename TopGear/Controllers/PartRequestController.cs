using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TopGear.Application.DTOs.PartRequestDTO;
using TopGear.Application.Interfaces;

namespace TopGear.Controllers;

[ApiController]
[Route("api/part-request")]
public class PartRequestController : ControllerBase
{
    private readonly IPartRequestService _partRequestService;

    public PartRequestController(IPartRequestService partRequestService)
    {
        _partRequestService = partRequestService;
    }

    /// <summary>
    /// Get all part requests for admin review.
    /// </summary>
    [Authorize(Roles = "Admin,Staff")]
    [HttpGet]
    public async Task<IActionResult> GetAllRequests()
    {
        var requests = await _partRequestService.GetAllRequestsAsync();
        return Ok(requests);
    }

    /// <summary>
    /// Get a part request by ID.
    /// </summary>
    [Authorize]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetRequestById(Guid id)
    {
        var request = await _partRequestService.GetRequestByIdAsync(id);

        if (request == null)
        {
            return NotFound();
        }

        return Ok(request);
    }

    /// <summary>
    /// Get the authenticated customer's part requests.
    /// </summary>
    [Authorize(Roles = "Customer")]
    [HttpGet("mine")]
    public async Task<IActionResult> GetMyRequests()
    {
        var customerId = GetAuthenticatedUserId();
        if (customerId == null)
        {
            return Unauthorized("Could not identify the authenticated customer.");
        }

        var requests = await _partRequestService.GetRequestsByCustomerAsync(customerId.Value);
        return Ok(requests);
    }

    /// <summary>
    /// Submit a request for an unavailable or non-existing part.
    /// </summary>
    [Authorize(Roles = "Customer")]
    [HttpPost]
    public async Task<IActionResult> CreateRequest([FromBody] CreatePartRequestDTO dto)
    {
        var customerId = GetAuthenticatedUserId();
        if (customerId == null)
        {
            return Unauthorized("Could not identify the authenticated customer.");
        }

        var created = await _partRequestService.CreateRequestAsync(customerId.Value, dto);
        return CreatedAtAction(nameof(GetRequestById), new { id = created.PartRequestId }, created);
    }

    /// <summary>
    /// Review a part request.
    /// </summary>
    [Authorize(Roles = "Admin")]
    [HttpPatch("{id:guid}/review")]
    public async Task<IActionResult> ReviewRequest(Guid id, [FromBody] ReviewPartRequestDTO dto)
    {
        var reviewed = await _partRequestService.ReviewRequestAsync(id, dto);

        if (reviewed == null)
        {
            throw new KeyNotFoundException();
        }

        return Ok(reviewed);
    }

    private Guid? GetAuthenticatedUserId()
    {
        var userId = User.FindFirst("sub")?.Value;
        return Guid.TryParse(userId, out var parsedUserId) ? parsedUserId : null;
    }
}
