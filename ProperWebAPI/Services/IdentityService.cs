using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using ProperWebAPI.Domain;
using ProperWebAPI.Options;

namespace ProperWebAPI.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<IdentityUser> userManager; //User manager to simplify Identity manipulation
        private readonly JwtOptions jwtOptions; // Json Web Token Options (settings)

        public IdentityService(UserManager<IdentityUser> UserManager, JwtOptions JwtSettings)
        {
            userManager = UserManager;
            jwtOptions = JwtSettings;
        }

        public async Task<AuthenticationResult> LoginAsync(string email, string password)
        {
            try
            {
                var user = await userManager.FindByEmailAsync(email: email); // Check if user Exists

                if (user == null)  // if not
                {
                    return new AuthenticationResult  // return an error
                    {
                        Errors = new[] { "User Authentication Failed" }

                    };
                }

                var userHasValidPassword = await userManager.CheckPasswordAsync(user, password);

                if (!userHasValidPassword)
                {
                    return new AuthenticationResult
                    {
                        Errors = new[] { "User Authentication Failed" }

                    };
                }

                return GenerateAuthenticationResultForUser(user);
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
                return new AuthenticationResult
                {
                    Errors = new[] { $"Unknown Authentication error: {ex.Message}" }

                };
            }
        }

        public async Task<AuthenticationResult> RegisterAsync(string email, string password)
        {
            try
            {
                var existingUser = await userManager.FindByEmailAsync(email: email);

                if (existingUser != null)
                {
                    return new AuthenticationResult
                    {
                        Errors = new[] { "User with this email already exists" }

                    };
                }

                var newUser = new IdentityUser
                {
                    Email = email,
                    UserName = email
                };

                var createdUser = await userManager.CreateAsync(newUser, password);

                if (!createdUser.Succeeded)
                {
                    return new AuthenticationResult
                    {
                        Errors = createdUser.Errors.Select(x => x.Description)
                    };
                }

                return GenerateAuthenticationResultForUser(newUser);
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message) ;
                return new AuthenticationResult
                {
                    Errors = new[] { $"Unknown Authentication error: {ex.Message}" }

                };
            }


        }

        private AuthenticationResult GenerateAuthenticationResultForUser(IdentityUser newUser)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(jwtOptions.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims: new[]
                {
                    new Claim(type: JwtRegisteredClaimNames.Sub, value: newUser.Email),
                    new Claim(type: JwtRegisteredClaimNames.Jti, value: Guid.NewGuid().ToString()),
                    new Claim(type: JwtRegisteredClaimNames.Email, value: newUser.Email),
                    new Claim(type: "id", value: newUser.Id)
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), algorithm: SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new AuthenticationResult
            {
                Success = true,
                Token = tokenHandler.WriteToken(token)
            };
        }
    }
}
