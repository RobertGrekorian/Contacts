using Microsoft.AspNetCore.Mvc;

namespace Contacts.Controllers
{
    public class SignalController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
