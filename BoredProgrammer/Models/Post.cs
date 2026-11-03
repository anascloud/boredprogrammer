using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BoredProgrammer.Models
{
    public class Post
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage ="The title is required")]
        [MaxLength(400, ErrorMessage ="The title can not exceed 400 characters")]
        public string Title { get; set; }
        [Required(ErrorMessage ="The Content is Required")]
        public string Content { get; set; }
        [Required(ErrorMessage = "The Author is Required")]
        [MaxLength(400, ErrorMessage = "The Name can not exceed 100 characters")]
        public string Author { get; set; }
        [ValidateNever]
        public string FeatureImagePath { get; set; }
        [DataType(DataType.Date)]
        public DateTime PublishedDate { get; set; } = DateTime.Now;
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        [ValidateNever]
        public Category Category { get; set; }
        public ICollection<Comment> Comments { get; set; }
    }
}
