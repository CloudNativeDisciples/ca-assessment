using System.Globalization;
using CA.Assessment.Application.Repositories;
using CA.Assessment.Database.Sqlite.Mappers;
using CA.Assessment.Database.Sqlite.Rows;
using CA.Assessment.Domain.Anemic;
using CA.Assessment.Store;
using Dapper;

namespace CA.Assessment.Database.Sqlite.Repositories;

internal sealed class SqliteImagesRepository : IImagesRepository
{
    private readonly IDatabaseSession _databaseSession;
    private readonly ImageRowsMapper _imageRowsMapper;

    public SqliteImagesRepository(IDatabaseSession databaseSession, ImageRowsMapper imageRowsMapper)
    {
        _databaseSession = databaseSession ?? throw new ArgumentNullException(nameof(databaseSession));
        _imageRowsMapper = imageRowsMapper ?? throw new ArgumentNullException(nameof(imageRowsMapper));
    }

    public async Task SaveAsync(Image image)
    {
        if (image is null)
        {
            throw new ArgumentNullException(nameof(image));
        }

        if (_databaseSession.Connection is null)
        {
            throw new InvalidOperationException(
                "No connection in the database session. You must open a connection before calling repository methods");
        }

        if (_databaseSession.Transaction is null)
        {
            throw new InvalidOperationException(
                "No transaction in the database session. You must start a transaction before calling repository methods");
        }

        var query = @"
            INSERT INTO images(id, mime, name)
            VALUES (@ImageId, @Mime, @Name)
        ";

        var imageId = image.Identity.ToString("D", CultureInfo.InvariantCulture);

        var queryParams = new
        {
            ImageId = imageId,
            image.Mime,
            image.Name
        };

        await _databaseSession.Connection.ExecuteAsync(query,
            queryParams,
            _databaseSession.Transaction);
    }

    public async Task<Image?> GetAsync(Guid imageId)
    {
        if (_databaseSession.Connection is null)
        {
            throw new InvalidOperationException(
                "No connection in the database session. You must open a connection before calling repository methods");
        }

        var query = @"
            SELECT id, name, mime
            FROM images
            WHERE id = @ImageId
        ";

        var strImageId = imageId.ToString("D", CultureInfo.InvariantCulture);

        var queryParams = new
        {
            ImageId = strImageId
        };

        var imageRow = await _databaseSession.Connection.QueryFirstOrDefaultAsync<ImageRow>(query,
            queryParams,
            _databaseSession.Transaction);

        if (imageRow is null)
        {
            return null;
        }

        return await _imageRowsMapper.MapOneAsync(imageRow);
    }
}