using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TopGear.Application.DTOs.VendorDTO;
using TopGear.Application.Interfaces;

namespace TopGear.Controllers;

[ApiController]
[Route("api/vendor")]
public class VendorController : ControllerBase
{
    public readonly IVendorService _vendorService;

    public VendorController(IVendorService vendorService)
    {
        _vendorService = vendorService;
    }

    /// <summary>
    /// Retrieve all vendors
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAllVendors()
    {
        var vendors = await _vendorService.GetVendorAsync();
        return Ok(vendors);
    }

    /// <summary>
    /// Retrieve a vendor by ID 
    /// </summary>
    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetVendorById(Guid id)
    {
        var vendor = await _vendorService.GetVendorByIdAsync(id);

        if (vendor == null)
        {
            return NotFound();
        }

        return Ok(vendor);
    }

    /// <summary>
    /// Create a new vendor
    /// </summary>

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateVendor([FromBody] CreateVendorDTO vendorCreateDTO)
    {
        var createdVendor = await _vendorService.CreateVendorAsync(vendorCreateDTO);

        return CreatedAtAction(nameof(GetVendorById),
            new { id = createdVendor.VendorId },
            createdVendor);
    }

    /// <summary>
    /// Update vendor details
    /// </summary>
    [Authorize(Roles = "Admin")]
    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateVendor(Guid id, [FromBody] EditVendorDTO vendorUpdateDTO)
    {
        var updatedVendor = await _vendorService.UpdateVendorAsync(id, vendorUpdateDTO);

        if (updatedVendor == null)
        {
            throw new KeyNotFoundException();
        }

        return Ok(updatedVendor);
    }

    /// <summary>
    /// Delete a vendor (Admin only)
    /// </summary>

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteVendor(Guid id)
    {
        var success = await _vendorService.DeleteVendorAsync(id);

        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }
}