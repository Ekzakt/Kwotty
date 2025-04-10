using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Kwotty.Data.Entities;


[Table("QuoteItems")]
public class QuoteItem
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid QuoteId { get; set; }


    [Required]
    [Column(TypeName = "nvarchar(MAX)")]
    public string? Text { get; set; }


    [Required]
    [Column(TypeName = "nvarchar(MAX)")]
    public string? TextSlug { get; set; }


    public Guid? MediumId { get; set; }


    public int SortNumber { get; set; }


    [ForeignKey(nameof(QuoteId))]
    public virtual Quote? Quote { get; set; }


    [ForeignKey(nameof(MediumId))]
    public virtual Medium? Medium { get; set; }
}
