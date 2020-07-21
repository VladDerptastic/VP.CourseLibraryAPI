using System.ComponentModel.DataAnnotations;

namespace VP.CourseLibrary.API.DtoModels
{
    public class CourseForUpdateDto : CourseForManipulationDto
    {
        [Required(ErrorMessage = "You should fill out a description!")]
        public override string Description { get => base.Description; set => base.Description = value; }
    }
}
