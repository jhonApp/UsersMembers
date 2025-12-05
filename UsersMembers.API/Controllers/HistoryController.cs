using Microsoft.AspNetCore.Mvc;

namespace UsersMembers.API.Controllers
{
    public class HistoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
