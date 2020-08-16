using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VP.CourseLibrary.API.DtoModels
{
    public class LoginDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
