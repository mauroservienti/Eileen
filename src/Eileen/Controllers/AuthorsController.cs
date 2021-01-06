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

        [HttpGet("{id}/books/new")]
        public async Task<IActionResult> NewAuthorBook(int id)
        {
            var author = await _dbContext.Authors
                .Select(a=> new
                {
                    a.Id,
                    a.Name
                })
                .SingleOrDefaultAsync(a => a.Id == id);

            return View(new NewAuthorBookViewModel{ AuthorName = author.Name });
        }

        [HttpPost("{id}/books/new"), ValidateAntiForgeryToken]
        public async Task<IActionResult> NewAuthorBook(int id, NewAuthorBookRequest model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(model);
            }

            var author = await _dbContext.Authors
                .Include(a => a.Books)
                .SingleOrDefaultAsync(a => a.Id == id);

            if (author == null)
            {
                throw new ArgumentNullException("id", $"Author {id} not found");
            }

            author.Books.Add(new ()
            {
                Title = model.BookTitle
            });

            await _dbContext.SaveChangesAsync();

            return RedirectToAction("View", new {id = author.Id});
        }
        
        [HttpGet("{id}/books/{bookId}/delete")]
        public async Task<IActionResult> DeleteAuthorBookConfirmation(int id, int bookId)
        {
            var author = await _dbContext.Authors
                .Select(a=> new
                {
                    a.Id,
                    a.Name
                })
                .SingleOrDefaultAsync(a => a.Id == id);
            
            if (author == null)
            {
                throw new ArgumentNullException(nameof(id), $"Author {id} not found");
            }
            
            var book = await _dbContext.Books
                .Select(b=> new
                {
                    b.Id,
                    b.Title
                })
                .SingleOrDefaultAsync(b => b.Id == id);
            
            if (book == null)
            {
                throw new ArgumentNullException(nameof(bookId), $"Book {bookId} not found");
            }

            return View(new DeleteAuthorBookViewModel{ AuthorName = author.Name,  BookTitle = book.Title });
        }

        [HttpPost("{id}/books/{bookId}/delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAuthorBook(int id, int bookId)
        {
            _dbContext.Books.Remove(new Book() {Id = bookId});
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("View", new {id = id});
        }

        [HttpGet("select")]
        public async Task<IActionResult> Select(string q = null, int page = 1, int pageSize = 25)
        {
            if (page <= 0)
            {
                page = 1;
            }

            if (pageSize <= 0)
            {
                pageSize = 25;
            }

            IQueryable<Author> authorsQuery = _dbContext.Authors;

            if(!string.IsNullOrWhiteSpace(q))
            {
                authorsQuery = authorsQuery.Where(a => a.Name.Contains(q));
            }

            var authors = await authorsQuery.OrderBy(a => a.Name)
                .ToPagedResultsAsync(page, pageSize);

            return View(authors);
        }

        [HttpPost("select/{id}"), ValidateAntiForgeryToken]
        public async Task<IActionResult> Select(int id)
        {
            var author = await _dbContext.Authors.FindAsync(id);

            //TODO handle author not found

            Response.Cookies.Append("selected-author-id", author.Id.ToString());
            Response.Cookies.Append("selected-author-name", author.Name);

            return RedirectToAction("New", "Books");
        }
    }
}