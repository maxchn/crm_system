using CrmSystem.DAL.Contexts;
using CrmSystem.DAL.Entities;
using CrmSystem.DAL.Interfaces;
using CrmSystem.DAL.UnitOfWork;
using CrmSystem.Server.GraphQL;
using CrmSystem.Server.Hubs;
using CrmSystem.Server.Models;
using CrmSystem.Server.Queries;
using CrmSystem.Server.Services;
using GraphQL.Server;
using GraphQL.Server.Ui.GraphiQL;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace CrmSystem.Server
{
    public class Startup
    {
        public const string GraphQlPath = "/graphql";
        public const string GraphiQlPath = "/ui/graphiql";

        public IConfiguration Configuration { get; }
        public IHostingEnvironment Environment { get; }


        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            Environment = hostingEnvironment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var npgsqlConnectionString = Configuration.GetSection("NpgsqlSqlConnection:ConnectionString").Value;
            services.AddDbContext<NpgsqlDbContext>(options => options.UseNpgsql(connectionString: npgsqlConnectionString), ServiceLifetime.Singleton);

            // Add email services
            services.AddTransient<IEmailSender, EmailSender>();

            services.AddTransient<IUnitOfWork, NpsqlUnitOfWork>();

            // Add Role Manager
            services.AddScoped<ApplicationRoleManager>();

            // Add Identity
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<NpgsqlDbContext>()
                .AddTokenProvider(
                    TokenOptions.DefaultProvider,
                    typeof(DataProtectorTokenProvider<ApplicationUser>));

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireDigit = true;
                options.Password.RequiredUniqueChars = 3;
                options.Password.RequiredLength = 6;

                // User settings
                options.User.RequireUniqueEmail = true;
            });

            // Add Jwt Authentication
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            // Configure Authentication
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = Configuration["JwtIssuer"],
                        ValidateAudience = true,
                        ValidAudience = Configuration["JwtIssuer"],
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtKey"])),
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];

                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) &&
                                path.StartsWithSegments("/chatHub"))
                            {
                                context.Request.Headers["Authorization"] = accessToken.ToString();
                                context.Token = accessToken.ToString();
                            }

                            return System.Threading.Tasks.Task.CompletedTask;
                        }
                    };
                });

            services.AddGraphQL(options =>
                {
                    options.EnableMetrics = true;
                    options.ExposeExceptions = Environment.IsDevelopment();
                })
                .AddGraphQLAuthorization(options =>
                {
                    options.AddPolicy("Authorized", p => p.RequireAuthenticatedUser());
                })
                .AddUserContextBuilder(httpContext => httpContext.User);

            // Add CORS
            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                builder =>
                {
                    builder
                        .WithOrigins("http://localhost:4200")
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            // Add SignalR
            services.AddSignalR();

            services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = int.MaxValue;
                x.MultipartHeadersLengthLimit = int.MaxValue;
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddTransient<AppSchema>();
            services.AddTransient<AppQuery>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("Access-Control-Allow-Origin", "http://localhost:4200");
                await next.Invoke();
            });

            // Shows UseCors with named policy.
            app.UseCors("AllowSpecificOrigin");

            app.UseSignalR(routes =>
            {
                routes.MapHub<ChatHub>("/chatHub");
            });

            app.UseHttpsRedirection();

            // Enabling use Authentication
            app.UseAuthentication();

            app.UseStaticFiles();

            app.UseGraphQL<AppSchema>(GraphQlPath);
            app.UseGraphiQLServer(new GraphiQLOptions
            {
                GraphiQLPath = GraphiQlPath,
                GraphQLEndPoint = GraphQlPath
            });

            app.UseMvc();
        }
    }
}