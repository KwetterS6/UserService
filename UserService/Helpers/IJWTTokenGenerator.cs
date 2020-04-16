using UserService.Models;

namespace UserService.Helpers
{
    public interface IJWTTokenGenerator
    {
        User Authenticate(User user);
    }
}