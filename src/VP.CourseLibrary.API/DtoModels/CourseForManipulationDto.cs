using System.ComponentModel.DataAnnotations;
using VP.CourseLibrary.API.ValidationAttributes;

namespace VP.CourseLibrary.API.DtoModels
{
    [CourseTitleMustBeDifferentFromDescription(ErrorMessage = "Stop being lazy and make the description different from the title!")]
    public abstract class CourseForManipulationDto
    {
        [Required(ErrorMessage = "You must provide a course title!")]
        [MaxLength(100, ErrorMessage = "Tittle must not exceed 100 characters!")]
        public string Title { get; set; }

        [MaxLength(1500)]
        public virtual string Description { get; set; }
    }
}
