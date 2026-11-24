namespace portfolio.Models;

// One Project object holds one row of portfolio.project.
public class Project
{
    public long ProjectId { get; set; }           // project_id    BIGINT, the primary key
    public long CourseId { get; set; }            // course_id     BIGINT NOT NULL, foreign key
    public string Title { get; set; } = "";       // title         VARCHAR(120) NOT NULL
    public string? Summary { get; set; }          // summary       VARCHAR(300), may be NULL
    public string? Description { get; set; }      // description   TEXT, may be NULL
    public string? RepoUrl { get; set; }          // repo_url      VARCHAR(300), may be NULL
    public string? VideoUrl { get; set; }         // video_url     VARCHAR(300), may be NULL
    public DateTime StartedOn { get; set; } = DateTime.Today;   // started_on    DATE NOT NULL
    public DateTime? CompletedOn { get; set; }    // completed_on  DATE, may be NULL
}