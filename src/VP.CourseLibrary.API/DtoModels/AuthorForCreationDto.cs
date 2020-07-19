using System;
using System.Collections.Generic;

namespace VP.CourseLibrary.API.DtoModels
{
    public class AuthorForCreationDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTimeOffset DateOfBirth { get; set; }
        public string MainCategory { get; set; }
        public List<CourseForCreationDto> Courses { get; set; } = new List<CourseForCreationDto>();
    }
}
