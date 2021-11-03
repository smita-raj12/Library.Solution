using System.Collections.Generic;

namespace Library.Models
{
  public class CopyBook
  {       
    public int CopyBookId { get; set; }
    public int BookId { get; set; }
    public int CopyId { get; set; }
    public virtual Book Book { get; set; }
    public virtual Copy Copy { get; set; }
    
  }
}