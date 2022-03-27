using CA.Assessment.Application.Repositories;
using CA.Assessment.Domain.Anemic;
using CA.Assessment.Infrastructure.Mappers;
using CA.Assessment.Infrastructure.Rows;
using CA.Assessment.Store;
using Dapper;

namespace CA.Assessment.Infrastructure.Repositories;

internal sealed class SQLiteBlogPostRepository : IBlogPostRepository
{
    private readonly IDatabaseSession databaseSession;
    private readonly BlogPostRowsMapper blogPostRowsMapper;

    public SQLiteBlogPostRepository(IDatabaseSession databaseSession, BlogPostRowsMapper blogPostRowsMapper)
    {
        this.databaseSession = databaseSession ?? throw new ArgumentNullException(nameof(databaseSession));
        this.blogPostRowsMapper = blogPostRowsMapper ?? throw new ArgumentNullException(nameof(blogPostRowsMapper));
    }

    public async Task SaveAsync(BlogPost blogPost)
    {
        if (blogPost is null) throw new ArgumentNullException(nameof(blogPost));

        if (databaseSession.Connection is null)
        {
            throw new InvalidOperationException(
                "No connection in the database session. You must open a connection before calling repository methods");
        }

        if (databaseSession.Transaction is null)
        {
            throw new InvalidOperationException(
                "No transaction in the database session. You must start a transaction before calling repository methods");
        }

        var insertBlogPostQuery = @"
            INSERT INTO blog_posts(id, author, title, content, image_id, category_id)
            VALUES (@Id, @Author, @Title, @Content, @ImageId, @CategoryId)
        ";

        var insertBlogPostParams = new
        {
            Id = blogPost.Identity.ToString(),
            blogPost.Author,
            blogPost.Title,
            blogPost.Content,
            ImageId = blogPost.Image.ToString(),
            CategoryId = blogPost.Category.ToString()
        };

        await databaseSession.Connection.ExecuteAsync(insertBlogPostQuery,
            param: insertBlogPostParams,
            transaction: databaseSession.Transaction);

        await SaveTagsToBlogPostAsync(blogPost);
    }

    public async Task<BlogPost?> GetAsync(Guid blogPostIdentity)
    {
        if (databaseSession.Connection is null)
        {
            throw new InvalidOperationException(
                "No connection in the database session. You must open a connection before calling repository methods");
        }

        var blogPostQuery = @"
            SELECT id, author, content, title, image_id AS ImageId, category_id AS CategoryId
            FROM blog_posts
            WHERE id = @BlogPostId
        ";

        var blogPostQueryParams = new
        {
            BlogPostId = blogPostIdentity.ToString()
        };

        var blogPostRow = await databaseSession.Connection.QueryFirstOrDefaultAsync<BlogPostRow>(blogPostQuery,
            param: blogPostQueryParams,
            transaction: databaseSession.Transaction);

        if (blogPostRow is null)
        {
            return null;
        }

        var blogPostTagsQuery = @"
            SELECT blog_post_id AS BlogPostId, tag_id AS TagId
            FROM blog_posts_to_tags
            WHERE blog_post_id = @BlogPostId
        ";

        var blogPostTagsQueryParams = new
        {
            BlogPostId = blogPostIdentity.ToString()
        };

        var blogPostTagRows = databaseSession.Connection.Query<BlogPostToTagRow>(blogPostTagsQuery,
            param: blogPostTagsQueryParams,
            transaction: databaseSession.Transaction);

        return blogPostRowsMapper.MapOne(blogPostRow, blogPostTagRows);
    }

