
using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using FluentValidation.AspNetCore;
using Marten;
using Marten.AspNetIdentity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using PickEmServer.App.Models;
using PickEmServer.Jwt;
using PickEmServer.Jwt.Models;
using Swashbuckle.AspNetCore.Swagger;

namespace PickEmServer
{
    public class Startup
    {

        private ILogger _logger;
        private string _runningSignature;

        private const string SecretKey = "iNivDmHLpUA223sqsfhqGbMRdRj1PVkH"; // TODO: get this from somewhere secure
        private readonly SymmetricSecurityKey _signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging();

            services
                .AddMvc()
                .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Startup>());

            services.AddCors();
            services.AddSwaggerGen(c => c.SwaggerDoc("v1", new Info { Title = "PickEm API", Version = "v1" }) );

            string postgresConnectionString = Configuration.GetSection("PostgresConnection:ConnectionString").Value;

            services.AddSingleton<IJwtFactory, JwtFactory>();

            // jwt wire up
            // Get options from app settings
            var jwtAppSettingOptions = Configuration.GetSection(nameof(JwtIssuerOptions));

            // Configure JwtIssuerOptions
            services.Configure<JwtIssuerOptions>(options =>
            {
                options.Issuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
                options.Audience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)];
                options.SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);
            });

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)],

                ValidateAudience = true,
                ValidAudience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)],

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _signingKey,

                RequireExpirationTime = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(configureOptions =>
            {
                configureOptions.ClaimsIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
                configureOptions.TokenValidationParameters = tokenValidationParameters;
                configureOptions.SaveToken = true;
            });

            // api user claim policy
            services.AddAuthorization(options =>
            {
                options.AddPolicy("ApiUser", policy => policy.RequireClaim(JwtClaimIdentifiers.Rol, JwtClaims.ApiAccess));
            });

            // Wire in ASP.NET identity using Marten->postgress
            services
                .AddIdentity<PickEmUser, IdentityRole>(i =>
                    {
                        i.Password.RequireDigit = false;
                        i.Password.RequireLowercase = false;
                        i.Password.RequireUppercase = false;
                        i.Password.RequireNonAlphanumeric = false;
                        i.Password.RequiredLength = 6;
                    }
                )
                .AddMartenStores<PickEmUser, IdentityRole>(postgresConnectionString)
                .AddDefaultTokenProviders();

            // Marten document store
            services.AddScoped<IDocumentStore>(provider => DocumentStore.For(postgresConnectionString));
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

            app.UseAuthentication();
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
