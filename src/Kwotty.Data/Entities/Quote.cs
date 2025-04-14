using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Kwotty.Data.Entities;


[Table("Quotes")]
public class Quote
{
    [Key]
    public Guid Id { get; set; }


    public bool IsHidden { get; set; }


    [Required]
    public Guid AuthorId { get; set; }

    [Required]
    public DateTimeOffset CreatedOnUtc { get; set; }


    [StringLength(256)]
    public string? CreatedBy { get; set; }


    [ForeignKey(nameof(AuthorId))]
    public virtual Author? Author { get; set; }


    public virtual ICollection<QuoteItem> QuoteItems { get; set; } = new HashSet<QuoteItem>();


    public virtual ICollection<QuoteCategory> QuoteCategories { get; set; } = new HashSet<QuoteCategory>();


    public virtual ICollection<Rating> QuoteRatings { get; set; } = new HashSet<Rating>();
}