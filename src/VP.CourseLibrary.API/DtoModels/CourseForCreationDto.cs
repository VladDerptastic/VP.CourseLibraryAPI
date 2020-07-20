using System.ComponentModel.DataAnnotations;
using VP.CourseLibrary.API.ValidationAttributes;

namespace VP.CourseLibrary.API.DtoModels
{
    [CourseTitleMustBeDifferentFromDescription(ErrorMessage = "Stop being lazy and make the description different from the title!")]
    public class CourseForCreationDto// : IValidatableObject
    {
        [Required(ErrorMessage = "You must provide a course title!")]
        [MaxLength(100, ErrorMessage = "Tittle must not exceed 100 characters!")]
        public string Title { get; set; }
        [MaxLength(1500)]
        public string Description { get; set; }

        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{
        //    if (Title == Description)
        //    {
        //        yield return new ValidationResult(
        //            "The provided description should be different from the title.",
        //            new[] { "CourseForCreationDto"});
        //    }
        //}
    }
}
