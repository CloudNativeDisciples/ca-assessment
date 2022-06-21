using CA.Assessment.Application.Mappers;
using CA.Assessment.Application.Repositories;
using CA.Assessment.Application.Responses;
using CA.Assessment.Application.Services;
using CA.Assessment.Model.Exceptions;
using CA.Assessment.Store;

namespace CA.Assessment.Application.Scripts;

public sealed class GetBlogPostImageDataTxScript
{
    private readonly IBlogPostRepository _blogPostsRepository;
    private readonly IDatabaseSessionManager _databaseSessionManager;
    private readonly IImagesContentStore _imagesContentStore;
    private readonly IImagesRepository _imagesRepository;

    public GetBlogPostImageDataTxScript(IDatabaseSessionManager databaseSessionManager,
        IBlogPostRepository blogPostsRepository,
        IImagesRepository imagesRepository,
        IImagesContentStore imagesContentStore)
    {
        _databaseSessionManager = databaseSessionManager ?? throw new ArgumentNullException(nameof(databaseSessionManager));
        _blogPostsRepository = blogPostsRepository ?? throw new ArgumentNullException(nameof(blogPostsRepository));
        _imagesRepository = imagesRepository ?? throw new ArgumentNullException(nameof(imagesRepository));
        _imagesContentStore = imagesContentStore ?? throw new ArgumentNullException(nameof(imagesContentStore));
    }

    public async Task<BlogPostImageData?> ExecuteAsync(Guid blogPostId)
    {
        await _databaseSessionManager.OpenConnectionAsync();

        var maybeBlogPost = await _blogPostsRepository.GetAsync(blogPostId);

        if (maybeBlogPost is null)
        {
            throw new BlogPostNotFoundException(blogPostId);
        }

        if (maybeBlogPost.Image == Guid.Empty)
        {
            return null;
        }

        try
        {
            var image = await _imagesRepository.GetAsync(maybeBlogPost.Image);

            if (image is null)
            {
                throw new BlogPostImageNotFoundException(maybeBlogPost.Image);
            }

            var imageContent = await _imagesContentStore.GetContentAsync(image.Identity);

            if (imageContent is null)
            {
                throw new BlogPostImageNotFoundException(image.Identity);
            }

            return ImageMapper.MapOneToBlogPostImageData(image, imageContent);
        }
        finally
        {
            await _databaseSessionManager.CloseConnectionAsync();
        }
    }
}
