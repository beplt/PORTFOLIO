using System.ComponentModel.DataAnnotations;

namespace portfolio.Models;

// One Course object holds one row of portfolio.course.
public class Course
{
    public long CourseId { get; set; }

    [StringLength(20, ErrorMessage = "The code can be at most 20 characters.")]
    [RegularExpression(@"^[A-Z]{3,10}[0-9]{0,3}$", ErrorMessage = "Use capital letters, then optional digits, such as DBMWEB1.")]
    public string Code { get; set; } = "";

    [StringLength(120, ErrorMessage = "The title can be at most 120 characters.")]
    public string Title { get; set; } = "";

    [StringLength(40, ErrorMessage = "The term can be at most 40 characters.")]
    public string? Term { get; set; }

    public string? Description { get; set; }
}