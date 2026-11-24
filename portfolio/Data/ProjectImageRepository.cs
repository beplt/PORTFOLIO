using Npgsql;
using portfolio.Models;

namespace portfolio.Data;

// Every SQL statement about project images lives in this one class.
public class ProjectImageRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public ProjectImageRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    private const string Columns = "image_id, project_id, url, caption, sort_order";

    private static ProjectImage ReadImage(NpgsqlDataReader reader)
    {
        return new ProjectImage
        {
            ImageId = reader.GetInt64(0),
            ProjectId = reader.GetInt64(1),
            Url = reader.GetString(2),
            Caption = reader.IsDBNull(3) ? null : reader.GetString(3),
            SortOrder = reader.GetInt32(4)
        };
    }

    // READ: the images of one project, in their order.
    public async Task<List<ProjectImage>> GetByProjectAsync(long projectId)
    {
        const string sql = "SELECT " + Columns + " FROM portfolio.project_image " +
                           "WHERE project_id = @project_id ORDER BY sort_order, image_id;";

        var images = new List<ProjectImage>();
        await using var command = _dataSource.CreateCommand(sql);
        command.Parameters.AddWithValue("project_id", projectId);
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            images.Add(ReadImage(reader));
        }
        return images;
    }

    // READ: one image, or null when no image has that id.
    public async Task<ProjectImage?> GetByIdAsync(long id)
    {
        const string sql = "SELECT " + Columns + " FROM portfolio.project_image WHERE image_id = @id;";

        await using var command = _dataSource.CreateCommand(sql);
        command.Parameters.AddWithValue("id", id);
        await using var reader = await command.ExecuteReaderAsync();
        return await reader.ReadAsync() ? ReadImage(reader) : null;
    }

    // CREATE: insert one image.
    public async Task AddAsync(ProjectImage image)
    {
        const string sql = "INSERT INTO portfolio.project_image (project_id, url, caption, sort_order) " +
                           "VALUES (@project_id, @url, @caption, @sort_order);";

        await using var command = _dataSource.CreateCommand(sql);
        command.Parameters.AddWithValue("project_id", image.ProjectId);
        command.Parameters.AddWithValue("url", image.Url);
        command.Parameters.AddWithValue("caption", (object?)image.Caption ?? DBNull.Value);
        command.Parameters.AddWithValue("sort_order", image.SortOrder);
        await command.ExecuteNonQueryAsync();
    }

    // UPDATE: change the url, caption and order of one image.
    public async Task UpdateAsync(ProjectImage image)
    {
        const string sql = "UPDATE portfolio.project_image " +
                           "SET url = @url, caption = @caption, sort_order = @sort_order WHERE image_id = @id;";

        await using var command = _dataSource.CreateCommand(sql);
        command.Parameters.AddWithValue("url", image.Url);
        command.Parameters.AddWithValue("caption", (object?)image.Caption ?? DBNull.Value);
        command.Parameters.AddWithValue("sort_order", image.SortOrder);
        command.Parameters.AddWithValue("id", image.ImageId);
        await command.ExecuteNonQueryAsync();
    }

    // DELETE: remove one image.
    public async Task DeleteAsync(long id)
    {
        const string sql = "DELETE FROM portfolio.project_image WHERE image_id = @id;";

        await using var command = _dataSource.CreateCommand(sql);
        command.Parameters.AddWithValue("id", id);
        await command.ExecuteNonQueryAsync();
    }
}