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
 
  public class CopiesController : Controller
  {
    private readonly LibraryContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public CopiesController(UserManager<ApplicationUser> userManager, LibraryContext db)
    {
      _userManager = userManager;
      _db = db;
    }

     [Authorize(Roles = "Librarian,Patron")]
    public async Task<ActionResult> Index()
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      var userCopies = _db.Copies.Where(entry => entry.User.Id == currentUser.Id).ToList();
      return View(userCopies);
      
      
    }
    [Authorize(Roles = "Librarian")]
    public ActionResult Create()
    {
      ViewBag.BookId = new SelectList(_db.Books, "BookId", "Title");
      return View();
    }
    [Authorize(Roles = "Librarian")]
    [HttpPost]
    public async Task<ActionResult> Create(Copy copy, int BookId, int id)
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      copy.User = currentUser;
      _db.Copies.Add(copy);
      _db.SaveChanges();
      if (BookId != 0)
      {
          _db.CopyBook.Add(new CopyBook() { BookId = BookId, CopyId = copy.CopyId });
      }
      _db.SaveChanges();
      return RedirectToAction("Index");
    }
  [Authorize(Roles = "Librarian,Patron")]
    public ActionResult Details(int id)
    {
      var thisCopy = _db.Copies
          .Include(copy => copy.JoinEntities)
          .ThenInclude(join => join.Book)
          .FirstOrDefault(copy => copy.CopyId == id);
      return View(thisCopy);
    }
   [Authorize(Roles = "Librarian")]
    public ActionResult Edit(int id)
    {
      var thisCopy = _db.Copies.FirstOrDefault(copy => copy.CopyId == id);
      ViewBag.BookId = new SelectList(_db.Copies, "BookId", "Title");
      return View(thisCopy);
    }

   [Authorize(Roles = "Librarian")]
    [HttpPost]
    public ActionResult Edit(Copy copy, int BookId)
    {
      if (BookId != 0)
      {
        _db.CopyBook.Add(new CopyBook() { BookId = BookId, CopyId = copy.CopyId });
      }
      _db.Entry(copy).State = EntityState.Modified;
      _db.SaveChanges();
      return RedirectToAction("Index");
    }
  //  [Authorize(Roles = "Librarian")]
  //   public ActionResult AddAuthor(int id)
  //   {
  //     var thisBook = _db.Books.FirstOrDefault(book => book.BookId == id);
  //     ViewBag.AuthorId = new SelectList(_db.Authors, "AuthorId", "Name");
  //     return View(thisBook);
  //   }
  //  [Authorize(Roles = "Librarian")]
  //   [HttpPost]
  //   public ActionResult AddAuthor(Book book, int AuthorId)
  //   {
  //     if (AuthorId != 0)
  //     {
  //     _db.AuthorBook.Add(new AuthorBook() { AuthorId = AuthorId, BookId = book.BookId });
  //     }
  //     _db.SaveChanges();
  //     return RedirectToAction("Index");
  //   }
     [Authorize(Roles = "Librarian")]
    public ActionResult Delete(int id)
    {
      var thisCopy = _db.Copies.FirstOrDefault(copy => copy.CopyId == id);
      return View(thisCopy);
    }
   [Authorize(Roles = "Librarian")]
    [HttpPost, ActionName("Delete")]
    public ActionResult DeleteConfirmed(int id)
    {
      var thisCopy = _db.Copies.FirstOrDefault(copy => copy.CopyId == id);
      _db.Copies.Remove(thisCopy);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

  // [Authorize(Roles = "Librarian")]
  //   [HttpPost]
  //   public ActionResult DeleteAuthor(int joinId)
  //   {
  //     var joinEntry = _db.AuthorBook.FirstOrDefault(entry => entry.AuthorBookId == joinId);
  //     _db.AuthorBook.Remove(joinEntry);
  //     _db.SaveChanges();
  //     return RedirectToAction("Index");
  //   }
  }
}