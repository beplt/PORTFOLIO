using Microsoft.AspNetCore.Mvc;
using portfolio.Data;
using portfolio.Models;

namespace portfolio.Controllers;

// Viewer side: read-only pages for courses.
public class CoursesController : Controller
{
    private readonly CourseRepository _courses;
    private readonly ProjectRepository _projects;

    public CoursesController(CourseRepository courses, ProjectRepository projects)
    {
        _courses = courses;
        _projects = projects;
    }

    // GET /Courses : every course.
    public async Task<IActionResult> Index()
    {
        return View(await _courses.GetAllAsync());
    }

    // GET /Courses/Details/5 : one course and its projects.
    public async Task<IActionResult> Details(long id)
    {
        var course = await _courses.GetByIdAsync(id);
        if (course == null)
        {
            return NotFound();
        }

        var details = new CourseDetails
        {
            Course = course,
            Projects = await _projects.GetByCourseAsync(id)
        };
        return View(details);
    }
}