using System.Collections.Generic;
using System.ComponentModel;
using System;

namespace Library.Models
{
  public class Copy
  {
    public Copy()
    {
      this.JoinEntities = new HashSet<CopyBook>();
    }
    public int CopyId { get; set; }
    public int CopyQuant { get; set; }
    public int BookId { get; set; }
    public virtual Book Book { get; set; }
    public bool IsCheckedOut { get; set; }
    public virtual ApplicationUser User { get; set; }
    public virtual ICollection<CopyBook> JoinEntities { get;}
  }
}