using CA.Assessment.Application.Dtos;
using CA.Assessment.Application.Mappers;
using CA.Assessment.Application.Providers;
using CA.Assessment.Application.Services;
using CA.Assessment.Application.Validations;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace CA.Assessment.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAssessmentApplication(this IServiceCollection serviceCollection)
    {
        if (serviceCollection is null) throw new ArgumentNullException(nameof(serviceCollection));

        serviceCollection.AddTransient<ICategoriesMaker, CategoriesMaker>();
        serviceCollection.AddTransient<ITagsMaker, TagsMaker>();

        serviceCollection.AddTransient<IBlogPostsService, BlogPostService>();

        serviceCollection.AddTransient<IValidator<NewBlogPost>, NewBlogPostValidator>();
        serviceCollection.AddTransient<IValidator<UpdateBlogPost>, UpdateBlogPostValidator>();

        serviceCollection.AddTransient<BlogPostMapper>();

        return serviceCollection;
    }
}
