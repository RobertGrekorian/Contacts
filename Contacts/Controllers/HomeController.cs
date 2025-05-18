using ContactData;
using ContactLib.Services;
using Contacts.Data;
using Contacts.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Contacts.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ITestService _testService;
        private readonly IDapperContactRepository _dapperContactRepository;

        public HomeController(ILogger<HomeController> logger, ITestService testService,
            IDapperContactRepository dapperContactRepository)
        {
            _logger = logger;
            _testService = testService;
            _dapperContactRepository = dapperContactRepository;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Message = _testService.GetHelloWorld();

            ViewBag.Message = _dapperContactRepository.GetContactName();

            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
