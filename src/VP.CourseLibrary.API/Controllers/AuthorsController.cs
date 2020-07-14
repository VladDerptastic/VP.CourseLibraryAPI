using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;

namespace VP.CourseLibrary.API.Controllers
{
    [ApiController]
    [Route("api/authors")]
    public class AuthorsController : ControllerBase
    {
        private readonly ICourseLibraryRepository _courseLibraryRepository;

        public AuthorsController(ICourseLibraryRepository courseLibraryRepository)
        {
            _courseLibraryRepository = courseLibraryRepository ?? throw new ArgumentNullException(nameof(courseLibraryRepository));
        }

        [HttpGet()]
        public IActionResult GetAuthors()
        {
            var authors = _courseLibraryRepository.GetAuthors();
            //return JsonResult(authors);
            //No NotFound codes => if we connect to this method, we're getting an existing resource on the DB even if its empty table
            return Ok(authors);
        }

        [HttpGet("{authorId:guid}")] 
        //[HttpGet("{authorId:int}")] => if we had an int ID as well as a guid, we could use this on another method => Route Constraints
        public IActionResult GetAuthor(Guid authorId)
        {
            var author = _courseLibraryRepository.GetAuthor(authorId);

            if (author == null)
                return NotFound();

            return Ok(author);
        }
    }
}
