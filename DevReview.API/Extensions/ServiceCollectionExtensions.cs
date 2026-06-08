using AutoMapper;
using DevReview.API.Services;
using DevReview.Application.Auth;
using DevReview.Application.Interfaces;
using DevReview.Application.Mappings;
using DevReview.Application.Validators;
using DevReview.Infrastructure.Configuration;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DevReview.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddMediatR(typeof(RegisterCommand).Assembly);
            services.AddAutoMapper(typeof(MappingProfile).Assembly);
            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining<RegisterCommandValidator>();
            return services;
        }

        public static IServiceCollection AddInfrastructureLayer(this IServiceCollection services, IConfiguration configuration)
        {
            return InfrastructureServiceRegistration.AddInfrastructureServices(services, configuration);
        }
    }
}
