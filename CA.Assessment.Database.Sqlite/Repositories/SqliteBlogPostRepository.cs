using CA.Assessment.Application.Repositories;
using CA.Assessment.Application.Responses;
using CA.Assessment.Database.Sqlite.Mappers;
using CA.Assessment.Database.Sqlite.Rows;
using CA.Assessment.Model;
using CA.Assessment.Store;
using Dapper;

namespace CA.Assessment.Database.Sqlite.Repositories;

internal sealed class SqliteBlogPostRepository : IBlogPostRepository
{
    private readonly IDatabaseSession _databaseSession;

    public SqliteBlogPostRepository(IDatabaseSession databaseSession)
    {
        _databaseSession = databaseSession ?? throw new ArgumentNullException(nameof(databaseSession));
    }

    public async Task SaveAsync(BlogPost blogPost)
    {
        if (blogPost is null)
        {
            throw new ArgumentNullException(nameof(blogPost));
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

        await _databaseSession.Connection.ExecuteAsync(insertBlogPostQuery,
            insertBlogPostParams,
            _databaseSession.Transaction);

        await SaveTagsToBlogPostAsync(blogPost);
    }

    public async Task<BlogPost?> GetAsync(Guid blogPostIdentity)
    {
        if (_databaseSession.Connection is null)
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

        var blogPostRow = await _databaseSession.Connection.QueryFirstOrDefaultAsync<BlogPostDbRow>(blogPostQuery,
            blogPostQueryParams,
            _databaseSession.Transaction);

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

        var blogPostTagRows = _databaseSession.Connection.Query<BlogPostToTagDbRow>(blogPostTagsQuery,
            blogPostTagsQueryParams,
            _databaseSession.Transaction);

        return BlogPostRowsMapper.MapOne(blogPostRow, blogPostTagRows);
    }

    public async Task DeleteAsync(Guid blogPostIdentity)
    {
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

        await _databaseSession.Connection.ExecuteAsync(deleteBlogPostsToTagsQuery,
            deleteBlogPostsToTagsQueryParams,
            _databaseSession.Transaction);

        await _databaseSession.Connection.ExecuteAsync(deleteBlogPostQuery,
            deleteBlogPostQueryParams,
            _databaseSession.Transaction);
    }

    public async Task UpdateAsync(BlogPost blogPost)
    {
        if (blogPost is null)
        {
            throw new ArgumentNullException(nameof(blogPost));
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

        //It's easier to delete all the N-to-N tuples from the table than trying to reconcile them without an ORM
        await DeleteTagsFromBlogPostAsync(blogPost);
        await SaveTagsToBlogPostAsync(blogPost);

        var query = @"
            UPDATE blog_posts
            SET title = @Title, content = @Content, author = @Author, category_id = @Category, image_id = @Image
            WHERE id = @BlogPostId
        ";

        var queryParams = new
        {
            BlogPostId = blogPost.Identity.ToString(),
            blogPost.Title,
            blogPost.Content,
            blogPost.Author,
            Category = blogPost.Category.ToString(),
            Image = blogPost.Image.ToString()
        };

        await _databaseSession.Connection.ExecuteAsync(query,
            queryParams,
            _databaseSession.Transaction);
    }

    public async Task<IEnumerable<BlogPostSummary>> SearchAsync(
        string? filtersTitle,
        string? filtersCategory,
        IEnumerable<string>? filtersTags)
    {
        if (_databaseSession.Connection is null)
        {
            throw new InvalidOperationException(
                "No connection in the database session. You must open a connection before calling repository methods");
        }

        var searchQuery = @"
            SELECT blog_posts.id, blog_posts.title
            FROM blog_posts
            INNER JOIN categories ON blog_posts.category_id = categories.id 
            WHERE (@Title IS NULL OR blog_posts.title LIKE @Title)
            AND (@Category IS NULL OR categories.name LIKE @Category)
            AND (
                @Tags IS NULL OR EXISTS (
            	SELECT 1 
            	FROM blog_posts_to_tags
            	WHERE blog_posts_to_tags.blog_post_id = blog_posts.id
            	AND blog_posts_to_tags.tag_id IN (
            		SELECT tags.id
            		FROM tags
            		WHERE tags.name LIKE @Tags
        	        )
                )
            )
        ";

        var queryParams = new
        {
            Title = filtersTitle is not null ? $"%{filtersTitle}%" : null,
            Category = filtersCategory is not null ? $"%{filtersCategory}%" : null,
            Tags = filtersTags?.Select(tags => $"%{tags}%").ToList()
        };

        var blogPostRows = await _databaseSession.Connection.QueryAsync<BlogPostSummaryDbRow>(searchQuery,
            queryParams,
            _databaseSession.Transaction);

        return BlogPostSummaryRowMapper.MapMany(blogPostRows);
    }

    private async Task DeleteTagsFromBlogPostAsync(BlogPost blogPost)
    {
        if (blogPost is null)
        {
            throw new ArgumentNullException(nameof(blogPost));
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
            DELETE FROM blog_posts_to_tags
            WHERE blog_post_id = @BlogPostId
        ";

        var queryParams = new
        {
            BlogPostId = blogPost.Identity.ToString()
        };

        await _databaseSession.Connection.ExecuteAsync(query,
            queryParams,
            _databaseSession.Transaction);
    }

    private async Task SaveTagsToBlogPostAsync(BlogPost blogPost)
    {
        if (blogPost is null)
        {
            throw new ArgumentNullException(nameof(blogPost));
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

        var insertTagsQuery = @"
            INSERT INTO blog_posts_to_tags(blog_post_id, tag_id)
            VALUES (@BlogPostId, @TagId)
        ";

        var insertTagsParams = blogPost.Tags
            .Select(t => new { BlogPostId = blogPost.Identity.ToString(), TagId = t.ToString() })
            .ToList();

        await _databaseSession.Connection.ExecuteAsync(insertTagsQuery,
            insertTagsParams,
            _databaseSession.Transaction);
    }
}
