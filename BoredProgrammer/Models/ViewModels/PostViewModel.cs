using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using SyncSyntax.Models;

namespace BoredProgrammer.Models.ViewModels
{
    public class PostViewModel
    {
        public Post Post { get; set; } = new Post();
        [ValidateNever]
        public IEnumerable<SelectListItem> Categories { get; set; } = new List<SelectListItem>();
        public IFormFile FeatureImage { get; set; } = null!;
    }
}
