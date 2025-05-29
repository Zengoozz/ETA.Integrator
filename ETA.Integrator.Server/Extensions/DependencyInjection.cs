using ETA.Integrator.Server.Data;
using ETA.Integrator.Server.Interface.Repositories;
using ETA.Integrator.Server.Interface.Services;
using ETA.Integrator.Server.Interface.Services.Consumer;
using ETA.Integrator.Server.Repositories;
using ETA.Integrator.Server.Services;
using ETA.Integrator.Server.Services.Consumer;
using Microsoft.EntityFrameworkCore;

namespace ETA.Integrator.Server.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddTransient<ISettingsStepService, SettingsStepService>();
            
            #region ConsumerServices
            services.AddTransient<ISignatureConsumerService, SignatureConsumerService>();
            services.AddTransient<IRequestFactoryConsumerService, RequestFactoryConsumerService>();
            services.AddTransient<IHttpRequestSenderConsumerService, HttpRequestSenderConsumerService>();
            services.AddTransient<IResponseProcessorConsumerService, ResponseProcessorConsumerService>();
            services.AddTransient<IApiConsumerService, ApiConsumerService>();
            #endregion

            return services;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<ISettingsStepRepository, SettingsStepRepository>();

            return services;
        }

        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(connectionString));

            services.Configure<CustomConfigurations>(configuration.GetSection("CustomConfigurations"));

            return services;
        }

        public static IServiceCollection AddApiServices(this IServiceCollection services)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddHttpClient();

            return services;
        }

        public static IServiceCollection AddAllServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddApiServices();
            services.AddRepositories();
            services.AddApplicationServices();
            services.AddPersistence(configuration);

            return services;
        }

    }
}
