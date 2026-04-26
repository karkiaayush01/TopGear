using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TopGear.Application.DTOs.AppointmentDTO;
using TopGear.Application.Interfaces;

namespace TopGear.Controllers;

[ApiController]
[Route("api/appointment")]
public class ServiceAppointmentController : ControllerBase
{
    private readonly IServiceAppointmentService _appointmentService;

    public ServiceAppointmentController(IServiceAppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    // only admin and staff can see all appointments
    [Authorize(Roles = "Admin,Staff")]
    [HttpGet]
    public async Task<IActionResult> GetAllAppointments()
    {
        var appointments = await _appointmentService.GetAllAppointmentsAsync();
        return Ok(appointments);
    }

    [Authorize]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetAppointmentById(Guid id)
    {
        var appointment = await _appointmentService.GetAppointmentByIdAsync(id);

        if (appointment == null)
        {
            return NotFound();
        }

        return Ok(appointment);
    }

    // customers can look up all their own appointments by their user id
    [Authorize]
    [HttpGet("customer/{customerId:guid}")]
    public async Task<IActionResult> GetAppointmentsByCustomer(Guid customerId)
    {
        var appointments = await _appointmentService.GetAppointmentsByCustomerAsync(customerId);
        return Ok(appointments);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateAppointment([FromBody] CreateAppointmentDTO dto)
    {
        var created = await _appointmentService.CreateAppointmentAsync(dto);
        return CreatedAtAction(nameof(GetAppointmentById), new { id = created.AppointmentId }, created);
    }

    // only admin and staff can update appointment status or reschedule
    [Authorize(Roles = "Admin,Staff")]
    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> UpdateAppointment(Guid id, [FromBody] UpdateAppointmentDTO dto)
    {
        var updated = await _appointmentService.UpdateAppointmentAsync(id, dto);

        if (updated == null)
        {
            throw new KeyNotFoundException();
        }

        return Ok(updated);
    }

    // customers can cancel their own appointment using their user id from the jwt token
    [Authorize]
    [HttpDelete("{id:guid}/cancel")]
    public async Task<IActionResult> CancelAppointment(Guid id)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var success = await _appointmentService.CancelAppointmentAsync(id, userId);

        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAppointment(Guid id)
    {
        var success = await _appointmentService.DeleteAppointmentAsync(id);

        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }
}
