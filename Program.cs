using be.Data;
using Microsoft.EntityFrameworkCore;
using be.Services.Auth;
using be.Services.TrainingDomain;
using be.Services.LearningModule;
using be.Services.LearningContent;
using be.Services.LearningProgress;
using be.Services.Assessment;
using be.Services.AssessmentQuestion;
using be.Services.ParticipantAssessment;
using be.Services.SimulationDomain;
using be.Services.SimulationScenario;
using be.Services.SimulationSession;
using be.Services.PerformanceEvaluation;
using be.Services.InstructorFeedback;
using be.Services.Admin;
using be.Models;
using Microsoft.AspNetCore.Identity;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<
    ITrainingDomainService,
    TrainingDomainService>();
    
builder.Services.AddScoped<
    ILearningModuleService,
    LearningModuleService>();

builder.Services.AddScoped<
    ILearningContentService,
    LearningContentService>();

builder.Services.AddScoped<
    ILearningProgressService,
    LearningProgressService>();

builder.Services.AddScoped<
    IAssessmentService,
    AssessmentService>();

builder.Services.AddScoped<
    IAssessmentQuestionService,
    AssessmentQuestionService>();

builder.Services.AddScoped<
    IParticipantAssessmentService,
    ParticipantAssessmentService>();

builder.Services.AddScoped<
    ISimulationDomainService,
    SimulationDomainService>();

builder.Services.AddScoped<
    ISimulationScenarioService,
    SimulationScenarioService>();

builder.Services.AddScoped<
    ISimulationSessionService,
    SimulationSessionService>();

builder.Services.AddScoped<
    IPerformanceEvaluationService,
    PerformanceEvaluationService>();

builder.Services.AddScoped<
    IInstructorFeedbackService,
    InstructorFeedbackService>();

builder.Services.AddScoped<
    IAdminService, 
    AdminService>();

builder.Services.AddControllers();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    builder.Configuration["Jwt:Key"]!
                )
            )
        };
    });

builder.Services.AddAuthorization();

// Add services to the container.
builder.Services.AddOpenApi();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider
        .GetRequiredService<AppDbContext>();

    var passwordHasher = scope.ServiceProvider
        .GetRequiredService<IPasswordHasher<User>>();

    var configuration = scope.ServiceProvider
        .GetRequiredService<IConfiguration>();

    await DbSeeder.SeedAsync(
        dbContext,
        passwordHasher,
        configuration
    );
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild",
    "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast(
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();

    return forecast;
})
.WithName("GetWeatherForecast");
app.MapControllers();
app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}