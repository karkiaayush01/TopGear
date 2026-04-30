using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TopGear.Application.DTOs.PartsDTO;
using TopGear.Application.Interfaces;

namespace TopGear.Controllers;

[ApiController]
[Route("api/part")]
public class PartController : ControllerBase
{
    public readonly IPartService _partService;

    public PartController(IPartService partService)
    {
        _partService = partService;
    }

    /// <summary>
    /// Get all parts
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAllParts()
    {

        var parts = await _partService.GetPartsAsync();
        return Ok(parts);

    }
    /// <summary>
    /// Get part by id
    /// </summary>
    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPartById(Guid id)
    {
        var part = await _partService.GetPartByIdAsync(id);
        if (part == null)
        {
            return NotFound();
        }
        return Ok(part);
    }

    /// <summary>
    /// Create a new part. Only Admin can access this endpoint.
    /// </summary>
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> CreatePart([FromForm] CreatePartDTO partCreateDTO)
    {
        var createdPart = await _partService.CreatePartAsync(partCreateDTO);
        return CreatedAtAction(nameof(GetPartById), new { id = createdPart.PartId }, createdPart);
    }

    /// <summary>
    /// Edit parts
    /// </summary>
    [Authorize(Roles = "Admin")]
    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdatePart(Guid id, [FromBody] EditPartDTO partUpdateDTO)
    {
        var updatedPart = await _partService.UpdatePartAsync(id, partUpdateDTO);
        if (updatedPart == null)
        {
            throw new KeyNotFoundException();
        }
        return Ok(updatedPart);
    }

    /// <summary>
    /// Delete part by id
    /// </summary>
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]

    public async Task<IActionResult> DeletePart(Guid id)
    {
        var success = await _partService.DeletePartAsync(id);
        if (!success)
        {
            return NotFound();
        }
        return NoContent();
    }

}
