using AuthenticationApi.Application.DTO;
using AuthenticationApi.Application.Interfaces;
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController(IUser user) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<ActionResult<Response>> Register(AppUserDTO appUser )
        {
            if (!ModelState.IsValid)return BadRequest(ModelState);

            var result = await user.Register(appUser);
            return result.Flag ? Ok(result) : BadRequest(Request);
        }

        [HttpPost("login")]
        public async Task<ActionResult<Response>> Login(LoginDTO userLogin)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await user.Login(userLogin);
            return result.Flag ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<GetUserDTO>> GetUser(int id)
        {
           if(id < 1)return BadRequest("invalid user id");

            var result = await user.GetUser(id);
            return  result.id < 1?BadRequest(result) : Ok(result);
        }




    }
}
