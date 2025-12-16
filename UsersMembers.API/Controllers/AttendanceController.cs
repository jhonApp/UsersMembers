using Microsoft.AspNetCore.Mvc;

namespace UsersMembers.API.Controllers
{
    public class AttendanceController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
