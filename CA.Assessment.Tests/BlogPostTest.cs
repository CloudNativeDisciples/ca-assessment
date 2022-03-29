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

    [Test]
    public async Task Blog_posts_can_be_searched()
    {
        var sut = Resolve<ISearchService>();

        var blogPostsService = Resolve<IBlogPostsService>();

        var newBlogPostId = Guid.NewGuid();

        var newBlogPostRequest = new NewBlogPost("title_1", "content", "author", "category_2", new[] { "tag_3" });

        await blogPostsService.NewAsync(newBlogPostId, newBlogPostRequest);

        var anotherBlogPostId = Guid.NewGuid();

        var anotherBlogPost = new NewBlogPost("title_2", "content", "author", "category_1", new[] { "tag_2" });

        await blogPostsService.NewAsync(anotherBlogPostId, anotherBlogPost);

        var moreBlogPostId = Guid.NewGuid();

        var moreBlogPosts = new NewBlogPost("title_3", "content", "author", "category_2", new[] { "tag_2" });

        await blogPostsService.NewAsync(moreBlogPostId, moreBlogPosts);

        var search = new SearchBlogPostsFilters(null, new[] { "tag_3" }, null);

        var blogPostsFound = await sut.SearchBlogPostsAsync(search);

        var blogPostsList = blogPostsFound.ToList();

        Assert.That(blogPostsList, Has.Count.EqualTo(1));
        Assert.That(blogPostsList[0].Title, Is.EqualTo("title_1"));
    }

    [Test]
    public async Task Blog_posts_can_be_tagged()
    {
        var sut = Resolve<ITagsService>();

        var blogPostsService = Resolve<IBlogPostsService>();

        var newBlogPostId = Guid.NewGuid();

        var newBlogPostRequest = new NewBlogPost("title_1", "content", "author", "category_2", new[] { "tag_3" });

        await blogPostsService.NewAsync(newBlogPostId, newBlogPostRequest);

        await sut.TagAsync(newBlogPostId, new[] { "tag_1", "tag_2" });

        var blogPost = await blogPostsService.GetAsync(newBlogPostId);

        var tagNames = blogPost!.Tags.Select(t => t.Name).ToList();

        Assert.That(tagNames, Has.Count.EqualTo(3));
        Assert.That(tagNames, Is.EquivalentTo(new[] { "tag_1", "tag_2", "tag_3" }));
    }

    [Test]
    public async Task Blog_posts_can_be_untagged()
    {
        var sut = Resolve<ITagsService>();

        var blogPostsService = Resolve<IBlogPostsService>();

        var newBlogPostId = Guid.NewGuid();

        var newBlogPostRequest = new NewBlogPost("title_1", "content", "author", "category_2",
            new[] { "tag_1", "tag_2", "tag_3" });

        await blogPostsService.NewAsync(newBlogPostId, newBlogPostRequest);

        await sut.UntagAsync(newBlogPostId, new[] { "tag_1", "tag_2" });

        var blogPost = await blogPostsService.GetAsync(newBlogPostId);

        var tagNames = blogPost!.Tags.Select(t => t.Name).ToList();

        Assert.That(tagNames, Has.Count.EqualTo(1));
        Assert.That(tagNames, Is.EquivalentTo(new[] { "tag_3" }));
    }

    [Test]
    public async Task Blog_posts_can_have_an_image()
    {
        var sut = Resolve<IImageService>();

        var blogPostsService = Resolve<IBlogPostsService>();

        var newBlogPostId = Guid.NewGuid();

        var newImageId = Guid.NewGuid();

        var newBlogPostRequest = new NewBlogPost("title_1", "content", "author", "category_2",
            new[] { "tag_1", "tag_2", "tag_3" });

        await blogPostsService.NewAsync(newBlogPostId, newBlogPostRequest);

        var newBlogPostImage = new NewBlogPostImage("test", "image/png", new byte[4] { 15, 14, 13, 14 });

        await sut.AttachImageToBlogPostAsync(newImageId, newBlogPostId, newBlogPostImage);

        var blogPost = await blogPostsService.GetAsync(newBlogPostId);

        var image = await sut.GetBlogPostImageAsync(newBlogPostId, newImageId);

        Assert.That(blogPost!.Image, Is.EqualTo(newImageId));

        Assert.That(image, Is.Not.Null);
        Assert.That(image.Content, Is.EqualTo(new byte[] { 15, 14, 13, 14 }));
        Assert.That(image.Mime, Is.EqualTo("image/png"));
    }
}
