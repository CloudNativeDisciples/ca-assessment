using CA.Assessment.Application.Repositories;
using CA.Assessment.Application.Services;
using CA.Assessment.Model;
using CA.Assessment.Model.Exceptions;
using CA.Assessment.Store;

namespace CA.Assessment.Application.Scripts;

public sealed class DeleteBlogPostTxScript
{
    private readonly IBlogPostRepository _blogPostRepository;
    private readonly ICurrentUserKindProvider _currentUserKindProvider;
    private readonly IDatabaseSessionManager _databaseSessionManager;

    public DeleteBlogPostTxScript(IDatabaseSessionManager databaseSessionManager,
        ICurrentUserKindProvider currentUserKindProvider,
        IBlogPostRepository blogPostRepository)
    {
        _databaseSessionManager = databaseSessionManager ?? throw new ArgumentNullException(nameof(databaseSessionManager));
        _currentUserKindProvider = currentUserKindProvider ?? throw new ArgumentNullException(nameof(currentUserKindProvider));
        _blogPostRepository = blogPostRepository ?? throw new ArgumentNullException(nameof(blogPostRepository));
    }

    public async Task ExecuteAsync(Guid blogPostId)
    {
        var currentUserKind = await _currentUserKindProvider.GetUserKindAsync();

        if (currentUserKind == UserKind.Unknown)
        {
            throw new UnauthorizedBlogPostDeletionException();
        }

        if (currentUserKind == UserKind.User)
        {
            throw new ForbiddenBlogPostDeletionException();
        }

        await _databaseSessionManager.BeginTransactionAsync();

        try
        {
            await _blogPostRepository.DeleteAsync(blogPostId);

            await _databaseSessionManager.CommitTransactionAsync();
        }
        catch
        {
            await _databaseSessionManager.RollbackTransactionAsync();

            throw;
        }
    }
}
