using Contacts.Models;
using Contacts.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Contacts.Controllers
{
    [Authorize]
    public class ContactController : Controller
    {
        private readonly IContactRepository _repo;
        private readonly ICountryRepository _countryrepo;

        public ContactController(IContactRepository repo, ICountryRepository countryrepo)
        {
            _repo = repo;
            _countryrepo = countryrepo;
        }
        public async Task<IActionResult> Index()
        {
            // If we use List<Contact> as the contacts type, we will meet some errors.
            // because We returnes IEnumerable<Contacts> so it can't cast it to List one.
            //List<Contact> contacts = await _repo.GetListAsync(); X

            var contacts = await _repo.GetListAsync();
            return View(contacts);
        }

        // GET: /Contact/Create
        public async Task<IActionResult> Create()
        {
            var contactVM = new ContactVM
            {
                CountryList = (await _countryrepo.GetListAsync()).Select(c => new SelectListItem
                {
                    Text = c.CountryName,
                    Value = c.CountryId.ToString()
                }).ToList(),
                Contact = new Contact()
            };

            return View(contactVM);
        }

        // POST: /Contact/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ContactVM contactVM)
        {
            if (ModelState.IsValid)
            {
                if (contactVM.Contact != null)
                {
                    await _repo.CreateAsync(contactVM.Contact);
                }

                TempData["success"] = "Country Added to the List";
                return RedirectToAction(nameof(Index));
            }
            return View(contactVM);
        }

        // GET: Contact/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {            
            if(id == null)
            {
                return BadRequest();
            }

            var contactVM = new ContactVM
            {
                CountryList = (await _countryrepo.GetListAsync()).Select(c => new SelectListItem
                {
                    Text = c.CountryName,
                    Value = c.CountryId.ToString()
                }).ToList(),
                Contact = await _repo.GetAsync(id.Value)
            };

            if (contactVM.Contact == null)
            {
                return NotFound();
            }

            return View(contactVM);
        }

        // POST: Contact/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ContactVM contactVM)
        {
            if (contactVM.Contact == null || id != contactVM.Contact.Id)
            {
                return BadRequest();
            }
            if (!ModelState.IsValid)
            {
                return(View(contactVM));
            }

            await _repo.UpdateAsync(contactVM.Contact);

            TempData["success"] = "Contact Successfully Updated";
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var contactVM = new ContactVM
            {
                CountryList = (await _countryrepo.GetListAsync()).Select(c => new SelectListItem
                {
                    Text = c.CountryName,
                    Value = c.CountryId.ToString()
                }).ToList(),
                Contact = await _repo.GetAsync(id.Value)
            };

            if (contactVM.Contact == null)
            {
                return NotFound();
            }
            return View(contactVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, ContactVM contactVM)
        {

            if (contactVM.Contact == null || id != contactVM.Contact.Id)
            {
                return BadRequest();
            }
            if (!ModelState.IsValid)
            {
                return View(contactVM.Contact);
            }
            await _repo.DeleteAsync(id);

            TempData["success"] = "Contact Removed from the List";
            return RedirectToAction(nameof(Index));
        }
    }
}
