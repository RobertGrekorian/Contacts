using Contacts.Models;
using Contacts.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Contacts.Controllers
{
    public class CountryController : Controller
    {
        public ICountryRepository _repo;
        public CountryController(ICountryRepository repo) {

            _repo = repo;
        }
        public async Task<IActionResult> Index()
        {
            var countries = await _repo.GetListAsync();
            return View(countries);
        }

        public  IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Country country)
        {
            if(country == null)
            {
                return BadRequest();
            }
            if (!ModelState.IsValid)
            {
                return View(country);
            }
            await _repo.CreateAsync(country);
                        
            return RedirectToAction(nameof(Index)); 
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if(id == null)
            {
                return BadRequest();
            }
            Country country = await _repo.GetByIdAsync(id.Value);
            if(country == null)
            {
                return NotFound();
            }
            return View(country);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,Country country)
        {

            if (id != country.CountryId)
            {
                return BadRequest();
            }
            if (!ModelState.IsValid)
            {
                return View(country);
            }
            await _repo.UpdateAsync(country);

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Country country = await _repo.GetByIdAsync(id.Value);
            if (country == null)
            {
                return NotFound();
            }
            return View(country);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, Country country)
        {

            if (id != country.CountryId)
            {
                return BadRequest();
            }
            if (!ModelState.IsValid)
            {
                return View(country);
            }
            await _repo.DeleteAsync(id);

            return RedirectToAction(nameof(Index));
        }
    }
}
