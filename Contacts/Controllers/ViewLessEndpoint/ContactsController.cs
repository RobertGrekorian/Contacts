using Contacts.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Contacts.Controllers.ViewLessEndpoint
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        private readonly IContactRepository _contactRepository;

        public ContactsController(IContactRepository contactRepository) { 
            _contactRepository = contactRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            return Ok(await _contactRepository.GetListAsync());
        }
    }
}
