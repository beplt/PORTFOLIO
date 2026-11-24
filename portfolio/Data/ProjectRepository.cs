using Npgsql;
using portfolio.Models;

namespace portfolio.Data;

// Every SQL statement about projects lives in this one class.
public class ProjectRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public ProjectRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    // The columns, in the order ReadProject reads them (positions 0 to 8).
    private const string Columns =
        "project_id, course_id, title, summary, description, repo_url, video_url, started_on, completed_on";

    private static Project ReadProject(NpgsqlDataReader reader)
    {
        return new Project
        {
            ProjectId = reader.GetInt64(0),
            CourseId = reader.GetInt64(1),
            Title = reader.GetString(2),
            Summary = reader.IsDBNull(3) ? null : reader.GetString(3),
            Description = reader.IsDBNull(4) ? null : reader.GetString(4),
            RepoUrl = reader.IsDBNull(5) ? null : reader.GetString(5),
            VideoUrl = reader.IsDBNull(6) ? null : reader.GetString(6),
            StartedOn = reader.GetDateTime(7),
            CompletedOn = reader.IsDBNull(8) ? null : reader.GetDateTime(8)
        };
    }

    // READ: the projects of one course, newest first.
    public async Task<List<Project>> GetByCourseAsync(long courseId)
    {
        const string sql = "SELECT " + Columns + " FROM portfolio.project " +
                           "WHERE course_id = @course_id ORDER BY started_on DESC;";

        var projects = new List<Project>();
        await using var command = _dataSource.CreateCommand(sql);
        command.Parameters.AddWithValue("course_id", courseId);
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            projects.Add(ReadProject(reader));
        }
        return projects;
    }

    // READ: one project, or null when no project has that id.
    public async Task<Project?> GetByIdAsync(long id)
    {
        const string sql = "SELECT " + Columns + " FROM portfolio.project WHERE project_id = @id;";

        await using var command = _dataSource.CreateCommand(sql);
        command.Parameters.AddWithValue("id", id);
        await using var reader = await command.ExecuteReaderAsync();
        return await reader.ReadAsync() ? ReadProject(reader) : null;
    }

    // CREATE: insert a project and give back the project_id the database chose.
    public async Task<long> AddAsync(Project project)
    {
        const string sql =
            "INSERT INTO portfolio.project (course_id, title, summary, description, repo_url, video_url, started_on, completed_on) " +
            "VALUES (@course_id, @title, @summary, @description, @repo_url, @video_url, @started_on, @completed_on) " +
            "RETURNING project_id;";

        await using var command = _dataSource.CreateCommand(sql);
        AddParameters(command, project);
        return (long)(await command.ExecuteScalarAsync())!;
    }

    // UPDATE: change every column of one project.
    public async Task UpdateAsync(Project project)
    {
        const string sql =
            "UPDATE portfolio.project SET course_id = @course_id, title = @title, summary = @summary, " +
            "description = @description, repo_url = @repo_url, video_url = @video_url, " +
            "started_on = @started_on, completed_on = @completed_on WHERE project_id = @id;";

        await using var command = _dataSource.CreateCommand(sql);
        AddParameters(command, project);
        command.Parameters.AddWithValue("id", project.ProjectId);
        await command.ExecuteNonQueryAsync();
    }

    // DELETE: remove one project. Its images go with it (ON DELETE CASCADE).
    public async Task DeleteAsync(long id)
    {
        const string sql = "DELETE FROM portfolio.project WHERE project_id = @id;";

        await using var command = _dataSource.CreateCommand(sql);
        command.Parameters.AddWithValue("id", id);
        await command.ExecuteNonQueryAsync();
    }

    // The eight parameters that INSERT and UPDATE share.
    private static void AddParameters(NpgsqlCommand command, Project project)
    {
        command.Parameters.AddWithValue("course_id", project.CourseId);
        command.Parameters.AddWithValue("title", project.Title);
        command.Parameters.AddWithValue("summary", (object?)project.Summary ?? DBNull.Value);
        command.Parameters.AddWithValue("description", (object?)project.Description ?? DBNull.Value);
        command.Parameters.AddWithValue("repo_url", (object?)project.RepoUrl ?? DBNull.Value);
        command.Parameters.AddWithValue("video_url", (object?)project.VideoUrl ?? DBNull.Value);
        command.Parameters.AddWithValue("started_on", DateOnly.FromDateTime(project.StartedOn));
        command.Parameters.AddWithValue("completed_on",
            project.CompletedOn.HasValue ? DateOnly.FromDateTime(project.CompletedOn.Value) : DBNull.Value);
    }
}