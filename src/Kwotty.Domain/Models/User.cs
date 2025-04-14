namespace Kwotty.Domain.Models;

/// <summary>
/// Represents a user within the application domain.
/// </summary>
public class User
{
    private readonly List<Rating> _ratings = new();


    public Guid Id { get; }

    public string? Email { get; private set; }

    public string? Firstname { get; private set; }

    public bool IsDisabled { get; private set; }

    public UserSettings? Settings { get; internal set; }

    public IReadOnlyList<Rating> GivenRatings => _ratings.AsReadOnly();


    public User(Guid id, string? email, string? firstname, bool isDisabled = false)
    {
        Id = id;
        Email = email; // Consider validation
        Firstname = firstname;
        IsDisabled = isDisabled;
    }


    public void UpdateProfile(string email, string firstname)
    {
        // Add validation logic here
        Email = email;
        Firstname = firstname;
    }

    public void DisableAccount()
    {
        if (!IsDisabled)
        {
            IsDisabled = true;
            // Raise domain event? e.g., UserDisabledEvent(this.Id)
        }
    }

    public void EnableAccount()
    {
        if (IsDisabled)
        {
            IsDisabled = false;
            // Raise domain event? e.g., UserEnabledEvent(this.Id)
        }
    }

    
}
