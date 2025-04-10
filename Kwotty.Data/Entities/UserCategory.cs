using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kwotty.Data.Entities;


[Table("UsersCategories")]
public class UserCategory
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    public Guid CategoryId { get; set; }


    [ForeignKey(nameof(UserId))]
    public virtual UserSettings? UserSettings { get; set; }


    [ForeignKey(nameof(CategoryId))]
    public virtual Category? Category { get; set; }
}