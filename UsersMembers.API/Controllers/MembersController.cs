using Microsoft.AspNetCore.Mvc;

namespace UsersMembers.API.Controllers
{
    public class MembersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
