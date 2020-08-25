using System;
using System.Collections.Generic;
using System.Linq;
using VP.CourseLibrary.API.DtoModels;
using VP.CourseLibrary.API.Enums;
using VP.CourseLibrary.Tools;

namespace VP.CourseLibrary.API.Services
{
    public class UserRepository : IUserRepository
    {
        private List<UserDto> users = new List<UserDto>()
        {
            new UserDto
            {
                Id = Guid.Parse("d28888e9-2ba9-473a-a40f-e38cb54f9b35"),
                Name = "Vladdy",
                Username = "notAdmin",
                Password = "4bf764b13b6361eb6fbeb56de0a3152e02afa040fb0e8770e9b76fab2616ce36", //sha256,
                Role = UserRole.Admin,
                GoogleId = "101517359495305583936"
            }
        };

        public UserDto GetByUsernameAndPassword(string username, string password)
        {
            var user = users.SingleOrDefault(u => u.Username == username && u.Password == password.AsSha256());
            return user;
        }
        public UserDto GetByGoogleId(string googleId)
        {
            //TODO: This is just to verify the rest of the logic is working
            return users[0];
        }
    }
}
