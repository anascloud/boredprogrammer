using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using SyncSyntax.Models;

namespace BoredProgrammer.Models.ViewModels
{
    public class EditViewModel
    {
        public Post Post { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> Categories { get; set; }
        [ValidateNever]
        public IFormFile FeatureImage { get; set; }
    }
}
