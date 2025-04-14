namespace Kwotty.Domain.Models;

/// <summary>
/// Represents a quote, potentially composed of multiple items.
/// Acts as an Aggregate Root.
/// </summary>
public class Quote
{
    private readonly List<QuoteItem> _items = [];

	private readonly List<Category> _categories = [];

	private readonly List<Rating> _ratings = [];


	public Guid Id { get; }

    public bool IsHidden { get; private set; }

    public DateTimeOffset CreatedOnUtc { get; }

    public string? CreatedBy { get; }

    public Author Author { get; private set; }

    public IReadOnlyList<QuoteItem> Items => _items.AsReadOnly();

    public IReadOnlyList<Category> Categories => _categories.AsReadOnly();

    public IReadOnlyList<Rating> Ratings => _ratings.AsReadOnly();


    public Quote(Guid id, Author author, bool isHidden, DateTimeOffset createdOnUtc, string? createdBy, IEnumerable<QuoteItem>? initialItems = null)
    {
        Id = id;
        Author = author ?? throw new ArgumentNullException(nameof(author));
        IsHidden = isHidden;
        CreatedOnUtc = createdOnUtc;
        CreatedBy = createdBy;

        if (initialItems != null)
        {
            _items.AddRange(initialItems);
        }
    }

	public void Hide() => IsHidden = true;

	public void Show() => IsHidden = false;

	public void ChangeAuthor(Author newAuthor)
    {
        // Author oldAuthor = Author;
        Author = newAuthor ?? throw new ArgumentNullException(nameof(newAuthor));
        // oldAuthor.RemoveQuoteReference(this.Id); // If managing bidirectional
        // newAuthor.AddQuoteReference(this);
    }

    public void UpdateCategories(IEnumerable<Category> categories)
    {
        _categories.Clear();
        _categories.AddRange(categories);
    }


    public void AddItem(Guid itemId, string? text, string? textSlug, int sortNumber, Medium? associatedMedium = null)
    {
        // Add validation (e.g., unique sortNumber?)
        _items.Add(new QuoteItem(itemId, text, textSlug, sortNumber, associatedMedium));
        _items.Sort((x, y) => x.SortNumber.CompareTo(y.SortNumber)); // Maintain sort order
    }


    public void RemoveItem(Guid itemId)
    {
        _items.RemoveAll(item => item.Id == itemId);
    }


    public void AddOrUpdateRating(User user, byte value, DateTimeOffset ratedOn)
    {
        ArgumentNullException.ThrowIfNull(user);

        var existingRating = _ratings.FirstOrDefault(r => r.UserId == user.Id);

        if (existingRating != null)
        {
            existingRating.UpdateRating(value, ratedOn);
        }
        else
        {
            _ratings.Add(new Rating(this.Id, user.Id, value, ratedOn));
        }
    }

    public void RemoveRating(User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        _ratings.RemoveAll(r => r.UserId == user.Id);
    }

	public void SetInitialCategories(IEnumerable<Category> categories)
	{
		_categories.Clear();
		_categories.AddRange(categories);
	}


	public void SetInitialRatings(IEnumerable<Rating> ratings)
    {
        _ratings.Clear();
        _ratings.AddRange(ratings);
    }
}