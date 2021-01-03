using System;
using System.Linq;
using System.Threading.Tasks;
using Eileen.Data;
using Eileen.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eileen.Controllers
{
    [Authorize, Route("Books")]
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public BooksController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 25)
        {
            if (page <= 0)
            {
                page = 1;
            }

            if (pageSize <= 0)
            {
                pageSize = 25;
            }

            var authors = await _dbContext.Books
                .OrderBy(a => a.Title)
                .ToPagedResultsAsync(page, pageSize);

            return View(authors);
        }

        [HttpGet("new")]
        public IActionResult New()
        {
            var isAuthorSelected = Request.Cookies.ContainsKey("selected-author-id");

            var viewModel = new NewBookViewModel()
            {
                IsAuthorSelected = isAuthorSelected,
                SelectedAutorId = isAuthorSelected ? int.Parse(Request.Cookies["selected-author-id"]) : default,
                SelectedAutorName = isAuthorSelected ? Request.Cookies["selected-author-name"] : null
            };

            return View(viewModel);
        }

        [HttpPost("ClearSelectedAuthor")]
        public IActionResult ClearSelectedAuthor(NewBookViewModel model)
        {
            Response.Cookies.Delete("selected-author-id");
            Response.Cookies.Delete("selected-author-name");

            return RedirectToAction("New");
        }

        [HttpPost("new")]
        public async Task<IActionResult> New(NewBookViewModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(model);
            }

            var newBook = new Book
            {
                AuthorId = model.SelectedAutorId,
                Title = model.Title
            };

            await _dbContext.Books.AddAsync(newBook);
            await _dbContext.SaveChangesAsync();

            Response.Cookies.Delete("selected-author-id");
            Response.Cookies.Delete("selected-author-name");

            return RedirectToAction("Index", "Book", new { id = newBook.Id });
        }
    }
}