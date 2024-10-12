using AppSecuriAndContainer.ViewModels;
using System.Security.Cryptography;

namespace AppSecuriAndContainer.Abstractions
{
    public interface ITokenRepository
    {
        string GenerateToken(UserDto userDto);

        bool ValidateToken(string token);

    }
}
