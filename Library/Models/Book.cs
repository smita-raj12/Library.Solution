using System.Collections.Generic;
using System;

namespace Library.Models
{
  public class Book
  {
    public Book()
    {
      this.JoinEntities = new HashSet<AuthorBook>();
    }

    public int BookId { get; set; }

    public string Title { get; set; }
    public string Description { get; set; }
    //public DateTime DueDate { get; set; }
    public virtual ApplicationUser User { get; set; }
    public virtual ICollection<CopyBook> Copies { get; set;}
    public virtual ICollection<AuthorBook> JoinEntities { get;}
  }
}