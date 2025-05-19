
using Contacts.Repositories;
using ContactsData.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Contacts.Controllers
{
    public class DapperContactController : Controller
    {
        private readonly IDapperContactRepository _repo;

        private readonly ICountryRepository _countryrepo;

        public DapperContactController(IDapperContactRepository repo, ICountryRepository countryrepo)
        {
            _repo = repo;
            _countryrepo = countryrepo;            
        }

        public async Task<IActionResult> Index()
        {
            
            var countries = (await _countryrepo.GetListAsync()).Select(c => new SelectListItem
            {
                Text = c.CountryName,
                Value = c.CountryId.ToString()
            }).ToList();

            var model = new ContactVM
            {
                CountryList = countries
            };

            return View(model);
        }

        public async Task<IActionResult> GetContacts()
        {
            var contacts = await _repo.GetListAsync(); // Ensure this includes countryName
            var countries = (await _countryrepo.GetListAsync()).Select(c => new
            {
                Text = c.CountryName,
                Value = c.CountryId.ToString()
            }).ToList();

            return Json(new { data = contacts, countryList = countries });
        }
    }
}
