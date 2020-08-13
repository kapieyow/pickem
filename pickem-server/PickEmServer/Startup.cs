﻿using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using FluentValidation.AspNetCore;
using Marten;
using Marten.AspNetIdentity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PickEmServer.App;
using PickEmServer.App.Models;
using PickEmServer.Config;
using PickEmServer.Data;
using PickEmServer.Heart;
using PickEmServer.Middleware;
using PickEmServer.WebSockets;
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

            services.AddScoped<GameService>();
            services.AddScoped<LeagueService>();
            services.AddScoped<LogService>();
            services.AddScoped<MartenLogger>();
            services.AddScoped<PickemDatabaseLoggerProvider>();
            services.AddScoped<TeamService>();
            services.AddSingleton<SuperWebSocketPoolManager>();
            services.AddSingleton<PickemEventer>();

            // custom config section loads
            services.AddOptions();
            // Postgres - config
            services.Configure<PostgresConfig>(Configuration.GetSection(nameof(PostgresConfig)));
            var postgresConfig = Configuration.GetSection(nameof(PostgresConfig)).Get<PostgresConfig>();

            // JWT - config
            services.Configure<JwtConfig>(Configuration.GetSection(nameof(JwtConfig)));
            var jwtConfig = Configuration.GetSection(nameof(JwtConfig)).Get<JwtConfig>();

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

                        ValidIssuer = jwtConfig.Issuer,
                        ValidAudience = jwtConfig.Audience,
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
            identBuilder.AddMartenStores<PickEmUser, IdentityRole>(postgresConfig.ConnectionString);

            var serviceProvider = services.BuildServiceProvider();

            // Marten document store -> postgres
            services.AddScoped<IDocumentStore>(provider => DocumentStore.For(_ =>
            {
                _.Connection(postgresConfig.ConnectionString);
                // TODO: by putting this AFTER the identity code above it will put all tables including the asp ones in the "public" schema
                // could separate
                _.DatabaseSchemaName = "public";
                _.Logger(serviceProvider.GetRequiredService<MartenLogger>());
            }));

            // Authorization setup. Used for "god role"
            services.AddAuthorization(options =>
            {
                options.AddPolicy("IsAGod", policy => policy.Requirements.Add(new GodModeRequirement()));
            });

            services.AddSingleton<IAuthorizationHandler, GodModeAuthorizationHandler>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime applicationLifetime, ILogger<Startup> logger, ILoggerFactory loggerFactory, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _logger.LogInformation("Running in ({0}) environment", env.EnvironmentName);

            loggerFactory.AddProvider(serviceProvider.GetService<PickemDatabaseLoggerProvider>());

            app.UseAuthentication();

            // JSON output exception output
            app.UseMiddleware(typeof(JsonOutputErrorHandlingMiddleware));
         
            app.UseStaticFiles();

            var webSocketOptions = new WebSocketOptions()
            {
                KeepAliveInterval = TimeSpan.FromSeconds(Consts.WEB_SOCKET_KEEP_ALIVE_SECONDS),
                ReceiveBufferSize = Consts.WEB_SOCKET_BUFFER_SIZE
            };
            app.UseWebSockets(webSocketOptions);

            app.UseMiddleware<SuperWebSocketMiddleware>();

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
