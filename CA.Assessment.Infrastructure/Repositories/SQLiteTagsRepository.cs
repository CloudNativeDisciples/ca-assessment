using CA.Assessment.Application.Repositories;
using CA.Assessment.Domain.Anemic;
using CA.Assessment.Infrastructure.Mappers;
using CA.Assessment.Infrastructure.Rows;
using CA.Assessment.Store;
using Dapper;

namespace CA.Assessment.Infrastructure.Repositories;

internal sealed class SQLiteTagsRepository : ITagsRepository
{
    private readonly IDatabaseSession databaseSession;
    private readonly TagRowsMapper tagRowsMapper;

    public SQLiteTagsRepository(IDatabaseSession databaseSession, TagRowsMapper tagRowsMapper)
    {
        this.databaseSession = databaseSession ?? throw new ArgumentNullException(nameof(databaseSession));
        this.tagRowsMapper = tagRowsMapper ?? throw new ArgumentNullException(nameof(tagRowsMapper));
    }

    public async Task SaveAsync(Tag tagToSave)
    {
        if (tagToSave is null) throw new ArgumentNullException(nameof(tagToSave));

        if (databaseSession.Connection is null)
            throw new InvalidOperationException(
                "No connection in the database session. You must open a connection before calling repository methods");

        if (databaseSession.Transaction is null)
            throw new InvalidOperationException(
                "No transaction in the database session. You must start a transaction before calling repository methods");

        var query = @"
            INSERT INTO tags(id, name)
            VALUES (@Id, @Name)
        ";

        var queryParams = new
        {
            Id = tagToSave.Identity.ToString(),
            tagToSave.Name
        };

        await databaseSession.Connection.ExecuteAsync(query,
            queryParams,
            databaseSession.Transaction);
    }

    public async Task<IEnumerable<Tag>> GetTagsByNameAsync(IEnumerable<string> tagNames)
    {
        if (databaseSession.Connection is null)
            throw new InvalidOperationException(
                "You must open a connection in the current database session before calling repository methods");

        var query = @"
            SELECT id, name
            FROM tags
            WHERE name IN @TagNames
        ";

        var queryParams = new
        {
            TagNames = tagNames
        };

        var tagRows = await databaseSession.Connection.QueryAsync<TagRow>(query,
            queryParams,
            databaseSession.Transaction);

        return tagRowsMapper.MapMany(tagRows);
    }

    public async Task<IEnumerable<Tag>> GetManyAsync(IEnumerable<Guid> tagIds)
    {
        if (databaseSession.Connection is null)
            throw new InvalidOperationException(
                "You must open a connection in the current database session before calling repository methods");

        var query = @"
            SELECT id, name
            FROM tags
            WHERE id IN @TagIds
        ";

        var queryParams = new
        {
            TagIds = tagIds.Select(t => t.ToString()).ToList()
        };

        var tagRows = await databaseSession.Connection.QueryAsync<TagRow>(query,
            queryParams,
            databaseSession.Transaction);

        return tagRowsMapper.MapMany(tagRows);
    }

    public async Task AddTagsToBlogPostAsync(Guid blogPostId, IEnumerable<Tag> tagsToAdd)
    {
        if (tagsToAdd is null) throw new ArgumentNullException(nameof(tagsToAdd));

        if (databaseSession.Connection is null)
            throw new InvalidOperationException(
                "No connection in the database session. You must open a connection before calling repository methods");

        if (databaseSession.Transaction is null)
            throw new InvalidOperationException(
                "No transaction in the database session. You must start a transaction before calling repository methods");

        var query = @"
            INSERT INTO blog_posts_to_tags(blog_post_id, tag_id)
            VALUES (@BlogPostId, @TagId)
        ";

        var queryParams =
            tagsToAdd.Select(t => new { TagId = t.Identity.ToString(), BlogPostId = blogPostId.ToString() })
                .ToList();

        await databaseSession.Connection.ExecuteAsync(query,
            param: queryParams,
            transaction: databaseSession.Transaction);
    }

    public async Task RemoveTagsToBlogPostAsync(Guid blogPostId, IEnumerable<Tag> tagsToRemove)
    {
        if (tagsToRemove is null) throw new ArgumentNullException(nameof(tagsToRemove));

        if (databaseSession.Connection is null)
            throw new InvalidOperationException(
                "No connection in the database session. You must open a connection before calling repository methods");

        if (databaseSession.Transaction is null)
            throw new InvalidOperationException(
                "No transaction in the database session. You must start a transaction before calling repository methods");

        var query = @"
            DELETE FROM blog_posts_to_tags
            WHERE blog_post_id = @BlogPostId AND tag_id = @TagId
        ";

        var queryParams =
            tagsToRemove.Select(t => new { TagId = t.Identity.ToString(), BlogPostId = blogPostId.ToString() })
                .ToList();

        await databaseSession.Connection.ExecuteAsync(query,
            param: queryParams,
            transaction: databaseSession.Transaction);
    }
}
