using Contacts.Data;
using Contacts.Models;
using Contacts.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Contacts.Controllers
{
    [Authorize]
    public class ContactController : Controller
    {
        private readonly IContactRepository _repo;
        private readonly ICountryRepository _countryrepo;
        private readonly ISharedContactRepository _sharedcontactrepo;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public ContactController(IContactRepository repo, ICountryRepository countryrepo, ApplicationDbContext context,
                                ISharedContactRepository sharedContactRepository, UserManager<ApplicationUser> userManager)
        {
            _repo = repo;
            _countryrepo = countryrepo;
            _sharedcontactrepo = sharedContactRepository;
            _userManager = userManager;
            _context = context;
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

            ViewBag.Countries = (await _countryrepo.GetListAsync()).Select(c => new SelectListItem
            {
                Text = c.CountryName,
                Value = c.CountryId.ToString()
            }).ToList();

            return View(contactVM);
        }

        // POST: /Contact/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ContactVM contactVM)
        {

            if (await _context.Contacts.AnyAsync(u => u.Email == contactVM.Contact.Email))
            {
                ModelState.AddModelError(string.Empty, "Email is already in use.");
            }
            else if (ModelState.IsValid)
            {
                if (contactVM.Contact != null)
                {
                    await _repo.CreateAsync(contactVM.Contact);
                }

                TempData["success"] = "Country Added to the List";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Countries = (await _countryrepo.GetListAsync()).Select(c => new SelectListItem
            {
                Text = c.CountryName,
                Value = c.CountryId.ToString()
            }).ToList();
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
            ViewBag.Countries = (await _countryrepo.GetListAsync()).Select(c => new SelectListItem
            {
                Text = c.CountryName,
                Value = c.CountryId.ToString()
            }).ToList();
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
                ViewBag.Countries = (await _countryrepo.GetListAsync()).Select(c => new SelectListItem
                {
                    Text = c.CountryName,
                    Value = c.CountryId.ToString()
                }).ToList();
                return (View(contactVM));
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

        [HttpGet]
        public async Task<IActionResult> Share(int? id)
        {            
            if(id == null)
            {
                return BadRequest();
            }

            var model = new SharedContactVM
            {
                Id = id.Value,
                Users = _userManager.Users.Select(c => new SelectListItem
                {
                    //Text = c.FirstName + " " + c.LastName,
                    //Text = string.Format("{0} {1}", c.FirstName, c.LastName),
                    Text = $"{c.FirstName} {c.LastName}",
                    Value = c.Id
                }).ToList()
            };


            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Share(int id, SharedContactVM model)
        {
            //if (selectedUserIds == null)
            //{
            //    return BadRequest();
            //}

            await _sharedcontactrepo.ShareContactAsync(id, model.SelectedUserId, false);

            //foreach(var userId in selectedUserIds)
            //{
            //    await _sharedcontactrepo.ShareContactAsync(id,userId,false);
            //}

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> ShareWithViewBag(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var model = new SharedContactWithViewBagVM { Id = id.Value };
            ViewBag.Users = await _userManager.Users.Select(c => new SelectListItem
            {
                //Text = c.FirstName + " " + c.LastName,
                //Text = string.Format("{0} {1}", c.FirstName, c.LastName),
                Text = $"{c.FirstName} {c.LastName}",
                Value = c.Id
            }).ToListAsync();


            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ShareWithViewBag(int id, SharedContactWithViewBagVM model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Model is not valid.");
                ViewBag.Users = _userManager.Users.Select(c => new SelectListItem
                {
                    Text = $"{c.FirstName} {c.LastName}",
                    Value = c.Id
                }).ToList();
                return View(model);
            }

            await _sharedcontactrepo.ShareContactAsync(id, model.SelectedUserId, false);

            //foreach(var userId in selectedUserIds)
            //{
            //    await _sharedcontactrepo.ShareContactAsync(id,userId,false);
            //}

            return RedirectToAction(nameof(Index));
        }
    }
}
