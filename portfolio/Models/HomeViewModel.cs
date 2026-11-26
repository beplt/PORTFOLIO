namespace portfolio.Models;

// One project shown on the home page, with its course code and first image.
public class FeaturedProject
{
    public Project Project { get; set; } = new();
    public string CourseCode { get; set; } = "";
    public string? CoverUrl { get; set; }
}

// Everything the home page shows.
public class HomeViewModel
{
    public List<Course> Courses { get; set; } = new();
    public int ProjectCount { get; set; }
    public List<FeaturedProject> Featured { get; set; } = new();
}