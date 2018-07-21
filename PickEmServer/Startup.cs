using System.Diagnostics;
using System.Reflection;
using System.Text;
using FluentValidation.AspNetCore;
using Marten;
using Marten.AspNetIdentity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using PickEmServer.App;
using PickEmServer.App.Models;
using PickEmServer.Jwt.Models;
using PickEmServer.Middleware;
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
            // Get JWT options from app settings
            var jwtAppSettingOptions = Configuration.GetSection(nameof(JwtIssuerOptions));

            // Configure JwtIssuerOptions
            services.Configure<JwtIssuerOptions>(options =>
            {
                options.Issuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
                options.Audience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)];
            });

            // Jawt auth
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)],
                        ValidAudience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Consts.SECRET_KEY))
                    };
                });

            // MVC
            services
                .AddMvc()
                .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Startup>());

            // CORS
            services.AddCors();

            // Swagga
            services.AddSwaggerGen(c => c.SwaggerDoc("v1", new Info { Title = "PickEm API", Version = "v1" }));

            // postgres conn
            string postgresConnectionString = Configuration.GetSection("PostgresConnection:ConnectionString").Value;

            // Wire in ASP.NET identity using Marten->postgress
            var identBuilder = services
                .AddIdentityCore<PickEmUser>(options =>
                {
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequiredLength = 6;
                });

            identBuilder = new IdentityBuilder(identBuilder.UserType, typeof(IdentityRole), identBuilder.Services);
            identBuilder.AddMartenStores<PickEmUser, IdentityRole>(postgresConnectionString);

            // Marten document store -> postgres
            services.AddScoped<IDocumentStore>(provider => DocumentStore.For(_ =>
            {
                _.Connection(postgresConnectionString);
                // TODO: by putting this AFTER the identity code above it will put all tables including the asp ones in the "public" schema
                // could separate
                _.DatabaseSchemaName = "public";
            }));


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime applicationLifetime, ILogger<Startup> logger)
        {
            _logger = logger;
            _logger.LogInformation("Running in ({0}) environment", env.EnvironmentName);

            app.UseAuthentication();

            if (env.IsDevelopment())
            {
                // JSON output exception output
                app.UseMiddleware(typeof(JsonOutputErrorHandlingMiddleware));
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
