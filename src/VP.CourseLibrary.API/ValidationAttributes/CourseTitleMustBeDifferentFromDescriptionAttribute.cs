using System.ComponentModel.DataAnnotations;
using VP.CourseLibrary.API.DtoModels;

namespace VP.CourseLibrary.API.ValidationAttributes
{
    public class CourseTitleMustBeDifferentFromDescriptionAttribute : ValidationAttribute
    {
        //also consider - https://fluentvalidation.net/ - by Jeremy Skimmer
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var course = (CourseForCreationDto)validationContext.ObjectInstance;

            if (course.Title == course.Description)
            {
                return new ValidationResult(ErrorMessage,
                    new[] { "CourseForCreationDto" });
            }

            return ValidationResult.Success;
        }
    }
}
