using CA.Assessment.Application.Dtos;
using CA.Assessment.Application.Repositories;
using CA.Assessment.Domain.Anemic;
using CA.Assessment.Domain.Anemic.Exceptions;

namespace CA.Assessment.Application.Mappers;

internal sealed class BlogPostMapper
{
    private readonly ICategoryRepository categoryRepository;
    private readonly ITagsRepository tagsRepository;

    public BlogPostMapper(ITagsRepository tagsRepository, ICategoryRepository categoryRepository)
    {
        this.tagsRepository = tagsRepository ?? throw new ArgumentNullException(nameof(tagsRepository));
        this.categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
    }

    public async Task<BlogPostDetail> MapOneToBlogPostDetailsAsync(BlogPost blogPost)
    {
        if (blogPost is null) throw new ArgumentNullException(nameof(blogPost));

        var blogPostCategory = await categoryRepository.GetAsync(blogPost.Category);

        if (blogPostCategory is null) throw new MissingCategoryException(blogPost.Category);

        var tags = await tagsRepository.GetManyAsync(blogPost.Tags);

        var tagDetails = tags
            .Select(t => new TagDetails(t.Identity, t.Name))
            .ToList();

        var categoryDetails = new CategoryDetails(blogPostCategory.Identity, blogPostCategory.Name);

        return new BlogPostDetail(blogPost.Identity, blogPost.Author, blogPost.Content, blogPost.Title,
            blogPost.Image, categoryDetails, tagDetails);
    }
}
