using Sudoku.App.Models;
using Sudoku.App.Models.FormModels;

namespace Sudoku.App.Repositories.Contracts;

public interface IAppUserRepository
{
    Task<CurrentUser> RegisterAsync(RegisterModel registerModel);
    Task<CurrentUser?> LoginAsync(LoginModel loginModel);
    Task<bool> EmailExistsAsync(string email);
    Task<bool> UserNameExistsAsync(string userName);
}