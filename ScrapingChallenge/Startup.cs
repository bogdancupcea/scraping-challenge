using System;
using System.IO;
using System.Reflection;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ScrapingChallenge.Application.Scrape.Commands;
using ScrapingChallenge.Application.Scrape.Infrastructure;
using ScrapingChallenge.Application.Scrape.Services;
using ScrapingChallenge.Exceptions;
using ScrapingChallenge.Infrastructure.Context;
using ScrapingChallenge.Infrastructure.Repositories;

namespace ScrapingChallenge
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(options => { options.Filters.Add<GlobalExceptionFilter>(); });

            var connectionString = Configuration["ConnectionStrings:DatabaseConnection"];
            services.AddDbContext<ScrapingDbContext>(options =>
                options.UseNpgsql(connectionString, b => b.MigrationsAssembly("ScrapingChallenge")));

            services.AddSwaggerGen(options =>
            {
                var xmlFile = $"{Assembly.GetEntryAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFile));
            });
            services.AddMediatR(typeof(Startup).Assembly, typeof(ScrapeMenuCommandHandler).Assembly);
            services.AddScoped<IScrapingService, ScrapingService>();
            services.AddScoped<IMenuItemRepository, MenuItemRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                c.RoutePrefix = string.Empty;
            });
        }
    }
}
