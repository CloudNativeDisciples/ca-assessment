using CA.Assessment.Application.Requests;
using CA.Assessment.Application.Scripts;
using CA.Assessment.Model;
using CA.Assessment.Model.Exceptions;
using CA.Assessment.Tests.Helpers;
using NUnit.Framework;
using static CA.Assessment.Tests.Helpers.StreamHelpers;

namespace CA.Assessment.Tests;

public class BlogPostTest : IntegrationTest
{
    [Test]
    public async Task Blog_posts_can_be_saved()
    {
        var newBlogPostTxScript = Resolve<NewBlogPostTxScript>();

        var newBlogPostId = Guid.NewGuid();

        var newBlogPostRequest = new NewBlogPost(newBlogPostId, "title", "content", "author", "category", new[] { "tag_1", "tag_2" });

        await newBlogPostTxScript.ExecuteAsync(newBlogPostRequest);

        var getBlogPostTxScript = Resolve<GetBlogPostTxScript>();

        var newBlogPost = await getBlogPostTxScript.ExecuteAsync(newBlogPostId);

        Assert.That(newBlogPost, Is.Not.Null);

        Assert.That(newBlogPost!.Identity, Is.EqualTo(newBlogPostId));
        Assert.That(newBlogPost.Category.Name, Is.EqualTo("category"));

        Assert.That(newBlogPost.Tags.Select(t => t.Name), Is.EquivalentTo(new[] { "tag_1", "tag_2" }));
    }

    [Test]
    public async Task Blog_posts_can_be_deleted_if_user_is_admin()
    {
        var newBlogPostTxScript = Resolve<NewBlogPostTxScript>();

        var newBlogPostId = Guid.NewGuid();

        var newBlogPostRequest = new NewBlogPost(newBlogPostId, "title", "content", "author", "category", new[] { "tag_1", "tag_2" });

        await newBlogPostTxScript.ExecuteAsync(newBlogPostRequest);

        var getBlogPostTxScript = Resolve<GetBlogPostTxScript>();

        var newBlogPost = await getBlogPostTxScript.ExecuteAsync(newBlogPostId);

        Assert.That(newBlogPost, Is.Not.Null);

        SetCurrentUserKind(UserKind.Admin);

        var deleteBlogPostTxScript = Resolve<DeleteBlogPostTxScript>();

        await deleteBlogPostTxScript.ExecuteAsync(newBlogPostId);

        var maybeBlogPost = await getBlogPostTxScript.ExecuteAsync(newBlogPostId);

        Assert.That(maybeBlogPost, Is.Null);
    }

    [Test]
    public async Task Blog_posts_cant_be_deleted_if_user_is_not_admin()
    {
        var newBlogPostTxScript = Resolve<NewBlogPostTxScript>();

        var newBlogPostId = Guid.NewGuid();

        var newBlogPostRequest = new NewBlogPost(newBlogPostId, "title", "content", "author", "category", new[] { "tag_1", "tag_2" });

        await newBlogPostTxScript.ExecuteAsync(newBlogPostRequest);

        var getBlogPostTxScript = Resolve<GetBlogPostTxScript>();

        var newBlogPost = await getBlogPostTxScript.ExecuteAsync(newBlogPostId);

        Assert.That(newBlogPost, Is.Not.Null);

        SetCurrentUserKind(UserKind.User);

        var deleteBlogPostTxScript = Resolve<DeleteBlogPostTxScript>();

        async Task TryDeleteAsync()
        {
            await deleteBlogPostTxScript.ExecuteAsync(newBlogPostId);
        }

        Assert.That(TryDeleteAsync, Throws.InstanceOf<ForbiddenBlogPostDeletionException>());

        var maybeBlogPost = await getBlogPostTxScript.ExecuteAsync(newBlogPostId);

        Assert.That(maybeBlogPost, Is.Not.Null);
    }

    [Test]
    public async Task Blog_post_can_be_partially_updated()
    {
        var newBlogPostTxScript = Resolve<NewBlogPostTxScript>();

        var newBlogPostId = Guid.NewGuid();

        var newBlogPostRequest = new NewBlogPost(newBlogPostId, "title", "content", "author", "category", new[] { "tag_1", "tag_2" });

        await newBlogPostTxScript.ExecuteAsync(newBlogPostRequest);

        var updateBlogPostTxScript = Resolve<UpdateBlogPostTxScript>();

        var updateBlogPost = new UpdateBlogPost(newBlogPostId, "new title", null, "new author", null, null);

        await updateBlogPostTxScript.ExecuteAsync(updateBlogPost);

        var getBlogPostTxScript = Resolve<GetBlogPostTxScript>();

        var blogPost = await getBlogPostTxScript.ExecuteAsync(newBlogPostId);

        Assert.That(blogPost, Is.Not.Null);

        Assert.That(blogPost!.Title, Is.EqualTo("new title"));
        Assert.That(blogPost.Author, Is.EqualTo("new author"));
    }

