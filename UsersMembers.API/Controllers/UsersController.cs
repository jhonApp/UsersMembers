using Microsoft.AspNetCore.Mvc;
using UsersMembers.Application.Interface;
using UsersMembers.Application.ViewModels.User;

namespace UsersMembers.API.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("create")]
        public ActionResult<ResponseUser> CreateUser(RequestUser request)
        {
            return Ok(_userService.Create(request));
        }
    }
}
