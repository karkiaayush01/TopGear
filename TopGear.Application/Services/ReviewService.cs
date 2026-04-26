using Microsoft.Extensions.Logging;
using TopGear.Application.DTOs.ReviewDTO;
using TopGear.Application.Interfaces;
using TopGear.Domain.Entities;
using TopGear.Domain.Enums;

namespace TopGear.Application.Services;

public class ReviewService : IReviewService
{
    private readonly IReviewRepository _repository;
    private readonly ILogger<ReviewService> _logger;

    public ReviewService(IReviewRepository repository, ILogger<ReviewService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<IEnumerable<ReviewDTO>> GetAllReviewsAsync()
    {
        _logger.LogInformation("Fetching all reviews");

        var reviews = await _repository.GetAllWithCustomerAsync();

        return reviews.Select(r => MapToDTO(r));
    }

    public async Task<ReviewDTO?> GetReviewByIdAsync(Guid id)
    {
        _logger.LogInformation("Fetching review with ID: {ReviewId}", id);

        var review = await _repository.GetByIdWithCustomerAsync(id);

        if (review == null)
        {
            _logger.LogWarning("Review not found with ID: {ReviewId}", id);
            return null;
        }

        return MapToDTO(review);
    }

    public async Task<IEnumerable<ReviewDTO>> GetReviewsByPartAsync(Guid partId)
    {
        _logger.LogInformation("Fetching reviews for part: {PartId}", partId);

        var reviews = await _repository.GetByPartIdAsync(partId);

        return reviews.Select(r => MapToDTO(r));
    }

    public async Task<IEnumerable<ReviewDTO>> GetReviewsByAppointmentAsync(Guid appointmentId)
    {
        _logger.LogInformation("Fetching reviews for appointment: {AppointmentId}", appointmentId);

        var reviews = await _repository.GetByAppointmentIdAsync(appointmentId);

        return reviews.Select(r => MapToDTO(r));
    }

    public async Task<IEnumerable<ReviewDTO>> GetReviewsByCustomerAsync(Guid customerId)
    {
        _logger.LogInformation("Fetching reviews by customer: {CustomerId}", customerId);

        var reviews = await _repository.GetByCustomerIdAsync(customerId);

        return reviews.Select(r => MapToDTO(r));
    }

    public async Task<ReviewDTO> CreateReviewAsync(Guid customerId, CreateReviewDTO dto)
    {
        _logger.LogInformation("Customer {CustomerId} creating a {ReviewType} review", customerId, dto.ReviewType);

        if (dto.ReviewType == ReviewType.Part && dto.PartId == null)
            throw new ArgumentException("PartId is required for a Part review.");

        if (dto.ReviewType == ReviewType.Service && dto.AppointmentId == null)
            throw new ArgumentException("AppointmentId is required for a Service review.");

        var review = new Review
        {
            CustomerId = customerId,
            ReviewType = dto.ReviewType,
            PartId = dto.PartId,
            AppointmentId = dto.AppointmentId,
            Rating = dto.Rating,
            Comment = dto.Comment,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _repository.Create(review);
        await _repository.SaveChangesAsync();

        _logger.LogInformation("Review created successfully with ID: {ReviewId}", review.ReviewId);

        var created = await _repository.GetByIdWithCustomerAsync(review.ReviewId);

        return MapToDTO(created!);
    }

    public async Task<ReviewDTO?> EditReviewAsync(Guid id, Guid requestingUserId, EditReviewDTO dto)
    {
        _logger.LogInformation("Customer {UserId} attempting to edit review {ReviewId}", requestingUserId, id);

        var review = await _repository.GetByIdWithCustomerAsync(id);

        if (review == null)
        {
            _logger.LogWarning("Edit failed. Review not found with ID: {ReviewId}", id);
            return null;
        }

        if (review.CustomerId != requestingUserId)
        {
            _logger.LogWarning("Customer {UserId} tried to edit review {ReviewId} that does not belong to them", requestingUserId, id);
            throw new UnauthorizedAccessException("You can only edit your own reviews.");
        }

        review.Rating = dto.Rating ?? review.Rating;
        review.Comment = dto.Comment ?? review.Comment;
        review.UpdatedAt = DateTime.UtcNow;

        _repository.Update(review);
        await _repository.SaveChangesAsync();

        _logger.LogInformation("Review {ReviewId} updated successfully", id);

        return MapToDTO(review);
    }

    public async Task<bool> DeleteReviewAsync(Guid id, Guid requestingUserId, bool isAdmin)
    {
        _logger.LogInformation("User {UserId} attempting to delete review {ReviewId}", requestingUserId, id);

        var review = await _repository.GetByIdAsync(id);

        if (review == null)
        {
            _logger.LogWarning("Delete failed. Review not found with ID: {ReviewId}", id);
            return false;
        }

        if (!isAdmin && review.CustomerId != requestingUserId)
        {
            _logger.LogWarning("User {UserId} tried to delete review {ReviewId} that does not belong to them", requestingUserId, id);
            throw new UnauthorizedAccessException("You can only delete your own reviews.");
        }

        _repository.Delete(review);
        await _repository.SaveChangesAsync();

        _logger.LogInformation("Review {ReviewId} deleted successfully", id);

        return true;
    }

    private static ReviewDTO MapToDTO(Review r)
    {
        return new ReviewDTO
        {
            ReviewId = r.ReviewId,
            CustomerId = r.CustomerId,
            CustomerName = r.Customer != null ? $"{r.Customer.FirstName} {r.Customer.LastName}" : "",
            ReviewType = r.ReviewType,
            PartId = r.PartId,
            AppointmentId = r.AppointmentId,
            Rating = r.Rating,
            Comment = r.Comment,
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt
        };
    }
}
