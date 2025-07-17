using ADOGenerator.IServices;
using ADOGenerator.Models;
using ADOGenerator.Services;

var builder = WebApplication.CreateBuilder(args);

// Add configuration and services
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

builder.Services.AddSingleton<IProjectService, ProjectService>();
builder.Services.AddSingleton<ITemplateService, TemplateService>();
builder.Services.AddSingleton<IAuthService, AuthService>();

var app = builder.Build();

app.MapPost("/api/projects/create", (ProjectRequest request, IProjectService svc) =>
{
    var model = new Project
    {
        id = Guid.NewGuid().ToString(),
        accountName = request.Organization,
        accessToken = request.AccessToken,
        adoAuthScheme = request.AuthScheme,
        ProjectName = request.ProjectName,
        selectedTemplateFolder = request.TemplateFolder,
        TemplateName = request.TemplateName,
        isExtensionNeeded = request.InstallExtensions,
        isAgreeTerms = request.InstallExtensions
    };
    bool created = svc.CreateProjectEnvironment(model);
    return Results.Ok(new { Success = created });
});

app.MapPost("/api/projects/artifacts", (ArtifactRequest request, ITemplateService templateSvc) =>
{
    var model = new Project
    {
        id = Guid.NewGuid().ToString(),
        accountName = request.Organization,
        ProjectName = request.ProjectName,
        ProjectId = request.ProjectId,
        accessToken = request.AccessToken,
        adoAuthScheme = request.AuthScheme
    };
    var result = templateSvc.GenerateTemplateArtifacts(model);
    if (result.Item1)
    {
        return Results.Ok(new { Template = result.Item2, Location = result.Item3 });
    }
    return Results.BadRequest(new { Message = "Artifact generation failed" });
});

app.Run();

public record ProjectRequest(string Organization, string ProjectName, string TemplateName, string TemplateFolder, string AccessToken, string AuthScheme, bool InstallExtensions);
public record ArtifactRequest(string Organization, string ProjectName, string ProjectId, string AccessToken, string AuthScheme);