    [Test]
    public async Task Blog_posts_can_be_searched()
    {
        var newBlogPostTxScript = Resolve<NewBlogPostTxScript>();

        var newBlogPostId = Guid.NewGuid();

        var newBlogPostRequest = new NewBlogPost(newBlogPostId, "title_1", "content", "author", "category_2", new[] { "tag_3" });

        await newBlogPostTxScript.ExecuteAsync(newBlogPostRequest);

        var anotherBlogPostId = Guid.NewGuid();

        var anotherBlogPost = new NewBlogPost(anotherBlogPostId, "title_2", "content", "author", "category_1", new[] { "tag_2" });

        await newBlogPostTxScript.ExecuteAsync(anotherBlogPost);

        var moreBlogPostId = Guid.NewGuid();

        var moreBlogPosts = new NewBlogPost(moreBlogPostId, "title_3", "content", "author", "category_2", new[] { "tag_2" });

        await newBlogPostTxScript.ExecuteAsync(moreBlogPosts);

        var searchBlogPostsTxScript = Resolve<SearchBlogPostsTxScript>();

        var filters = new SearchBlogPost(null, new[] { "tag_3" }, null);

        var blogPostsFound = await searchBlogPostsTxScript.ExecuteAsync(filters);

        var blogPostsList = blogPostsFound.ToList();

        Assert.That(blogPostsList, Has.Count.EqualTo(1));
        Assert.That(blogPostsList[0].Title, Is.EqualTo("title_1"));
    }

    [Test]
    public async Task Blog_posts_can_be_tagged()
    {
        var newBlogPostTxScript = Resolve<NewBlogPostTxScript>();

        var newBlogPostId = Guid.NewGuid();

        var newBlogPostRequest = new NewBlogPost(newBlogPostId, "title_1", "content", "author", "category_2", new[] { "tag_3" });

        await newBlogPostTxScript.ExecuteAsync(newBlogPostRequest);

        var tagBlogPostTxScript = Resolve<TagBlogPostTxScript>();

        await tagBlogPostTxScript.ExecuteAsync(newBlogPostId, new[] { "tag_1", "tag_2" });

        var getBlogPostTxScript = Resolve<GetBlogPostTxScript>();

        var blogPost = await getBlogPostTxScript.ExecuteAsync(newBlogPostId);

        var tagNames = blogPost!.Tags.Select(t => t.Name).ToList();

        Assert.That(tagNames, Has.Count.EqualTo(3));
        Assert.That(tagNames, Is.EquivalentTo(new[] { "tag_1", "tag_2", "tag_3" }));
    }

    [Test]
    public async Task Blog_posts_can_be_untagged()
    {
        var newBlogPostTxScript = Resolve<NewBlogPostTxScript>();

        var newBlogPostId = Guid.NewGuid();

        var newBlogPostRequest = new NewBlogPost(newBlogPostId, "title_1", "content", "author", "category_2",
            new[] { "tag_1", "tag_2", "tag_3" });

        await newBlogPostTxScript.ExecuteAsync(newBlogPostRequest);

        var untagBlogPostTxScript = Resolve<UntagBlogPostTxScript>();

        await untagBlogPostTxScript.ExecuteAsync(newBlogPostId, new[] { "tag_1", "tag_2" });

        var getBlogPostTxScript = Resolve<GetBlogPostTxScript>();

        var blogPost = await getBlogPostTxScript.ExecuteAsync(newBlogPostId);

        var tagNames = blogPost!.Tags.Select(t => t.Name).ToList();

        Assert.That(tagNames, Has.Count.EqualTo(1));
        Assert.That(tagNames, Is.EquivalentTo(new[] { "tag_3" }));
    }

    [Test]
    public async Task Blog_posts_can_have_an_image()
    {
        var newBlogPostTxScript = Resolve<NewBlogPostTxScript>();

        var newBlogPostId = Guid.NewGuid();

        var newImageId = Guid.NewGuid();

        var newBlogPostRequest = new NewBlogPost(newBlogPostId, "title_1", "content", "author", "category_2",
            new[] { "tag_1", "tag_2", "tag_3" });

        await newBlogPostTxScript.ExecuteAsync(newBlogPostRequest);

        var inMemoryStream = await NewBytesStreamAsync(new byte[] { 15, 14, 13, 14 });

        var newBlogPostImage = new AttachImageToBlogPost(newBlogPostId, newImageId, "test", "image/png", inMemoryStream);

        var attachImageTxScript = Resolve<AttachBlogPostImageTxScript>();

        await attachImageTxScript.ExecuteAsync(newBlogPostImage);

        var getBlogPostTxScript = Resolve<GetBlogPostTxScript>();

        var blogPost = await getBlogPostTxScript.ExecuteAsync(newBlogPostId);

        var getBlogPostImageTxScript = Resolve<GetBlogPostImageDataTxScript>();

        var image = await getBlogPostImageTxScript.ExecuteAsync(newBlogPostId);

        Assert.That(blogPost!.Image, Is.EqualTo(newImageId));

        Assert.That(image, Is.Not.Null);
        Assert.That(image!.ImageStream, Is.Not.Null);
        Assert.That(image.Mime, Is.EqualTo("image/png"));

        var buffer = await ReadBytesFromStreamAsync(image.ImageStream, 4);

        Assert.That(buffer, Has.Length.EqualTo(4));
        Assert.That(buffer, Is.EqualTo(new byte[] { 15, 14, 13, 14 }));
    }
}
