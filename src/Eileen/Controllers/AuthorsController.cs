using System.Linq;
using System.Threading.Tasks;
using Eileen.Data;
using Eileen.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Eileen.Controllers
{
    [Route("Authors")]
    public class AuthorsController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public AuthorsController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> List(int page = 1, int pageSize = 25)
        {
            if (page <= 0)
            {
                page = 1;
            }

            if (pageSize <= 0)
            {
                pageSize = 25;
            }

            var authors = await _dbContext.AuthorsWithBooksCountView
                .OrderBy(a => a.Name)
                .ToPagedResultsAsync(page, pageSize);

            return View(authors);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> View(int id)
        {
            var author = await _dbContext.Authors
                .Include(a => a.Books)
                .SingleOrDefaultAsync(a => a.Id == id);

            //TODO handle author not found
            return View(author);
        }

        [HttpGet("New")]
        public IActionResult New()
        {
            return View(new NewAuthorViewModel());
        }

        [HttpPost("New")]
        public async Task<IActionResult> New(NewAuthorViewModel model)
        {
            var author = new Author()
            {
                Name = model.Name
            };
            await _dbContext.Authors.AddAsync(author);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("View", new {id = author.Id});
        }
    }
}