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

    [HttpGet]
    public async Task<IActionResult> GetAllParts()
    {

        var parts = await _partService.GetPartsAsync();
        return Ok(parts);

    }

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

    [HttpPost]
    public async Task<IActionResult> CreatePart([FromBody] CreatePartDTO partCreateDTO)
    {
        var createdPart = await _partService.CreatePartAsync(partCreateDTO);
        return CreatedAtAction(nameof(GetPartById), new { id = createdPart.PartId }, createdPart);
    }

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
