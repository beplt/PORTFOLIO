using Microsoft.AspNetCore.Mvc;
using portfolio.Data;
using portfolio.Models;

namespace portfolio.Controllers;

// The home page: a welcome, the courses and the latest projects.
public class HomeController : Controller
{
    private readonly CourseRepository _courses;
    private readonly ProjectRepository _projects;
    private readonly ProjectImageRepository _images;

    public HomeController(CourseRepository courses, ProjectRepository projects, ProjectImageRepository images)
    {
        _courses = courses;
        _projects = projects;
        _images = images;
    }

    // GET / : the home page.
    public async Task<IActionResult> Index()
    {
        var courses = await _courses.GetAllAsync();
        var projects = await _projects.GetAllAsync();
        var codes = courses.ToDictionary(c => c.CourseId, c => c.Code);

        // The three newest projects, each with its first image as a cover.
        var featured = new List<FeaturedProject>();
        foreach (var project in projects.Take(3))
        {
            var images = await _images.GetByProjectAsync(project.ProjectId);
            featured.Add(new FeaturedProject
            {
                Project = project,
                CourseCode = codes.GetValueOrDefault(project.CourseId, ""),
                CoverUrl = images.FirstOrDefault()?.Url
            });
        }

        return View(new HomeViewModel
        {
            Courses = courses,
            ProjectCount = projects.Count,
            Featured = featured
        });
    }
}