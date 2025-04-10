using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Kwotty.Data.Entities;


[Table("Users")]
public class User
{
    [Key]
    public Guid Id { get; set; }


    [StringLength(256)]
    [EmailAddress]
    public string? Email { get; set; }


    [StringLength(64)]
    public string? Firstname { get; set; }


    public bool IsDisabled { get; set; }


    public virtual UserSettings? UserSettings { get; set; }


    public virtual ICollection<Rating> QuoteRatings { get; set; } = new HashSet<Rating>();
}