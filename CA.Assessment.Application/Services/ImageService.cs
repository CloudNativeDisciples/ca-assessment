using CA.Assessment.Application.Dtos;
using CA.Assessment.Application.Mappers;
using CA.Assessment.Application.Repositories;
using CA.Assessment.Domain.Anemic;
using CA.Assessment.Domain.Anemic.Exceptions;
using CA.Assessment.Store;

namespace CA.Assessment.Application.Services;

internal sealed class ImageService : IImageService
{
    private readonly IBlogPostRepository blogPostsRepository;
    private readonly IDatabaseSessionManager databaseSessionManager;
    private readonly ImageMapper imageMapper;
    private readonly IImagesRepository imagesRepository;

    public ImageService(
        IImagesRepository imagesRepository,
        IBlogPostRepository blogPostsRepository,
        IDatabaseSessionManager databaseSessionManager,
        ImageMapper imageMapper)
    {
        this.imagesRepository = imagesRepository ?? throw new ArgumentNullException(nameof(imagesRepository));

        this.blogPostsRepository = blogPostsRepository ?? throw new ArgumentNullException(nameof(blogPostsRepository));

        this.databaseSessionManager =
            databaseSessionManager ?? throw new ArgumentNullException(nameof(databaseSessionManager));

        this.imageMapper = imageMapper ?? throw new ArgumentNullException(nameof(imageMapper));
    }

    public async Task AttachImageToBlogPostAsync(Guid newImageId, Guid blogPostId, NewBlogPostImage newBlogPostImage)
    {
        if (newBlogPostImage is null) throw new ArgumentNullException(nameof(newBlogPostImage));

        await databaseSessionManager.BeginTransactionAsync();

        var maybeBlogPost = await blogPostsRepository.GetAsync(blogPostId);

        if (maybeBlogPost is null) throw new BlogPostNotFoundException(blogPostId);

        var newImage = new Image(newImageId, newBlogPostImage.Content, newBlogPostImage.Mime, newBlogPostImage.Name);

        var blogPostWithImage = new BlogPost(maybeBlogPost.Identity, maybeBlogPost.Title, maybeBlogPost.Author,
            maybeBlogPost.Content, newImageId, maybeBlogPost.Tags, maybeBlogPost.Category);

        try
        {
            await imagesRepository.SaveAsync(newImage);

            await blogPostsRepository.UpdateAsync(blogPostWithImage);

            await databaseSessionManager.CommitTransactionAsync();
        }
        catch
        {
            await databaseSessionManager.RollbackTransactionAsync();

            throw;
        }
    }

    public async Task<BlogPostImageData> GetBlogPostImageAsync(Guid blogPostId, Guid imageId)
    {
        await databaseSessionManager.OpenConnectionAsync();

        var maybeBlogPost = await blogPostsRepository.GetAsync(blogPostId);

        if (maybeBlogPost is null) throw new BlogPostNotFoundException(blogPostId);

        try
        {
            var image = await imagesRepository.GetAsync(imageId);

            if (image is null) throw new BlogPostImageNotFoundException(imageId);

            return imageMapper.MapOneToBlogPostImageData(image);
        }
        finally
        {
            await databaseSessionManager.CloseConnectionAsync();
        }
    }
}
