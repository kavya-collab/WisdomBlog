using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WisdomBlog.API.Authorization;
using WisdomBlog.API.Services;
using WisdomBlog.Models.RequestModels;

namespace WisdomBlog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LoginController : ControllerBase
    {
        private IUserService _userService;

        public LoginController(
           IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("UserAuthenticate")]
        public IActionResult UserAuthenticate(LoginRequest model)
        {
            var response = _userService.UserAuthenticate(model);
            return Ok(response);
        }
    }
}
