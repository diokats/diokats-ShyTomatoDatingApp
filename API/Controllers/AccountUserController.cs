using System.Security.Cryptography;
using System.Text;
using API.Controllers;
using API.Data;
using API.DTOS;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Services;

namespace API.Controllers
{


    public class AccountController(DataContext context, ITokenService tokenService) : BaseApiController
    {
        [HttpPost("register")] // account/register
        public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDto)
        {
            if (await UserExists(registerDto.Username)) return BadRequest("Username is taken");
            
                        using var hmac = new HMACSHA512();

                        var user = new AppUser
            {
                UserName = registerDto.Username.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key
            };
context.Users.Add(user);
            await context.SaveChangesAsync();

                    return new UserDTO{
                    Username = user.UserName,
                    Token = tokenService.CreateToken(user)
                };
            }

            [HttpPost("login")] // account/login
            public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDto)
            {
                var user = await context.Users.FirstOrDefaultAsync(x => x.UserName == loginDto.Username.ToLower());
                    

                if (user == null) return Unauthorized("Invalid username");

                using var hmac = new HMACSHA512(user.PasswordSalt);
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid password");
                }

                return new UserDTO{
                    Username = user.UserName,
                    Token = tokenService.CreateToken(user)
                };
            }
            private async Task<bool> UserExists(string username)
            {
                return await context.Users.AnyAsync(x => x.UserName.ToLower() == username.ToLower());
            }
    }
}
        
        

    