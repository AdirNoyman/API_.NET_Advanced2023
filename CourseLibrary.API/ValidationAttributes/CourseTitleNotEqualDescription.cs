using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using CourseLibrary.API.Models;

namespace ValidationAttributes
{
    public class CourseTitleNotEqualDescription : ValidationAttribute
    {

        public CourseTitleNotEqualDescription()
        {


        }

        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {

            if (validationContext.ObjectInstance is not CourseForManipulationDto course)
            {
                throw new Exception($"Attribute " + $"{nameof(CourseTitleNotEqualDescription)} " + $"must be applied to a " + $"{nameof(CourseForManipulationDto)} or derived type.");
            }

            if (course.Title == course.Description)
            {
                return new ValidationResult("The provided description should be diffrent form the title", new[] { nameof(CourseForManipulationDto) });
            }

            return ValidationResult.Success;
        }

    }
}