using AssignmentSubmissionManagement.Core.Interfaces.Repositories;
using AssignmentSubmissionManagement.Core.Interfaces.Services;
using AssignmentSubmissionManagement.Core.Validators;
using AssignmentSubmissionManagement.Infrastructure.Data;
using AssignmentSubmissionManagement.Infrastructure.Repositories;
using AssignmentSubmissionManagement.Infrastructure.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using System.Text;

public partial class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
            ?? builder.Configuration["DATABASE_URL"];

        var useInMemory = builder.Configuration.GetValue("UseInMemoryDatabase", false)
            || string.IsNullOrEmpty(connectionString);

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
        {
            if (useInMemory)
            {
                options.UseInMemoryDatabase("AssignmentMgmtDb");
            }
            else
            {
                options.UseNpgsql(connectionString!);
            }
        });

        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IClassRepository, ClassRepository>();
        builder.Services.AddScoped<IAssignmentRepository, AssignmentRepository>();

        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IClassService, ClassService>();
        builder.Services.AddScoped<IAssignmentService, AssignmentService>();


        builder.Services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();

        var jwtKey = builder.Configuration["Jwt:Key"]
            ?? builder.Configuration["JWT_KEY"]
            ?? throw new InvalidOperationException("JWT key is not configured.");

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "AssignmentApi",
                    ValidAudience = builder.Configuration["Jwt:Audience"] ?? "AssignmentClient",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                };
            });

        builder.Services.AddAuthorization();

        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(
                    new System.Text.Json.Serialization.JsonStringEnumConverter());
            });

        builder.Services.AddOpenApi("v1", options =>
        {
            options.AddDocumentTransformer((document, context, cancellationToken) =>
            {
                document.Info = new OpenApiInfo
                {
                    Title = "Assignment Submission Management API",
                    Version = "v1",
                    Description = "Role-based REST API for managing assignments, submissions and grading."
                };
                return Task.CompletedTask;
            });

            options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
        });

        var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
            ?? new[] { "http://localhost:3000" };

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("Frontend", policy =>
                policy.WithOrigins(allowedOrigins)
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials());
        });

        builder.Services.AddProblemDetails();

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            await DatabaseSeeder.SeedAsync(db, logger);
        }

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference(options =>
            {
                options.WithTitle("Assignment Submission Management API");
                options.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
            });
        }
        else
        {
            app.UseExceptionHandler();
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseCors("Frontend");
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}

internal sealed class BearerSecuritySchemeTransformer(
    Microsoft.AspNetCore.Authentication.IAuthenticationSchemeProvider authenticationSchemeProvider)
    : IOpenApiDocumentTransformer
{
    public async Task TransformAsync(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        var schemes = await authenticationSchemeProvider.GetAllSchemesAsync();
        if (schemes.Any(s => s.Name == JwtBearerDefaults.AuthenticationScheme))
        {
            document.Components ??= new OpenApiComponents();
            var bearerScheme = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description = "Enter your JWT token obtained from /api/auth/login"
            };
            document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
            document.Components.SecuritySchemes["Bearer"] = bearerScheme;

            document.Security ??= new List<OpenApiSecurityRequirement>();
            var requirement = new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
            };
            document.Security.Add(requirement);
        }

        await Task.CompletedTask;
    }
}

