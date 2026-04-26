using TopGear.Application.DTOs.ReviewDTO;

namespace TopGear.Application.Interfaces;

public interface IReviewService
{
    Task<IEnumerable<ReviewDTO>> GetAllReviewsAsync();
    Task<ReviewDTO?> GetReviewByIdAsync(Guid id);
    Task<IEnumerable<ReviewDTO>> GetReviewsByPartAsync(Guid partId);
    Task<IEnumerable<ReviewDTO>> GetReviewsByAppointmentAsync(Guid appointmentId);
    Task<IEnumerable<ReviewDTO>> GetReviewsByCustomerAsync(Guid customerId);
    Task<ReviewDTO> CreateReviewAsync(Guid customerId, CreateReviewDTO dto);
    Task<ReviewDTO?> EditReviewAsync(Guid id, Guid requestingUserId, EditReviewDTO dto);
    Task<bool> DeleteReviewAsync(Guid id, Guid requestingUserId, bool isAdmin);
}
