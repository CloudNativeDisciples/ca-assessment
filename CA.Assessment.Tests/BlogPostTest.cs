using CA.Assessment.Model;
using CA.Assessment.Model.Exceptions;
using CA.Assessment.Tests.Helpers;
using CA.Assessment.WebAPI.Dtos;
using CA.Assessment.WebAPI.Services;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using static CA.Assessment.Tests.Helpers.StreamHelpers;

namespace CA.Assessment.Tests;

public class BlogPostTest : IntegrationTest
{
    [Test]
    public async Task Blog_posts_can_be_saved()
    {
        var txScriptsFacade = Resolve<TxScriptsFacade>();

        var newBlogPostDto = new NewBlogPostDto
        {
            Title = "title",
            Content = "content",
            Author = "author",
            Category = "category",
            Tags = new[]
            {
                "tag_1",
                "tag_2"
            }
        };

        var newBlogPostId = await txScriptsFacade.NewBlogPostAsync(newBlogPostDto);

        var newBlogPost = await txScriptsFacade.GetBlogPostAsync(newBlogPostId);

        Assert.That(newBlogPost, Is.Not.Null);

        Assert.That(newBlogPost!.Identity, Is.EqualTo(newBlogPostId));
        Assert.That(newBlogPost.Category.Name, Is.EqualTo("category"));

        Assert.That(newBlogPost.Tags.Select(t => t.Name), Is.EquivalentTo(new[] { "tag_1", "tag_2" }));
    }

    [Test]
    public async Task Blog_posts_can_be_deleted_if_user_is_admin()
    {
        var txScriptsFacade = Resolve<TxScriptsFacade>();

        var newBlogPostDto = new NewBlogPostDto
        {
            Title = "title",
            Content = "content",
            Author = "author",
            Category = "category",
            Tags = new[]
            {
                "tag_1",
                "tag_2"
            }
        };

        var newBlogPostId = await txScriptsFacade.NewBlogPostAsync(newBlogPostDto);

        var newBlogPost = await txScriptsFacade.GetBlogPostAsync(newBlogPostId);

        Assert.That(newBlogPost, Is.Not.Null);

        SetCurrentUserKind(UserKind.Admin);

        await txScriptsFacade.DeleteBlogPostAsync(newBlogPostId);

        var maybeBlogPost = await txScriptsFacade.GetBlogPostAsync(newBlogPostId);

        Assert.That(maybeBlogPost, Is.Null);
    }

    [Test]
    public async Task Blog_posts_cant_be_deleted_if_user_is_not_admin()
    {
        var txScriptsFacade = Resolve<TxScriptsFacade>();

        var newBlogPostDto = new NewBlogPostDto
        {
            Title = "title",
            Content = "content",
            Author = "author",
            Category = "category",
            Tags = new[]
            {
                "tag_1",
                "tag_2"
            }
        };

        var newBlogPostId = await txScriptsFacade.NewBlogPostAsync(newBlogPostDto);

        var newBlogPost = await txScriptsFacade.GetBlogPostAsync(newBlogPostId);

        Assert.That(newBlogPost, Is.Not.Null);

        SetCurrentUserKind(UserKind.User);

        async Task TryDeleteAsync()
        {
            await txScriptsFacade.DeleteBlogPostAsync(newBlogPostId);
        }

        Assert.That(TryDeleteAsync, Throws.InstanceOf<ForbiddenBlogPostDeletionException>());

        var maybeBlogPost = await txScriptsFacade.GetBlogPostAsync(newBlogPostId);

        Assert.That(maybeBlogPost, Is.Not.Null);
    }

    [Test]
    public async Task Blog_post_can_be_partially_updated()
    {
        var txScriptsFacade = Resolve<TxScriptsFacade>();

        var newBlogPostDto = new NewBlogPostDto
        {
            Title = "title",
            Content = "content",
            Author = "author",
            Category = "category",
            Tags = new[]
            {
                "tag_1",
                "tag_2"
            }
        };

        var newBlogPostId = await txScriptsFacade.NewBlogPostAsync(newBlogPostDto);

        var updateBlogPost = new UpdateBlogPostDto
        {
            Title = "new title",
            Author = "puppolo",
            Category = "another category",
            Content = "more content",
            Tags = new[]
            {
                "tag_a",
                "tag_b"
            }
        };

        await txScriptsFacade.UpdateBlogPostAsync(newBlogPostId, updateBlogPost);

        var blogPost = await txScriptsFacade.GetBlogPostAsync(newBlogPostId);

        Assert.That(blogPost, Is.Not.Null);

        Assert.That(blogPost!.Title, Is.EqualTo("new title"));
        Assert.That(blogPost.Author, Is.EqualTo("puppolo"));
        Assert.That(blogPost.Category.Name, Is.EqualTo("another category"));
        Assert.That(blogPost.Content, Is.EqualTo("more content"));
        Assert.That(blogPost.Tags.Select(t => t.Name), Is.EquivalentTo(new[] { "tag_a", "tag_b" }));
    }

