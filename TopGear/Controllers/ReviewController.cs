using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TopGear.Application.DTOs.ReviewDTO;
using TopGear.Application.Interfaces;

namespace TopGear.Controllers;

[ApiController]
[Route("api/review")]
public class ReviewController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllReviews()
    {
        var reviews = await _reviewService.GetAllReviewsAsync();
        return Ok(reviews);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetReviewById(Guid id)
    {
        var review = await _reviewService.GetReviewByIdAsync(id);

        if (review == null)
        {
            return NotFound();
        }

        return Ok(review);
    }

    // get all reviews for a specific part
    [HttpGet("part/{partId:guid}")]
    public async Task<IActionResult> GetReviewsByPart(Guid partId)
    {
        var reviews = await _reviewService.GetReviewsByPartAsync(partId);
        return Ok(reviews);
    }

    // get all reviews for a specific service appointment
    [HttpGet("appointment/{appointmentId:guid}")]
    public async Task<IActionResult> GetReviewsByAppointment(Guid appointmentId)
    {
        var reviews = await _reviewService.GetReviewsByAppointmentAsync(appointmentId);
        return Ok(reviews);
    }

    // get all reviews made by a specific customer
    [Authorize]
    [HttpGet("customer/{customerId:guid}")]
    public async Task<IActionResult> GetReviewsByCustomer(Guid customerId)
    {
        var reviews = await _reviewService.GetReviewsByCustomerAsync(customerId);
        return Ok(reviews);
    }

    // authenticated user creates a review, their id is taken from the jwt token
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateReview([FromBody] CreateReviewDTO dto)
    {
        var customerId = Guid.Parse(User.FindFirstValue("sub")!);
        var created = await _reviewService.CreateReviewAsync(customerId, dto);
        return CreatedAtAction(nameof(GetReviewById), new { id = created.ReviewId }, created);
    }

    // only the owner can edit their review
    [Authorize]
    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> EditReview(Guid id, [FromBody] EditReviewDTO dto)
    {
        var requestingUserId = Guid.Parse(User.FindFirstValue("sub")!);
        var updated = await _reviewService.EditReviewAsync(id, requestingUserId, dto);

        if (updated == null)
        {
            return NotFound();
        }

        return Ok(updated);
    }

    // owner can delete their own review, admin can delete any review
    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteReview(Guid id)
    {
        var requestingUserId = Guid.Parse(User.FindFirstValue("sub")!);
        var isAdmin = User.IsInRole("Admin");

        var success = await _reviewService.DeleteReviewAsync(id, requestingUserId, isAdmin);

        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }
}
