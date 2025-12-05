using Microsoft.AspNetCore.Mvc;

namespace UsersMembers.API.Controllers
{
    public class ProfilesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
