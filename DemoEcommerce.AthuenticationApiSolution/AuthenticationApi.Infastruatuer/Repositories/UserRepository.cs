using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AuthenticationApi.Application.DTO;
using AuthenticationApi.Application.Interfaces;
using AuthenticationApi.Domain.Entities;
using AuthenticationApi.Infrastructure.Data;
using BCrypt.Net;
using eCommerce.SharedLibrary;
using eCommerce.SharedLibrary.DependencyInjection;
using eCommerce.SharedLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AuthenticationApi.Infrastructure.Repositories
{
    public class UserRepository(AuthenticationDbContext context , IConfiguration config) : IUser
    {
        public async Task<AppUser> GetUserByEmail(string Email)
        {
            try
            {

                var user = await context.users.FirstOrDefaultAsync(u => u.Email == Email);

                return user is null ? null! : user;
                    

               
            }
            catch (Exception ex) {


                LogException.LogExceptions(ex);
                return null!;
            }


        }
        public async Task<GetUserDTO> GetUser(int userId)
        {
            try
            {



                var user = await context.users.FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null || user.Id < 0)
                    return null!;

                return new GetUserDTO(user.Id,
                    user.Name!,
                    user.TelephoneNumber!,
                    user.Address!,
                    user.Email!,
                    user.Role!
                    );
            }
            catch (Exception ex)
            {

                LogException.LogExceptions(ex);
                return null!;
            }

            
        }

        public async Task<Response> Login(LoginDTO loginDTO)
        {


            var getUser = await GetUserByEmail(loginDTO.Email);

            if (getUser is null)
                return new Response(false, $"Invalid credentials");

            bool verifyPassword = BCrypt.Net.BCrypt.Verify(loginDTO.Password, getUser.Password);
            if (verifyPassword) {

                string token = GeneratToken(getUser);
                return new Response(true,token);
            }
            else
            {
                return new Response(false, $"Invalid credentials");

            }

        }

        public async Task<Response> Register(AppUserDTO appUserDTO)
        {
            var getuser = await GetUserByEmail(appUserDTO.Email);
            if (getuser != null)
                return new Response(false, $"you cannot use this email for registraing");

            var result = await context.users.AddAsync(new Domain.Entities.AppUser {Name = appUserDTO.Name,
            Email = appUserDTO.Email,
             Password = BCrypt.Net.BCrypt.HashPassword(appUserDTO.Password),
             Role = appUserDTO.Role,
             TelephoneNumber = appUserDTO.TelephoneNumber,
             DateRegistered = DateTime.Now
             , Address = appUserDTO.Address
            }) ;

            await context.SaveChangesAsync();

           return result.Entity.Id >0 ? new Response(true,"User Registered successfully"):
              new Response(false, "Invalid data probided");
        }

        private  string GeneratToken(AppUser user)
        {
            var myjwtoptions = config.GetSection("Authentication").Get<AuthJWTBerrer>();

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(myjwtoptions!.SigningKey));
            var credentilas = new SigningCredentials(securityKey,SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,user.Name!),
                new Claim(ClaimTypes.Email,user.Email!),
                new Claim(ClaimTypes.Role,!string.IsNullOrEmpty(user.Role)||!Equals("string",user.Role)?user.Role:ClaimTypes.Role)


            };

            var token = new JwtSecurityToken(
               issuer: myjwtoptions.Issuer,
               audience : myjwtoptions.Audience,
               claims : claims,
                expires: null,
                signingCredentials: credentilas
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
