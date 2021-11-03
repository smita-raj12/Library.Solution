using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Library.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
namespace Library.Controllers
{
 
  public class BooksController : Controller
  {
    private readonly LibraryContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public BooksController(UserManager<ApplicationUser> userManager, LibraryContext db)
    {
      _userManager = userManager;
      _db = db;
    }

    [AllowAnonymous]
    public  ActionResult Index(string searchString)
    {
      
            IQueryable <Book> userBooks = _db.Books.OrderByDescending(book => book.Title);
            if (!string.IsNullOrEmpty(searchString))
            {
                userBooks =
                    userBooks
                        .Where(rate => rate.Title.Contains(searchString));
            }
      // var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      // var currentUser = await _userManager.FindByIdAsync(userId);
      // var userBooks = _db.Books.Where(entry => entry.User.Id == currentUser.Id).ToList();
      // return View(userBooks);
      
      return View(userBooks.ToList());
    }
    [Authorize(Roles = "Librarian")]
    public ActionResult Create()
    {
      ViewBag.AuthorId = new SelectList(_db.Authors, "AuthorId", "Name");
      return View();
    }
    [Authorize(Roles = "Librarian")]
    [HttpPost]
    public async Task<ActionResult> Create(Book book, int AuthorId)
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      book.User = currentUser;
      _db.Books.Add(book);
      _db.SaveChanges();
      if (AuthorId != 0)
      {
          _db.AuthorBook.Add(new AuthorBook() { AuthorId = AuthorId, BookId = book.BookId });
      }
      _db.SaveChanges();
      return RedirectToAction("Index");
    }
   [AllowAnonymous]
    public ActionResult Details(int id)
    {
      var thisBook = _db.Books
          .Include(book => book.JoinEntities)
          .ThenInclude(join => join.Author)
          .FirstOrDefault(book => book.BookId == id);
      return View(thisBook);
    }
    [Authorize(Roles = "User")]
    public ActionResult Edit(int id)
    {
      var thisBook = _db.Books.FirstOrDefault(book => book.BookId == id);
      ViewBag.AuthorId = new SelectList(_db.Authors, "AuthorId", "Name");
      return View(thisBook);
    }

   [Authorize(Roles = "Librarian")]
    [HttpPost]
    public ActionResult Edit(Book book, int AuthorId)
    {
      if (AuthorId != 0)
      {
        _db.AuthorBook.Add(new AuthorBook() { AuthorId = AuthorId, BookId = book.BookId });
      }
      _db.Entry(book).State = EntityState.Modified;
      _db.SaveChanges();
      return RedirectToAction("Index");
    }
   [Authorize(Roles = "Librarian")]
    public ActionResult AddAuthor(int id)
    {
      var thisBook = _db.Books.FirstOrDefault(book => book.BookId == id);
      ViewBag.AuthorId = new SelectList(_db.Authors, "AuthorId", "Name");
      return View(thisBook);
    }
   [Authorize(Roles = "Librarian")]
    [HttpPost]
    public ActionResult AddAuthor(Book book, int AuthorId)
    {
      if (AuthorId != 0)
      {
      _db.AuthorBook.Add(new AuthorBook() { AuthorId = AuthorId, BookId = book.BookId });
      }
      _db.SaveChanges();
      return RedirectToAction("Index");
    }
     [Authorize(Roles = "Librarian")]
    public ActionResult Delete(int id)
    {
      var thisBook = _db.Books.FirstOrDefault(book => book.BookId == id);
      return View(thisBook);
    }
   [Authorize(Roles = "Librarian")]
    [HttpPost, ActionName("Delete")]
    public ActionResult DeleteConfirmed(int id)
    {
      var thisBook = _db.Books.FirstOrDefault(book => book.BookId == id);
      _db.Books.Remove(thisBook);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

  [Authorize(Roles = "Librarian")]
    [HttpPost]
    public ActionResult DeleteAuthor(int joinId)
    {
      var joinEntry = _db.AuthorBook.FirstOrDefault(entry => entry.AuthorBookId == joinId);
      _db.AuthorBook.Remove(joinEntry);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }
  }
}