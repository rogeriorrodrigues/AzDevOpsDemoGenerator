using ADOGenerator.IServices;
using ADOGenerator.Models;
using ADOGenerator.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ADOGenerator.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : Controller
    {
        private readonly IProjectService _projectService;
        private readonly ITemplateService _templateService;

        public ProjectsController(IProjectService projectService, ITemplateService templateService)
        {
            _projectService = projectService;
            _templateService = templateService;
        }

        [HttpPost("create")]
        public IActionResult Create([FromBody] ProjectViewModel request)
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
            bool created = _projectService.CreateProjectEnvironment(model);
            return Ok(new { Success = created });
        }

        [HttpPost("artifacts")]
        public IActionResult Artifacts([FromBody] ArtifactRequest request)
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
            var result = _templateService.GenerateTemplateArtifacts(model);
            if (result.Item1)
            {
                return Ok(new { Template = result.Item2, Location = result.Item3 });
            }
            return BadRequest(new { Message = "Artifact generation failed" });
        }
    }

    public record ArtifactRequest(string Organization, string ProjectName, string ProjectId, string AccessToken, string AuthScheme);
}
