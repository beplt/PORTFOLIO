using Microsoft.AspNetCore.Mvc;
using portfolio.Data;
using portfolio.Models;

namespace portfolio.Controllers;

// Viewer side: read-only page for one project.
public class ProjectsController : Controller
{
    private readonly ProjectRepository _projects;
    private readonly CourseRepository _courses;
    private readonly ProjectImageRepository _images;

    public ProjectsController(ProjectRepository projects, CourseRepository courses, ProjectImageRepository images)
    {
        _projects = projects;
        _courses = courses;
        _images = images;
    }

    // GET /Projects/Details/5 : one project, its course, images and video.
    public async Task<IActionResult> Details(long id)
    {
        var project = await _projects.GetByIdAsync(id);
        if (project == null)
        {
            return NotFound();
        }

        var details = new ProjectDetails
        {
            Project = project,
            Course = (await _courses.GetByIdAsync(project.CourseId))!,
            Images = await _images.GetByProjectAsync(id)
        };
        return View(details);
    }
}