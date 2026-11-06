using BoredProgrammer.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SyncSyntax.Models
{
    public class Post
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "The Title is Required")]
        [MaxLength(400, ErrorMessage = "The title cannot exceed 400 characters")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "The Content is Required")]
        public string Content { get; set; } = string.Empty;

        [Required(ErrorMessage = "The Author is Required")]
        [MaxLength(100, ErrorMessage = "The Name cannot exceed 100 characters")]
        public string Author { get; set; } = string.Empty;

        [ValidateNever]
        public string FeatureImagePath { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        public DateTime PublishedDate { get; set; } = DateTime.Now;
        [ForeignKey("Category")]

        [DisplayName("Category")]
        public int CategoryId { get; set; }
        [ValidateNever]
        public Category Category { get; set; } = null!;
        [ValidateNever]
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}