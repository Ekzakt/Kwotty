using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kwotty.Data.Entities;


[Table("QuotesCategories")]
public class QuoteCategory
{
    [Required]
    public Guid QuoteId { get; set; }


    [Required]
    public Guid CategoryId { get; set; }


    [ForeignKey(nameof(QuoteId))]
    public virtual Quote? Quote { get; set; }


    [ForeignKey(nameof(CategoryId))]
    public virtual Category? Category { get; set; }
}