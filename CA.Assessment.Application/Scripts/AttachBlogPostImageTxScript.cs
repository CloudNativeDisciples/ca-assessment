﻿using CA.Assessment.Application.Repositories;
using CA.Assessment.Application.Requests;
using CA.Assessment.Application.Services;
using CA.Assessment.Model;
using CA.Assessment.Model.Exceptions;
using CA.Assessment.Store;

namespace CA.Assessment.Application.Scripts;

public sealed class AttachBlogPostImageTxScript
{
    private readonly IBlogPostRepository _blogPostsRepository;
    private readonly IDatabaseSessionManager _databaseSessionManager;
    private readonly IImagesContentStore _imagesContentsStore;
    private readonly IImagesRepository _imagesRepository;

    public AttachBlogPostImageTxScript(IImagesRepository imagesRepository,
        IBlogPostRepository blogPostsRepository,
        IDatabaseSessionManager databaseSessionManager,
        IImagesContentStore imagesContentStore)
    {
        _imagesRepository = imagesRepository ?? throw new ArgumentNullException(nameof(imagesRepository));
        _blogPostsRepository = blogPostsRepository ?? throw new ArgumentNullException(nameof(blogPostsRepository));
        _databaseSessionManager = databaseSessionManager ?? throw new ArgumentNullException(nameof(databaseSessionManager));
        _imagesContentsStore = imagesContentStore ?? throw new ArgumentNullException(nameof(imagesContentStore));
    }

    public async Task ExecuteAsync(Guid newImageId, Guid blogPostId, BlogPostImageToAttach blogPostImageToAttach)
    {
        if (blogPostImageToAttach is null)
        {
            throw new ArgumentNullException(nameof(blogPostImageToAttach));
        }

        await _databaseSessionManager.BeginTransactionAsync();

        var maybeBlogPost = await _blogPostsRepository.GetAsync(blogPostId);

        if (maybeBlogPost is null)
        {
            throw new BlogPostNotFoundException(blogPostId);
        }

        var newImage = new BlogPostImage(newImageId, blogPostImageToAttach.Mime, blogPostImageToAttach.Name);

        var blogPostWithImage = new BlogPost(maybeBlogPost.Identity, maybeBlogPost.Title, maybeBlogPost.Author,
            maybeBlogPost.Content, newImageId, maybeBlogPost.Tags, maybeBlogPost.Category);

        try
        {
            await _imagesRepository.SaveAsync(newImage);

            await _blogPostsRepository.UpdateAsync(blogPostWithImage);

            await _imagesContentsStore.SaveContentAsync(newImage.Identity, blogPostImageToAttach.ImageStream);

            await _databaseSessionManager.CommitTransactionAsync();
        }
        catch
        {
            await _databaseSessionManager.RollbackTransactionAsync();

            throw;
        }
    }
}
