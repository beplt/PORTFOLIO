namespace portfolio.Models;

// A project, its course and its images, passed to one page.
public class ProjectDetails
{
    public Project Project { get; set; } = new();
    public Course Course { get; set; } = new();
    public List<ProjectImage> Images { get; set; } = new();
}