using AppSecuriAndContainer.Models;
using AppSecuriAndContainer.ViewModels;

namespace AppSecuriAndContainer.Abstractions
{
    public interface IUserRepository
    {
        string AddUser(UserDto userDto);
        
        string CheckUser(LoginDto loginDto);
        
    }
}
