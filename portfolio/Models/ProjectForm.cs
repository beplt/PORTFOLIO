using System.ComponentModel.DataAnnotations;

namespace portfolio.Models;

// What the project form sends: a project plus its image URLs, one per line.
public class ProjectForm
{
    public long ProjectId { get; set; }

    public long CourseId { get; set; }

    [StringLength(120, ErrorMessage = "The title can be at most 120 characters.")]
    public string Title { get; set; } = "";

    [StringLength(300, ErrorMessage = "The summary can be at most 300 characters.")]
    public string? Summary { get; set; }

    public string? Description { get; set; }

    [StringLength(300, ErrorMessage = "The link can be at most 300 characters.")]
    [RegularExpression(@"^https?://\S+$", ErrorMessage = "The code link must start with http:// or https://.")]
    public string? RepoUrl { get; set; }

    [StringLength(300, ErrorMessage = "The link can be at most 300 characters.")]
    [RegularExpression(@"^https?://(www\.)?(youtube\.com/(watch\?v=|shorts/)|youtu\.be/)[A-Za-z0-9_-]{11}([&?]\S*)?$",
        ErrorMessage = "Paste a YouTube link, such as https://youtu.be/abcdefghijk.")]
    public string? VideoUrl { get; set; }

    public DateTime StartedOn { get; set; } = DateTime.Today;

    public DateTime? CompletedOn { get; set; }

    // One image URL per line.
    public string? ImageUrls { get; set; }

    // Turns the form into a Project, ready for the repository.
    public Project ToProject()
    {
        return new Project
        {
            ProjectId = ProjectId,
            CourseId = CourseId,
            Title = Title,
            Summary = Summary,
            Description = Description,
            RepoUrl = RepoUrl,
            VideoUrl = VideoUrl,
            StartedOn = StartedOn,
            CompletedOn = CompletedOn
        };
    }
}