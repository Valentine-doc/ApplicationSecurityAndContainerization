using AppSecuriAndContainer.Abstractions;
using AppSecuriAndContainer.Models;
using AppSecuriAndContainer.ViewModels;
using System.Security.Cryptography;
using System.Text;

namespace AppSecuriAndContainer.Repository
{
    public class UserRepository : IUserRepository
    {
       private readonly ITokenRepository _tokenRepository;

        public UserRepository(ITokenRepository tokenRepository)=> _tokenRepository = tokenRepository;
        
        public string AddUser(UserDto userDto)
        {
            using (var context = new UserDbContext()) 
            {
                if (userDto.userRoleEnum == UserRoleE.Admin)
                {
                    if (context.Users.Any(u => u.RoleId == RoleId.Admin))
                    {
                        throw new Exception("Admin already exists");
                    }
                    else 
                    {

                        var userToAdd = new User() { UserName = userDto.Name };
                        userToAdd.RoleId = RoleId.Admin;
                        userToAdd.Salt = new byte[16];
                        new Random().NextBytes(userToAdd.Salt);
                        var data = Encoding.UTF8.GetBytes(userDto.Password).Concat(userToAdd.Salt).ToArray();
                        userToAdd.Password = new SHA512Managed().ComputeHash(data);

                        context.Users.Add(userToAdd);
                        context.SaveChanges();

                        return _tokenRepository.GenerateToken(userDto);
                    }
                }
                else 
                {
                    if (context.Users.Any(u => u.UserName == userDto.Name))
                    {
                        throw new Exception("User already exists");
                    }
                    else 
                    {

                        var userToAdd = new User() {UserName = userDto.Name};
                        userToAdd.RoleId = RoleId.User;
                        userToAdd.Salt = new byte[16];
                        new Random().NextBytes(userToAdd.Salt);
                        var data = Encoding.UTF8.GetBytes(userDto.Password).Concat(userToAdd.Salt).ToArray();
                        userToAdd.Password = new SHA512Managed().ComputeHash(data);

                        context.Users.Add(userToAdd);
                        context.SaveChanges();

                        return _tokenRepository.GenerateToken(userDto);
                    }
                }
            }
           
        }

        public string CheckUser(LoginDto loginDto)
        {
            using (var context = new UserDbContext()) 
            {
                if (context.Users.Any(u => u.UserName == loginDto.Name))
                { throw new Exception("User name is not found"); }

                else 
                {
                    var user = context.Users.FirstOrDefault(x=>x.UserName == loginDto.Name);

                    var data = Encoding.UTF8.GetBytes(loginDto.Password).Concat(user.Salt).ToArray();

                    var hash = new SHA512Managed().ComputeHash(data);

                    if (!user.Password.SequenceEqual(hash))
                    {throw new Exception("Wrong password");}


                    else 
                    {
                        var userDto = new UserDto()
                        {
                            Name = user.UserName,
                            userRoleEnum = (UserRoleE)user.RoleId,
                            Password = Encoding.UTF8.GetString(user.Password)
                        };
                        return _tokenRepository.GenerateToken(userDto);
                    }
                }
            }
        }

       
    }
}
