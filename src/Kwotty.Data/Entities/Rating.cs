using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kwotty.Data.Entities;


[Table("Ratings")]
public class Rating
{
    [Key]
    public long Id { get; set; }


    [Required]
    public Guid QuoteId { get; set; }


    [Required]
    public Guid UserId { get; set; }


    [Required]
    public byte Value { get; set; }


    public DateTimeOffset RatedOnUtc  { get; set; }


    [ForeignKey(nameof(QuoteId))]
    public virtual Quote? Quote { get; set; }


    [ForeignKey(nameof(UserId))]
    public virtual User? User { get; set; }
}
