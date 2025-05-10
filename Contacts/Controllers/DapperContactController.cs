using Contacts.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Contacts.Controllers
{
    public class DapperContactController : Controller
    {
        private readonly IDapperContactRepository _repo;

        public DapperContactController(IDapperContactRepository repo) {
            _repo = repo;
        }

        public async Task<IActionResult> Index()
        {
            var contacts = await _repo.GetListAsync();
            return View(contacts);
        }
    }
}
