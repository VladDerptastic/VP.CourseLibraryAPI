using System;
using VP.CourseLibrary.API.Enums;

namespace VP.CourseLibrary.API.DtoModels
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public UserRole Role { get; set; }
        public string GoogleId { get; set; }
    }
}
