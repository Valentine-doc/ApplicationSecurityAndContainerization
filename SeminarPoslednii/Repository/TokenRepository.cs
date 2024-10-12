namespace AppSecuriAndContainer.Repository
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.IO;
    using System.Security.Claims;
    using System.Security.Cryptography;
    using System.Text;
    using Microsoft.IdentityModel.Tokens;
    using AppSecuriAndContainer.Abstractions;
    using AppSecuriAndContainer.ViewModels;

    public class TokenRepository:ITokenRepository
    {
        private static RSAParameters _privateKey;
        private static RSAParameters _publicKey;

        public TokenRepository()
        {
            _privateKey = LoadPrivateKey("./private_key.pem");
            _publicKey = LoadPublicKey("./public_key.pem");
        }

        // Метод для генерации токена
        public string GenerateToken(UserDto userDto)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var rsaSecurityKey = new RsaSecurityKey(_privateKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                // Идентификатор пользователя и роль
                {
                new Claim(ClaimTypes.Name, userDto.Name),
                new Claim(ClaimTypes.Role, userDto.userRoleEnum.ToString())
                }),

                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(rsaSecurityKey, SecurityAlgorithms.RsaSha256)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        // Метод для валидации токена
        public bool ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var rsaSecurityKey = new RsaSecurityKey(_publicKey);

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = rsaSecurityKey,
                    ValidateIssuer = false,
                    ValidateAudience = false
                }, out SecurityToken validatedToken);

                return true;
            }
            catch
            {
                return false;
            }
        }


        private static RSAParameters LoadPrivateKey(string privateKeyPath)
        {
            using (var reader = new StreamReader(privateKeyPath))
            {
                var pem = reader.ReadToEnd();
                var rsa = RSA.Create();
                rsa.ImportFromPem(pem.ToCharArray());
                return rsa.ExportParameters(true);
            }
        }


        private static RSAParameters LoadPublicKey(string publicKeyPath)
        {
            using (var reader = new StreamReader(publicKeyPath))
            {
                var pem = reader.ReadToEnd();
                var rsa = RSA.Create();
                rsa.ImportFromPem(pem.ToCharArray());
                return rsa.ExportParameters(false);
            }
        }
    }

   

}
