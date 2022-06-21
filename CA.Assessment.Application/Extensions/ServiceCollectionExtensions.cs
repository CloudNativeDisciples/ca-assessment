using CA.Assessment.Application.Requests;
using CA.Assessment.Application.Scripts;
using CA.Assessment.Application.Services;
using CA.Assessment.Application.Validators;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace CA.Assessment.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAssessmentApplication(this IServiceCollection serviceCollection)
    {
        if (serviceCollection is null)
        {
            throw new ArgumentNullException(nameof(serviceCollection));
        }

        serviceCollection.AddTransient<CategoriesMaker>();
        serviceCollection.AddTransient<TagsMaker>();

        serviceCollection.AddTransient<NewBlogPostTxScript>();
        serviceCollection.AddTransient<GetBlogPostTxScript>();
        serviceCollection.AddTransient<UpdateBlogPostTxScript>();
        serviceCollection.AddTransient<DeleteBlogPostTxScript>();

        serviceCollection.AddTransient<AttachBlogPostImageTxScript>();
        serviceCollection.AddTransient<GetBlogPostImageDataTxScript>();

        serviceCollection.AddTransient<SearchBlogPostsTxScript>();

        serviceCollection.AddTransient<TagBlogPostTxScript>();
        serviceCollection.AddTransient<UntagBlogPostTxScript>();

        serviceCollection.AddTransient<IValidator<NewBlogPost>, NewBlogPostValidator>();
        serviceCollection.AddTransient<IValidator<UpdateBlogPost>, UpdateBlogPostValidator>();

        return serviceCollection;
    }
}
