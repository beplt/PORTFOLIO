using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Npgsql;
using portfolio.Data;
using portfolio.Models;

namespace portfolio.Controllers;

// Editor side: add, edit and delete projects, with their images.
[Route("editor/projects/[action]/{id?}")]
public class EditorProjectsController : Controller
{
    // Regex rule 2: each image URL must be an http or https link with no spaces.
    private const string ImageUrlPattern = @"^https?://\S+$";

    private readonly ProjectRepository _projects;
    private readonly CourseRepository _courses;
    private readonly ProjectImageRepository _images;

    public EditorProjectsController(ProjectRepository projects, CourseRepository courses, ProjectImageRepository images)
    {
        _projects = projects;
        _courses = courses;
        _images = images;
    }

    // GET /editor/projects : the list with its buttons.
    [HttpGet("~/editor/projects")]
    public async Task<IActionResult> Index()
    {
        var courses = await _courses.GetAllAsync();
        ViewBag.CourseCodes = courses.ToDictionary(c => c.CourseId, c => c.Code);
        return View(await _projects.GetAllAsync());
    }

    // GET /editor/projects/Create : an empty form.
    public async Task<IActionResult> Create()
    {
        await FillCoursesAsync();
        return View(new ProjectForm());
    }

    // POST /editor/projects/Create : the form was submitted.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProjectForm form)
    {
        var urls = SplitLines(form.ImageUrls);
        CheckImageUrls(urls);
        if (!ModelState.IsValid)
        {
            await FillCoursesAsync();
            return View(form);
        }

        try
        {
            await _projects.AddWithImagesAsync(form.ToProject(), urls);
            return RedirectToAction(nameof(Index));
        }
        catch (PostgresException ex) when (IsKnownRefusal(ex))
        {
            AddRefusalMessage(ex);
        }

        await FillCoursesAsync();
        return View(form);
    }

    // GET /editor/projects/Edit/5 : the form, filled with project 5 and its images.
    public async Task<IActionResult> Edit(long id)
    {
        var project = await _projects.GetByIdAsync(id);
        if (project == null)
        {
            return NotFound();
        }

        var images = await _images.GetByProjectAsync(id);
        var form = new ProjectForm
        {
            ProjectId = project.ProjectId,
            CourseId = project.CourseId,
            Title = project.Title,
            Summary = project.Summary,
            Description = project.Description,
            RepoUrl = project.RepoUrl,
            VideoUrl = project.VideoUrl,
            StartedOn = project.StartedOn,
            CompletedOn = project.CompletedOn,
            ImageUrls = string.Join(Environment.NewLine, images.Select(i => i.Url))
        };

        await FillCoursesAsync();
        return View(form);
    }

    // POST /editor/projects/Edit/5 : the changed form was submitted.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ProjectForm form)
    {
        var urls = SplitLines(form.ImageUrls);
        CheckImageUrls(urls);
        if (!ModelState.IsValid)
        {
            await FillCoursesAsync();
            return View(form);
        }

        try
        {
            await _projects.UpdateWithImagesAsync(form.ToProject(), urls);
            return RedirectToAction(nameof(Index));
        }
        catch (PostgresException ex) when (IsKnownRefusal(ex))
        {
            AddRefusalMessage(ex);
        }

        await FillCoursesAsync();
        return View(form);
    }

    // GET /editor/projects/Delete/5 : ask "are you sure?"
    public async Task<IActionResult> Delete(long id)
    {
        var project = await _projects.GetByIdAsync(id);
        if (project == null)
        {
            return NotFound();
        }
        return View(project);
    }

    // POST /editor/projects/Delete/5 : the delete was confirmed. Its images go with it.
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(long id)
    {
        await _projects.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }

    // Splits the text box into lines, trimmed, with blank lines dropped.
    private static List<string> SplitLines(string? text)
    {
        return (text ?? "")
            .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList();
    }

    // Checks every image URL, line by line, with Regex.IsMatch.
    private void CheckImageUrls(List<string> urls)
    {
        for (var i = 0; i < urls.Count; i++)
        {
            if (!Regex.IsMatch(urls[i], ImageUrlPattern))
            {
                ModelState.AddModelError("ImageUrls", $"Line {i + 1} is not a valid image link. It must start with http:// or https://.");
            }
        }
    }

    // The three refusals the database can give for a project.
    private static bool IsKnownRefusal(PostgresException ex)
    {
        return ex.SqlState == PostgresErrorCodes.UniqueViolation
            || ex.SqlState == PostgresErrorCodes.CheckViolation
            || ex.SqlState == PostgresErrorCodes.ForeignKeyViolation;
    }

    private void AddRefusalMessage(PostgresException ex)
    {
        if (ex.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            ModelState.AddModelError("ImageUrls", "The same image link appears more than once. Nothing was saved.");
        }
        else if (ex.SqlState == PostgresErrorCodes.CheckViolation)
        {
            ModelState.AddModelError("CompletedOn", "A rule was broken: the completed date cannot be before the start date. Nothing was saved.");
        }
        else
        {
            ModelState.AddModelError("CourseId", "Choose a course from the list. Nothing was saved.");
        }
    }

    // Fills the course dropdown.
    private async Task FillCoursesAsync()
    {
        var courses = await _courses.GetAllAsync();
        ViewBag.Courses = new SelectList(courses, "CourseId", "Title");
    }
}