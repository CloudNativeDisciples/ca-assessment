using System;
using System.Linq;
using System.Threading.Tasks;
using CA.Assessment.Application.Dtos;
using CA.Assessment.Application.Services;
using CA.Assessment.Domain.Anemic;
using CA.Assessment.Domain.Anemic.Exceptions;
using CA.Assessment.Tests.Helpers;
using NUnit.Framework;

namespace CA.Assessment.Tests;

public class BlogPostTest : IntegrationTest
{
    [Test]
    public async Task Blog_posts_can_be_saved()
    {
        var sut = Resolve<IBlogPostsService>();

        var newBlogPostId = Guid.NewGuid();

        var newBlogPostRequest = new NewBlogPost("title", "content", "author", "category", new[] { "tag_1", "tag_2" });

        await sut.NewAsync(newBlogPostId, newBlogPostRequest);

        var newBlogPost = await sut.GetAsync(newBlogPostId);

        Assert.That(newBlogPost, Is.Not.Null);

        Assert.That(newBlogPost!.Identity, Is.EqualTo(newBlogPostId));
        Assert.That(newBlogPost.Category.Name, Is.EqualTo("category"));

        Assert.That(newBlogPost.Tags.Select(t => t.Name), Is.EquivalentTo(new[] { "tag_1", "tag_2" }));
    }

    [Test]
    public async Task Blog_posts_can_be_deleted_if_user_is_admin()
    {
        var sut = Resolve<IBlogPostsService>();

        var newBlogPostId = Guid.NewGuid();

        var newBlogPostRequest = new NewBlogPost("title", "content", "author", "category", new[] { "tag_1", "tag_2" });

        await sut.NewAsync(newBlogPostId, newBlogPostRequest);

        var newBlogPost = await sut.GetAsync(newBlogPostId);

        Assert.That(newBlogPost, Is.Not.Null);

        SetCurrentUserKind(UserKind.Admin);

        await sut.DeleteAsync(newBlogPostId);

        var maybeBlogPost = await sut.GetAsync(newBlogPostId);

        Assert.That(maybeBlogPost, Is.Null);
    }

    [Test]
    public async Task Blog_posts_cant_be_deleted_if_user_is_not_admin()
    {
        var sut = Resolve<IBlogPostsService>();

        var newBlogPostId = Guid.NewGuid();

        var newBlogPostRequest = new NewBlogPost("title", "content", "author", "category", new[] { "tag_1", "tag_2" });

        await sut.NewAsync(newBlogPostId, newBlogPostRequest);

        var newBlogPost = await sut.GetAsync(newBlogPostId);

        Assert.That(newBlogPost, Is.Not.Null);

        SetCurrentUserKind(UserKind.User);

        async Task TryDeleteAsync()
        {
            await sut.DeleteAsync(newBlogPostId);
        }

        Assert.That(TryDeleteAsync, Throws.InstanceOf<UnauthorizedBlogPostDeletionException>());

        var maybeBlogPost = await sut.GetAsync(newBlogPostId);

        Assert.That(maybeBlogPost, Is.Not.Null);
    }

    [Test]
    public async Task Blog_post_can_be_partially_updated()
    {
        var sut = Resolve<IBlogPostsService>();

        var newBlogPostId = Guid.NewGuid();

        var newBlogPostRequest = new NewBlogPost("title", "content", "author", "category", new[] { "tag_1", "tag_2" });

        await sut.NewAsync(newBlogPostId, newBlogPostRequest);

        var newBlogPost = await sut.GetAsync(newBlogPostId);

        Assert.That(newBlogPost, Is.Not.Null);

        var updateBlogPost = new UpdateBlogPost("new title", null, "new author", null, null);

        await sut.UpdateAsync(newBlogPostId, updateBlogPost);

        var blogPost = await sut.GetAsync(newBlogPostId);

        Assert.That(blogPost, Is.Not.Null);

        Assert.That(blogPost!.Title, Is.EqualTo("new title"));
        Assert.That(blogPost.Author, Is.EqualTo("new author"));
    }
}
