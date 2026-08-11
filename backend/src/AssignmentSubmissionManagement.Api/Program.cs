using Microsoft.EntityFrameworkCore;
using AssignmentSubmissionManagement.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

app.MapGet("/", () => Results.Ok("API running"));
app.Run();

