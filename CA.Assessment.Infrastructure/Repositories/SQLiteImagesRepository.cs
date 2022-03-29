using CA.Assessment.Application.Repositories;
using CA.Assessment.Domain.Anemic;
using CA.Assessment.Infrastructure.Images;
using CA.Assessment.Infrastructure.Mappers;
using CA.Assessment.Infrastructure.Rows;
using CA.Assessment.Store;
using Dapper;

namespace CA.Assessment.Infrastructure.Repositories;

internal sealed class SQLiteImagesRepository : IImagesRepository
{
    private readonly IDatabaseSession databaseSession;
    private readonly ImageStore imageStore;
    private readonly ImageRowsMapper imageRowsMapper;

    public SQLiteImagesRepository(
        IDatabaseSession databaseSession,
        ImageStore imageStore,
        ImageRowsMapper imageRowsMapper)
    {
        this.databaseSession = databaseSession ?? throw new ArgumentNullException(nameof(databaseSession));
        this.imageStore = imageStore ?? throw new ArgumentNullException(nameof(imageStore));
        this.imageRowsMapper = imageRowsMapper ?? throw new ArgumentNullException(nameof(imageRowsMapper));
    }

    public async Task SaveAsync(Image image)
    {
        if (image is null) throw new ArgumentNullException(nameof(image));

        if (databaseSession.Connection is null)
            throw new InvalidOperationException(
                "No connection in the database session. You must open a connection before calling repository methods");

        if (databaseSession.Transaction is null)
            throw new InvalidOperationException(
                "No transaction in the database session. You must start a transaction before calling repository methods");

        var query = @"
            INSERT INTO images(id, mime, name)
            VALUES (@ImageId, @Mime, @Name)
        ";

        await imageStore.SaveImageAsync(image.Identity, image.Content);

        var queryParams = new
        {
            ImageId = image.Identity.ToString(),
            Mime = image.Mime,
            Name = image.Name
        };

        await databaseSession.Connection.ExecuteAsync(query,
            param: queryParams,
            transaction: databaseSession.Transaction);
    }

    public async Task<Image?> GetAsync(Guid imageId)
    {
        if (databaseSession.Connection is null)
            throw new InvalidOperationException(
                "No connection in the database session. You must open a connection before calling repository methods");

        var query = @"
            SELECT id, name, mime
            FROM images
            WHERE id = @ImageId
        ";

        var queryParams = new
        {
            ImageId = imageId.ToString()
        };

        var imageRow = await databaseSession.Connection.QueryFirstOrDefaultAsync<ImageRow>(query,
            param: queryParams,
            transaction: databaseSession.Transaction);

        if (imageRow is null)
        {
            return null;
        }

        return await imageRowsMapper.MapOneAsync(imageRow);
    }
}
