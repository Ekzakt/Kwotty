using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Kwotty.Data.Entities;


[Table("Authors")]
public class Author
{
    [Key]
    public Guid Id { get; set; }


    [Required]
    [StringLength(256)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(256)]
    public string? Slug { get; set; }


    public Guid? MediumId { get; set; }


    public bool IsHidden { get; set; }


    [Column("CreateOn")]
    public DateTime CreatedOn { get; set; }


    [StringLength(256)]
    public string? CreatedBy { get; set; }


    [ForeignKey(nameof(MediumId))]
    public virtual Medium? Medium { get; set; }


    public virtual ICollection<Quote> Quotes { get; set; } = new HashSet<Quote>();
}