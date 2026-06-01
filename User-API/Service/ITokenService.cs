using User_API.Models;

namespace User_API.Service
{
    public interface ITokenService
    {
        string CreateToken(User user);
    }
}