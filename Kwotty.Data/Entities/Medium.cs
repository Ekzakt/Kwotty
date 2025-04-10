using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Kwotty.Data.Entities;


[Table("Media")]
public class Medium
{
    [Key]
    public Guid Id { get; set; }


    [StringLength(1024)]
    public string? BaseUrl { get; set; }


    [StringLength(128)]
    public string? FileName { get; set; }


    [StringLength(1152)]
    public string? CompleteUrl { get; set; }


    public bool IsHidden { get; set; }


    public DateTime CreatedOn { get; set; }


    [StringLength(256)]
    public string? CreatedBy { get; set; }


    public virtual ICollection<Author> Authors { get; set; } = new HashSet<Author>();


    public virtual ICollection<QuoteItem> QuoteItems { get; set; } = new HashSet<QuoteItem>();
}