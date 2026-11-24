namespace portfolio.Models;

// A course and its projects, passed to one page.
public class CourseDetails
{
    public Course Course { get; set; } = new();
    public List<Project> Projects { get; set; } = new();
}