namespace portfolio.Models;

// One Course object holds one row of portfolio.course.
public class Course
{
    public long CourseId { get; set; }            // course_id    BIGINT, the primary key
    public string Code { get; set; } = "";        // code         VARCHAR(20) NOT NULL, unique
    public string Title { get; set; } = "";       // title        VARCHAR(120) NOT NULL
    public string? Term { get; set; }             // term         VARCHAR(40), may be NULL
    public string? Description { get; set; }      // description  TEXT, may be NULL
}