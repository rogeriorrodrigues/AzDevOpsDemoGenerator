using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace ADOGenerator.Web.ViewModels
{
    public class ProjectViewModel
    {
        public string Organization { get; set; }
        public string ProjectName { get; set; }
        public string TemplateName { get; set; }
        public string TemplateFolder { get; set; }
        public string AccessToken { get; set; }
        public string AuthScheme { get; set; } = "pat";
        public bool InstallExtensions { get; set; } = true;
        public IEnumerable<SelectListItem> TemplateOptions { get; set; } = new List<SelectListItem>();
        public string ResultMessage { get; set; }
    }
}
