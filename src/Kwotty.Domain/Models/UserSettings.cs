namespace Kwotty.Domain.Models;

/// <summary>
/// Represents settings specific to a User.
/// Closely tied to the User aggregate.
/// </summary>
public class UserSettings
{
    private readonly List<Category> _preferredCategories = new();


    public Guid UserId { get; }

    public bool IsPaused { get; private set; }

    public DateTimeOffset? LastPausedOnUtc { get; private set; }

    public DateTimeOffset? LastResumedOnUtc { get; private set; }

    public IReadOnlyList<Category> PreferredCategories => _preferredCategories.AsReadOnly();


    public UserSettings(Guid userId)
    {
        UserId = userId;
        IsPaused = false; // Default state
    }


    public void Pause()
    {
        if (!IsPaused)
        {
            IsPaused = true;
            LastPausedOnUtc = DateTimeOffset.UtcNow;
        }
    }

    public void Resume()
    {
        if (IsPaused)
        {
            IsPaused = false;
            LastResumedOnUtc = DateTimeOffset.UtcNow;
        }
    }

    public void UpdateCategories(IEnumerable<Category> categories)
    {
        // Replace entire list - simpler for mapping M-M
        _preferredCategories.Clear();
        _preferredCategories.AddRange(categories);
        // More complex logic could involve Add/Remove methods
    }

    // Used internally or by mappers to set initial state
    public void SetInitialCategories(IEnumerable<Category> categories)
    {
        _preferredCategories.Clear();
        _preferredCategories.AddRange(categories);
    }
}
