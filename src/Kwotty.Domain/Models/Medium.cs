namespace Kwotty.Domain.Models;

/// <summary>
/// Represents a media item (e.g., image, video file).
/// </summary>
public class Medium
{
    public Guid Id { get; }

    public string? BaseUrl { get; private set; }

    public string? FileName { get; private set; }

    public string? CompleteUrl { get; private set; }

    public bool IsHidden { get; private set; }

    public DateTimeOffset CreatedOnUtc { get; }

    public string? CreatedBy { get; }

    
    public Medium(Guid id, string? baseUrl, string? fileName, string? completeUrl, bool isHidden, DateTimeOffset createdOnUtc, string? createdBy)
    {
        Id = id;
        BaseUrl = baseUrl;
        FileName = fileName;
        CompleteUrl = completeUrl ?? GenerateCompleteUrl(baseUrl, fileName); // Generate if not provided
        IsHidden = isHidden;
        CreatedOnUtc = createdOnUtc;
        CreatedBy = createdBy;
    }


    public void UpdateDetails(string? baseUrl, string? fileName, bool isHidden)
    {
        BaseUrl = baseUrl;
        FileName = fileName;
        CompleteUrl = GenerateCompleteUrl(baseUrl, fileName);
        IsHidden = isHidden;
    }

    public void Hide() => IsHidden = true;

    public void Show() => IsHidden = false;



    #region Helpers

    private static string? GenerateCompleteUrl(string? baseUrl, string? fileName)
    {
        if (string.IsNullOrWhiteSpace(baseUrl) || string.IsNullOrWhiteSpace(fileName))
        {
            return null;
        }

        return $"{baseUrl.TrimEnd('/')}/{fileName.TrimStart('/')}";
    }

    #endregion Helpers
}