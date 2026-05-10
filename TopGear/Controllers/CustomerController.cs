using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TopGear.Application.DTOs.CustomerDTO;
using TopGear.Application.DTOs.VehicleDTO;
using TopGear.Application.Interfaces;

namespace TopGear.Controllers
{
    [ApiController]
    [Route("api/customer")]
    public class CustomerController(ICustomerService customerService, IVehicleService vehicleService) : ControllerBase
    {
        [Authorize(Roles = "Admin,Staff")]
        [HttpPost("register")]
        public async Task<IActionResult> Create(CreateCustomerRequest request)
        {
            var result = await customerService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [Authorize(Roles = "Admin,Staff")]
        [HttpGet("{id:guid}")]
        public async Task<ActionResult> GetById(Guid id)
        {
            var result = await customerService.GetByIdAsync(id);
            return Ok(result);
        }

        [Authorize(Roles = "Admin,Staff")]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, UpdateCustomerRequest request)
        {
            await customerService.UpdateAsync(id, request);
            return NoContent();
        }

        [Authorize(Roles = "Admin,Staff")]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Deactivate(Guid id)
        {
            await customerService.DeactivateAsync(id);
            return NoContent();
        }

        /// <summary>
        /// Search and filter customers by name, phone, ID, or vehicle plate number with pagination.
        /// </summary>
        [Authorize(Roles = "Admin,Staff")]
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] CustomerSearchParams parameters)
        {
            var result = await customerService.SearchAsync(parameters);
            return Ok(result);
        }

        // ─── Vehicle endpoints ───────────────────────────────────────────────

        /// <summary>
        /// Get all vehicles for a customer. Staff can view any customer; a customer can only view their own.
        /// </summary>
        [Authorize(Roles = "Admin,Staff,Customer")]
        [HttpGet("{customerId:guid}/vehicles")]
        public async Task<IActionResult> GetVehicles(Guid customerId)
        {
            if (!CanAccessCustomerData(customerId))
                return Forbid();

            var vehicles = await vehicleService.GetCustomerVehiclesAsync(customerId);
            return Ok(vehicles);
        }

        /// <summary>
        /// Add a vehicle for a customer. Staff can add for any customer; a customer can only add for themselves.
        /// </summary>
        [Authorize(Roles = "Admin,Staff,Customer")]
        [HttpPost("{customerId:guid}/vehicles")]
        public async Task<IActionResult> AddVehicle(Guid customerId, CreateVehicleDTO dto)
        {
            if (!CanAccessCustomerData(customerId))
                return Forbid();

            var vehicle = await vehicleService.AddVehicleAsync(customerId, dto);
            return CreatedAtAction(nameof(GetVehicles), new { customerId }, vehicle);
        }

        /// <summary>
        /// Update a vehicle. Staff can update any customer's vehicle; a customer can only update their own.
        /// </summary>
        [Authorize(Roles = "Admin,Staff,Customer")]
        [HttpPatch("{customerId:guid}/vehicles/{vehicleId:guid}")]
        public async Task<IActionResult> UpdateVehicle(Guid customerId, Guid vehicleId, UpdateVehicleDTO dto)
        {
            if (!CanAccessCustomerData(customerId))
                return Forbid();

            var vehicle = await vehicleService.UpdateVehicleAsync(customerId, vehicleId, dto);
            return Ok(vehicle);
        }

        /// <summary>
        /// Delete a vehicle. Staff can delete any customer's vehicle; a customer can only delete their own.
        /// </summary>
        [Authorize(Roles = "Admin,Staff,Customer")]
        [HttpDelete("{customerId:guid}/vehicles/{vehicleId:guid}")]
        public async Task<IActionResult> DeleteVehicle(Guid customerId, Guid vehicleId)
        {
            if (!CanAccessCustomerData(customerId))
                return Forbid();

            await vehicleService.DeleteVehicleAsync(customerId, vehicleId);
            return NoContent();
        }

        // Returns true if the caller is Staff/Admin, or if the caller's own ID matches customerId.
        private bool CanAccessCustomerData(Guid customerId)
        {
            var roles = User.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToHashSet();

            if (roles.Contains("Admin") || roles.Contains("Staff"))
                return true;

            var userId = User.FindFirstValue("sub");
            return userId != null && Guid.TryParse(userId, out var callerId) && callerId == customerId;
        }
    }
}
