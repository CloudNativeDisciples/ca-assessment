using CA.Assessment.Application.Dtos;
using CA.Assessment.Application.Mappers;
using CA.Assessment.Application.Repositories;
using CA.Assessment.Domain.Anemic;
using CA.Assessment.Domain.Anemic.Exceptions;
using CA.Assessment.Images;
using CA.Assessment.Store;

namespace CA.Assessment.Application.Services;

internal sealed class ImageService : IImageService
{
    private readonly IBlogPostRepository _blogPostsRepository;
    private readonly IDatabaseSessionManager _databaseSessionManager;
    private readonly ImageMapper _imageMapper;
    private readonly IImageStore _imageStore;
    private readonly IImagesRepository _imagesRepository;

    public ImageService(
        IImagesRepository imagesRepository,
        IBlogPostRepository blogPostsRepository,
        IDatabaseSessionManager databaseSessionManager,
        ImageMapper imageMapper,
        IImageStore imageStore)
    {
        _imagesRepository = imagesRepository ?? throw new ArgumentNullException(nameof(imagesRepository));

        _blogPostsRepository = blogPostsRepository ?? throw new ArgumentNullException(nameof(blogPostsRepository));

        _databaseSessionManager =
            databaseSessionManager ?? throw new ArgumentNullException(nameof(databaseSessionManager));

        _imageMapper = imageMapper ?? throw new ArgumentNullException(nameof(imageMapper));

        _imageStore = imageStore ?? throw new ArgumentNullException(nameof(imageStore));
    }

    public async Task AttachImageToBlogPostAsync(Guid newImageId, Guid blogPostId, NewBlogPostImage newBlogPostImage)
    {
        if (newBlogPostImage is null)
        {
            throw new ArgumentNullException(nameof(newBlogPostImage));
        }

        await _databaseSessionManager.BeginTransactionAsync();

        var maybeBlogPost = await _blogPostsRepository.GetAsync(blogPostId);

        if (maybeBlogPost is null)
        {
            throw new BlogPostNotFoundException(blogPostId);
        }

        var newImage = new Image(newImageId, newBlogPostImage.Mime, newBlogPostImage.Name);

        var blogPostWithImage = new BlogPost(maybeBlogPost.Identity, maybeBlogPost.Title, maybeBlogPost.Author,
            maybeBlogPost.Content, newImageId, maybeBlogPost.Tags, maybeBlogPost.Category);

        try
        {
            await _imagesRepository.SaveAsync(newImage);

            await _blogPostsRepository.UpdateAsync(blogPostWithImage);

            await _imageStore.SaveImageAsync(newImage.Identity, newBlogPostImage.ImageStream);

            await _databaseSessionManager.CommitTransactionAsync();
        }
        catch
        {
            await _databaseSessionManager.RollbackTransactionAsync();

            throw;
        }
    }

    public async Task<BlogPostImageData> GetBlogPostImageAsync(Guid blogPostId, Guid imageId)
    {
        await _databaseSessionManager.OpenConnectionAsync();

        var maybeBlogPost = await _blogPostsRepository.GetAsync(blogPostId);

        if (maybeBlogPost is null)
        {
            throw new BlogPostNotFoundException(blogPostId);
        }

        try
        {
            var image = await _imagesRepository.GetAsync(imageId);

            if (image is null)
            {
                throw new BlogPostImageNotFoundException(imageId);
            }

            var imageFromStore = await _imageStore.GetImageAsync(image.Identity);

            if (imageFromStore is null)
            {
                throw new BlogPostImageNotFoundException(image.Identity);
            }

            return _imageMapper.MapOneToBlogPostImageData(image, imageFromStore);
        }
        finally
        {
            await _databaseSessionManager.CloseConnectionAsync();
        }
    }
}