    [Test]
    public async Task Blog_posts_can_be_searched()
    {
        var txScriptsFacade = Resolve<TxScriptsFacade>();

        var newBlogPostDto = new NewBlogPostDto
        {
            Title = "title_1",
            Content = "content",
            Author = "author",
            Category = "category_2",
            Tags = new[]
            {
                "tag_3"
            }
        };

        await txScriptsFacade.NewBlogPostAsync(newBlogPostDto);

        var anotherBlogPostDto = new NewBlogPostDto
        {
            Title = "title_2",
            Content = "content",
            Author = "author",
            Category = "category_1",
            Tags = new[]
            {
                "tag_2"
            }
        };

        await txScriptsFacade.NewBlogPostAsync(anotherBlogPostDto);

        var moreBlogPostDto = new NewBlogPostDto
        {
            Title = "title_3",
            Content = "content",
            Author = "author",
            Category = "category_2",
            Tags = new[]
            {
                "tag_2"
            }
        };

        await txScriptsFacade.NewBlogPostAsync(moreBlogPostDto);

        var searchFilters = new SearchBlogPostFiltersDto
        {
            Category = null,
            Tags = new[] { "tag_3" },
            Title = null
        };

        var searchResults = await txScriptsFacade.SearchBlogPostsAsync(searchFilters);

        var blogPostsList = searchResults.ToList();

        Assert.That(blogPostsList, Has.Count.EqualTo(1));
        Assert.That(blogPostsList[0].Title, Is.EqualTo("title_1"));
    }

    [Test]
    public async Task Blog_posts_can_be_tagged()
    {
        var txScriptsFacade = Resolve<TxScriptsFacade>();

        var newBlogPostDto = new NewBlogPostDto
        {
            Title = "title_2",
            Content = "content",
            Author = "author",
            Category = "category_1",
            Tags = new[]
            {
                "tag_3"
            }
        };

        var newBlogPostId = await txScriptsFacade.NewBlogPostAsync(newBlogPostDto);

        await txScriptsFacade.TagBlogPostAsync(newBlogPostId, new[] { "tag_1", "tag_2" });

        var blogPost = await txScriptsFacade.GetBlogPostAsync(newBlogPostId);

        var tagNames = blogPost!.Tags.Select(t => t.Name).ToList();

        Assert.That(tagNames, Has.Count.EqualTo(3));
        Assert.That(tagNames, Is.EquivalentTo(new[] { "tag_1", "tag_2", "tag_3" }));
    }

    [Test]
    public async Task Blog_posts_tags_are_never_duplicated()
    {
        var txScriptsFacade = Resolve<TxScriptsFacade>();

        var newBlogPostDto = new NewBlogPostDto
        {
            Title = "title_2",
            Content = "content",
            Author = "author",
            Category = "category_1",
            Tags = new[]
            {
                "tag_1",
                "tag_2",
                "tag_3"
            }
        };

        var newBlogPostId = await txScriptsFacade.NewBlogPostAsync(newBlogPostDto);

        await txScriptsFacade.TagBlogPostAsync(newBlogPostId, new[] { "tag_1", "tag_2" });

        var blogPost = await txScriptsFacade.GetBlogPostAsync(newBlogPostId);

        var tagNames = blogPost!.Tags.Select(t => t.Name).ToList();

        Assert.That(tagNames, Has.Count.EqualTo(3));
        Assert.That(tagNames, Is.EquivalentTo(new[] { "tag_1", "tag_2", "tag_3" }));
    }

    [Test]
    public async Task Blog_posts_can_be_untagged()
    {
        var txScriptsFacade = Resolve<TxScriptsFacade>();

        var newBlogPostDto = new NewBlogPostDto
        {
            Title = "title_2",
            Content = "content",
            Author = "author",
            Category = "category_1",
            Tags = new[]
            {
                "tag_1",
                "tag_2",
                "tag_3"
            }
        };

        var newBlogPostId = await txScriptsFacade.NewBlogPostAsync(newBlogPostDto);

        await txScriptsFacade.UntagBlogPostAsync(newBlogPostId, new[] { "tag_1", "tag_2" });

        var blogPost = await txScriptsFacade.GetBlogPostAsync(newBlogPostId);

        var tagNames = blogPost!.Tags.Select(t => t.Name).ToList();

        Assert.That(tagNames, Has.Count.EqualTo(1));
        Assert.That(tagNames, Is.EquivalentTo(new[] { "tag_3" }));
    }

    [Test]
    public async Task Blog_posts_can_have_an_image()
    {
        var txScriptsFacade = Resolve<TxScriptsFacade>();

        var newBlogPostDto = new NewBlogPostDto
        {
            Title = "title_2",
            Content = "content",
            Author = "author",
            Category = "category_1",
            Tags = new[]
            {
                "tag_1",
                "tag_2",
                "tag_3"
            }
        };

        var newBlogPostId = await txScriptsFacade.NewBlogPostAsync(newBlogPostDto);

        using var inMemoryStream = await NewBytesStreamAsync(new byte[] { 15, 14, 13, 14 });

        var formFileMock = new Mock<IFormFile>();

        formFileMock.Setup(f => f.OpenReadStream())
            .Returns(inMemoryStream);

        formFileMock.Setup(f => f.Name)
            .Returns("pippo.png");

        formFileMock.Setup(f => f.ContentType)
            .Returns("image/png");

        var newImageId = await txScriptsFacade.AttachImageToBlogPostAsync(newBlogPostId, formFileMock.Object);

        var blogPost = await txScriptsFacade.GetBlogPostAsync(newBlogPostId);
        var image = await txScriptsFacade.GetBlogPostImageDataAsync(newBlogPostId);

        Assert.That(blogPost!.Image, Is.EqualTo(newImageId));

        Assert.That(image, Is.Not.Null);
        Assert.That(image!.ImageStream, Is.Not.Null);
        Assert.That(image.Mime, Is.EqualTo("image/png"));

        var buffer = await ReadBytesFromStreamAsync(image.ImageStream, 4);

        Assert.That(buffer, Has.Length.EqualTo(4));
        Assert.That(buffer, Is.EqualTo(new byte[] { 15, 14, 13, 14 }));
    }
}
