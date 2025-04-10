using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Kwotty.Data.Entities;


[Table("Categories")]
public class Category
{
    [Key]
    public Guid Id { get; set; }


    [Required]
    [StringLength(256)]
    public string Name { get; set; } = string.Empty;


    [StringLength(256)]
    [Column(TypeName = "nvarchar(256)")]
    public string? NameSlug { get; set; }


    public bool IsHidden { get; set; }


    [Required]
    public DateTime CreatedAt { get; set; }


    [StringLength(256)]
    public string? CreatedBy { get; set; }


    public virtual ICollection<UserCategory> UserCategories { get; set; } = new HashSet<UserCategory>();


    public virtual ICollection<QuoteCategory> QuoteCategories { get; set; } = new HashSet<QuoteCategory>();
}