using ADOGenerator.IServices;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace ADOGenerator.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TemplatesController : Controller
    {
        private readonly ITemplateService _templateService;
        public TemplatesController(ITemplateService templateService)
        {
            _templateService = templateService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var list = _templateService.GetAvailableTemplates()
                .Select(t => new { t.Name, t.TemplateFolder, t.Description });
            return Ok(list);
        }
    }
}
