namespace Kwotty.Domain.Models; 

public class Category
{
    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string NameSlug { get; init; } = string.Empty;

    public bool IsHidden { get; init; }

    public DateTime CreatedOn { get; init; }

    public string CreatedBy { get; init; } = string.Empty;


    public List<User> Users = [];

    public List<Quote>
}
