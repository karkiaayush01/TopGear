namespace TopGear.Application.Interfaces;

public interface IUserService
{
    Task<bool> CheckUserExistsByEmail(string email);
}
