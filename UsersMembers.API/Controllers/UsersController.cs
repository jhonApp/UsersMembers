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
        public async Task<IActionResult> CreateUser(RequestUser request, CancellationToken ct)
        {
            var result = await _userService.CreateAsync(request, ct);
            
            if (result.Success)
            {
                return Ok(result.Data);
            }

            return BadRequest(result.Message);
        }
    }
}
