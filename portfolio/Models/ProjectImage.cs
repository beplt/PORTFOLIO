namespace portfolio.Models;

// One ProjectImage object holds one row of portfolio.project_image.
public class ProjectImage
{
    public long ImageId { get; set; }             // image_id    BIGINT, the primary key
    public long ProjectId { get; set; }           // project_id  BIGINT NOT NULL, foreign key
    public string Url { get; set; } = "";         // url         VARCHAR(500) NOT NULL
    public string? Caption { get; set; }          // caption     VARCHAR(200), may be NULL
    public int SortOrder { get; set; }            // sort_order  INTEGER NOT NULL
}