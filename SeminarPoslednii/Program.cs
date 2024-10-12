
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using AppSecuriAndContainer.Abstractions;
using AppSecuriAndContainer.Models;
using AppSecuriAndContainer.Repository;
using System.Security.Cryptography;

namespace AppSecuriAndContainer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            RSA rsa = RSA.Create();
            rsa.ImportFromPem(System.IO.File.ReadAllText(@"./public_key.pem").ToCharArray());

            var signingKey = new RsaSecurityKey(rsa);

            builder.Services.AddControllers();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false; 
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false, 
                    ValidateLifetime = true, 
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = signingKey 
                };
            });

                builder.Services.AddDbContext<UserDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
                 .UseLazyLoadingProxies()
                 .LogTo(Console.WriteLine));

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddScoped<ITokenRepository, TokenRepository>();

            builder.Services.AddScoped<IUserRepository, UserRepository>();
           



         


            var app = builder.Build();

          
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();


            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }

   
}
