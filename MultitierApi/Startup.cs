using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using System;
using Microsoft.AspNetCore.Http;
using FrontendApi.Repositories;
using Polly;
using System.Net.Http;
using Polly.Extensions.Http;

namespace FrontendApi
{
    public class Startup
    {   
        private IConfiguration Configuration { get; set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            //return HttpPolicyExtensions
            //    .HandleTransientHttpError()
            //    .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
            //    .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

            Random jitterer = new Random();
            var retryWithJitterPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(6, // exponential back-off plus some jitter
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                                    + TimeSpan.FromMilliseconds(jitterer.Next(0, 100))
                );

            return retryWithJitterPolicy;
        }
        
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services
                .AddSingleton<IConfiguration>(Configuration)
                .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
                .AddTransient<ITodoRepository, TodoRepository>()
                .AddMvc();

            services
                .AddHttpClient<IHttpClient, StandardHttpClient>()
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))
                .AddPolicyHandler(GetRetryPolicy());

            // Register the Swagger generator, defining one or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Todo Frontend API", Version = "v1" });
            });

            var container = new ContainerBuilder();
            container.Populate(services);

            return new AutofacServiceProvider(container.Build());
        }

        public void Configure(IApplicationBuilder app)
        {
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Todo Frontend API V1");
            });

            //app.UseHttpsRedirection();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseMvc();
        }
    }
}
