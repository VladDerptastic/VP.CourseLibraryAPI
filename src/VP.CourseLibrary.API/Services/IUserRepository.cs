using VP.CourseLibrary.API.DtoModels;

namespace VP.CourseLibrary.API.Services
{
    public interface IUserRepository
    {
        UserDto GetByUsernameAndPassword(string username, string password);
        UserDto GetByGoogleId(string googleId);
    }
}
