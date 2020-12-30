using System;
using System.Linq;
using System.Threading.Tasks;
using Eileen.Data;
using Eileen.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Eileen.Controllers
{
    [Authorize, Route("Authors")]
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

        [HttpGet("{id}/delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var authorProjection = await _dbContext.Authors
                .Select(a=>new
                {
                    a.Id,
                    a.Name
                })
                .SingleOrDefaultAsync(a => a.Id == id);

            //TODO handle author not found

            return View(new DeleteAuthorViewModel
            {
                Name = authorProjection.Name
            });
        }

        [HttpPost("{id}/delete")]
        public async Task<IActionResult> Delete(int id, DeleteAuthorRequest model)
        {
            var author = await _dbContext.Authors
                .Include(a => a.Books)
                .SingleOrDefaultAsync(a => a.Id == id);

            _dbContext.Authors.Remove(author);
            if (model.DeleteBooks)
            {
                _dbContext.Books.RemoveRange(author.Books);
            }

            await _dbContext.SaveChangesAsync();

            return RedirectToAction("List");
        }

        [HttpGet("New")]
        public IActionResult New()
        {
            return View(new NewAuthorViewModel());
        }

        [HttpPost("New"), ValidateAntiForgeryToken]
        public async Task<IActionResult> New(NewAuthorViewModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(model);
            }

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