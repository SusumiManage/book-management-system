using BookManagement.Api.Dtos;

namespace BookManagement.Api.Services
{
    public interface IAuthService
    {
        Task<(bool ok, string? error)> RegisterAsync(RegisterDto dto);
        Task<(bool ok, string? error, string? token)> LoginAsync(LoginDto dto);
        Task<List<UserListDto>> GetRegisteredUsersAsync();
    }
}
