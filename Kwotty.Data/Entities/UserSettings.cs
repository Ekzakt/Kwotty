using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Kwotty.Data.Entities;


[Table("UserSettings")]
public class UserSettings
{
    [Key]
    public Guid UserId { get; set; }

    public bool IsPauzed { get; set; }


    public DateTime? LastPausedOn { get; set; }


    public DateTime? LastResumedOn { get; set; }


    public virtual User? User { get; set; }


    public virtual ICollection<UserCategory> UserCategories { get; set; } = new HashSet<UserCategory>();
}