
using System.Diagnostics;
using System.Reflection;
using FluentValidation.AspNetCore;
using Marten;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;

namespace PickEmServer
{
    public class Startup
    {

        private ILogger _logger;
        private string _runningSignature;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvc()
                .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Startup>());

            services.AddCors();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Pickem API", Version = "v1" });
            });


            // TODO: do this the right way. Should this be here?
            // Marten document store
            services.AddScoped<IDocumentStore>(provider =>
                DocumentStore.For(Configuration.GetSection("PostgresConnection:ConnectionString").Value));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime applicationLifetime, ILogger<Startup> logger)
        {
            _logger = logger;
            _logger.LogInformation("Running in ({0}) environment", env.EnvironmentName);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            // allow requests from any origin
            app.UseCors(builder =>
            {
                builder.AllowAnyOrigin();
                builder.AllowAnyMethod();
                builder.AllowAnyHeader();
            });

            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Pickem API");
            });

            applicationLifetime.ApplicationStarted.Register(OnApplicationStarted);
            applicationLifetime.ApplicationStopping.Register(OnApplicationStopping);
        }

        public void OnApplicationStarted()
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(executingAssembly.Location);

            _runningSignature = string.Format("{0} [{1}]",
                executingAssembly.GetCustomAttribute<AssemblyProductAttribute>().Product,
                fileVersionInfo.ProductVersion
                );

            _logger.LogInformation("{0}: Started", _runningSignature);
        }

        public void OnApplicationStopping()
        {
            _logger.LogInformation("{0}: Stopping", _runningSignature);
        }
    }
}