    public async Task DeleteAsync(Guid blogPostIdentity)
    {
        if (databaseSession.Connection is null)
        {
            throw new InvalidOperationException(
                "No connection in the database session. You must open a connection before calling repository methods");
        }

        if (databaseSession.Transaction is null)
        {
            throw new InvalidOperationException(
                "No transaction in the database session. You must start a transaction before calling repository methods");
        }

        var deleteBlogPostQuery = @"
            DELETE FROM blog_posts
            WHERE id = @BlogPostId
        ";

        var deleteBlogPostQueryParams = new
        {
            BlogPostId = blogPostIdentity.ToString()
        };

        var deleteBlogPostsToTagsQuery = @"
            DELETE FROM blog_posts_to_tags
            WHERE blog_post_id = @BlogPostId
        ";

        var deleteBlogPostsToTagsQueryParams = new
        {
            BlogPostId = blogPostIdentity.ToString()
        };

        await databaseSession.Connection.ExecuteAsync(deleteBlogPostsToTagsQuery,
            param: deleteBlogPostsToTagsQueryParams,
            transaction: databaseSession.Transaction);

        await databaseSession.Connection.ExecuteAsync(deleteBlogPostQuery,
            param: deleteBlogPostQueryParams,
            transaction: databaseSession.Transaction);
    }

    public async Task UpdateAsync(BlogPost blogPost)
    {
        if (blogPost is null) throw new ArgumentNullException(nameof(blogPost));

        if (databaseSession.Connection is null)
        {
            throw new InvalidOperationException(
                "No connection in the database session. You must open a connection before calling repository methods");
        }

        if (databaseSession.Transaction is null)
        {
            throw new InvalidOperationException(
                "No transaction in the database session. You must start a transaction before calling repository methods");
        }

        //It's easier to delete all the N-to-N tuples from the table than trying to reconcile them without an ORM
        await DeleteTagsFromBlogPostAsync(blogPost);
        await SaveTagsToBlogPostAsync(blogPost);

        var query = @"
            UPDATE blog_posts
            SET title = @Title, content = @Content, author = @Author, category_id = @Category
            WHERE id = @BlogPostId
        ";

        var queryParams = new
        {
            BlogPostId = blogPost.Identity.ToString(),
            blogPost.Title,
            blogPost.Content,
            blogPost.Author,
            Category = blogPost.Category.ToString()
        };

        await databaseSession.Connection.ExecuteAsync(query,
            param: queryParams,
            transaction: databaseSession.Transaction);
    }

    private async Task DeleteTagsFromBlogPostAsync(BlogPost blogPost)
    {
        if (blogPost is null) throw new ArgumentNullException(nameof(blogPost));

        if (databaseSession.Connection is null)
        {
            throw new InvalidOperationException(
                "No connection in the database session. You must open a connection before calling repository methods");
        }

        if (databaseSession.Transaction is null)
        {
            throw new InvalidOperationException(
                "No transaction in the database session. You must start a transaction before calling repository methods");
        }

        var query = @"
            DELETE FROM blog_posts_to_tags
            WHERE blog_post_id = @BlogPostId
        ";

        var queryParams = new
        {
            BlogPostId = blogPost.Identity.ToString()
        };

        await databaseSession.Connection.ExecuteAsync(query,
            param: queryParams,
            transaction: databaseSession.Transaction);
    }

    private async Task SaveTagsToBlogPostAsync(BlogPost blogPost)
    {
        if (blogPost is null) throw new ArgumentNullException(nameof(blogPost));

        if (databaseSession.Connection is null)
        {
            throw new InvalidOperationException(
                "No connection in the database session. You must open a connection before calling repository methods");
        }

        if (databaseSession.Transaction is null)
        {
            throw new InvalidOperationException(
                "No transaction in the database session. You must start a transaction before calling repository methods");
        }

        var insertTagsQuery = @"
            INSERT INTO blog_posts_to_tags(blog_post_id, tag_id)
            VALUES (@BlogPostId, @TagId)
        ";

        var insertTagsParams = blogPost.Tags
            .Select(t => new { BlogPostId = blogPost.Identity.ToString(), TagId = t.ToString() })
            .ToList();

        await databaseSession.Connection.ExecuteAsync(insertTagsQuery,
            param: insertTagsParams,
            transaction: databaseSession.Transaction);
    }
}
