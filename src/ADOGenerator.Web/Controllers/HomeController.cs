using ADOGenerator.IServices;
using ADOGenerator.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

namespace ADOGenerator.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ITemplateService _templateService;

        public HomeController(ITemplateService templateService)
        {
            _templateService = templateService;
        }

        public IActionResult Index()
        {
            var vm = new ProjectViewModel();
            vm.TemplateOptions = _templateService.GetAvailableTemplates()
                .Select(t => new SelectListItem { Text = t.Name, Value = t.TemplateFolder });
            return View(vm);
        }
    }
}
