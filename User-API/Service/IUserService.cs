using User_API.DTOs;
using User_API.Models;

namespace User_API.Service
{
    public interface IUserService
    {
        Task<User> CreateUserAsync(CreateUserDTO userCreateDto);
        Task<User?> GetUserByIdAsync(int id);
        Task<User?> GetUserByUsernameAsync(string username);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User?> UpdateUserAsync(int id, UpdateUserDTO userUpdateDto);
        Task<bool> DeleteUserAsync(int id);
    }
}