using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudyGroup.Repository.Data;
using StudyGroup.Repository.Models;
using StudyGroup.Service.Models.Services;
using StudyGroup.Service.Interface;

var builder = WebApplication.CreateBuilder(args);

// ----------------------
// Add services to the container
// ----------------------
builder.Services.AddControllers()
    .AddJsonOptions(x =>
        x.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
    );

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure SQL Server DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// ----------------------
// Register Services for Dependency Injection
// ----------------------

// Lookup service
builder.Services.AddScoped<ILookupService, LookupService>();

// Main entity services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IStudyGroupService, StudyGroupService>();
builder.Services.AddScoped<IGroupMemberService, GroupMemberService>();
builder.Services.AddScoped<IAvailabilityService, AvailabilityService>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<CourseScraperService>();



// ----------------------
// Build app
// ----------------------
var app = builder.Build();

// Configure HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // helpful for debugging
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
