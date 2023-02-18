using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ValidationAttributes;


namespace CourseLibrary.API.Models

{
    [CourseTitleNotEqualDescription]
    public abstract class CourseForManipulationDto // : IValidatableObject
    {


        [Required(ErrorMessage = "You should fill in the course title 🤨")]
        [MaxLength(100, ErrorMessage = "Title too long! 🤨")]
        public string Title { get; set; } = string.Empty;
        [MaxLength(1500, ErrorMessage = "Description too long! 🤨")]
        public virtual string? Description { get; set; } = string.Empty;

        // public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        // {
        //     if (Title == Description)
        //     {
        //         // Yield makes sure that the program code continues to run after the this error is raised
        //         // new[] { "Course" } => adds a note about which resource is this error relates to
        //         yield return new ValidationResult("Description can't be the same as the title 🤨", new[] { "Course" });
        //     }
        // }
    }
}