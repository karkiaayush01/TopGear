using TopGear.Application.DTOs.ForgotPasswordDTO;

namespace TopGear.Application.Interfaces;

public interface IForgotPasswordService
{
    Task<ForgotPasswordRequestDTO> CreateRequest(string email);

    Task ResetPassword(ResetPasswordDTO data);
}
