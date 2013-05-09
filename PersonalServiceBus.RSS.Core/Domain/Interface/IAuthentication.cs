using PersonalServiceBus.RSS.Core.Contract;
using PersonalServiceBus.RSS.Core.Domain.Model;

namespace PersonalServiceBus.RSS.Core.Domain.Interface
{
    public interface IAuthentication
    {
        Status Register(User user);
        SingleResponse<bool> ValidateUser(User user);
        SingleResponse<bool> ChangePassword(string username, string oldPassword, string newPassword);
        SingleResponse<User> GetUserByUsername(User user);
        SingleResponse<User> UpdateUser(User user);
    }
}