using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using portfolio.Data;
using portfolio.Models;

namespace portfolio.Controllers;

// Editor side: add, edit and delete courses.
[Authorize]
[Route("editor/courses/[action]/{id?}")]
public class EditorCoursesController : Controller
{
    private readonly CourseRepository _courses;

    public EditorCoursesController(CourseRepository courses)
    {
        _courses = courses;
    }

    // GET /editor/courses : the list with its buttons.
    [HttpGet("~/editor/courses")]
    public async Task<IActionResult> Index()
    {
        return View(await _courses.GetAllAsync());
    }

    // GET /editor/courses/Create : an empty form.
    public IActionResult Create()
    {
        return View(new Course());
    }

    // POST /editor/courses/Create : the form was submitted.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Course course)
    {
        if (!ModelState.IsValid)
        {
            return View(course);
        }

        try
        {
            await _courses.AddAsync(course);
        }
        catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            ModelState.AddModelError("Code", "Another course already uses this code.");
            return View(course);
        }

        return RedirectToAction(nameof(Index));
    }

    // GET /editor/courses/Edit/5 : the form, filled with course 5.
    public async Task<IActionResult> Edit(long id)
    {
        var course = await _courses.GetByIdAsync(id);
        if (course == null)
        {
            return NotFound();
        }
        return View(course);
    }

    // POST /editor/courses/Edit/5 : the changed form was submitted.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Course course)
    {
        if (!ModelState.IsValid)
        {
            return View(course);
        }

        try
        {
            await _courses.UpdateAsync(course);
        }
        catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            ModelState.AddModelError("Code", "Another course already uses this code.");
            return View(course);
        }

        return RedirectToAction(nameof(Index));
    }

    // GET /editor/courses/Delete/5 : ask "are you sure?"
    public async Task<IActionResult> Delete(long id)
    {
        var course = await _courses.GetByIdAsync(id);
        if (course == null)
        {
            return NotFound();
        }
        return View(course);
    }

    // POST /editor/courses/Delete/5 : the delete was confirmed.
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(long id)
    {
        try
        {
            await _courses.DeleteAsync(id);
        }
        catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.ForeignKeyViolation)
        {
            var course = await _courses.GetByIdAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            ViewBag.Error = "This course still has projects, so it cannot be deleted. Delete its projects first.";
            return View("Delete", course);
        }

        return RedirectToAction(nameof(Index));
    }
}