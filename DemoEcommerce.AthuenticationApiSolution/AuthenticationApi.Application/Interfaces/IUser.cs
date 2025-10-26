using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuthenticationApi.Application.DTO;
using eCommerce.SharedLibrary.Responses;

namespace AuthenticationApi.Application.Interfaces
{
    public interface IUser
    {
        Task<Response> Register(AppUserDTO appUserDTO);
         Task<GetUserDTO> GetUser(int userId);
        Task<Response> Login(LoginDTO loginDTO);

    }
}
