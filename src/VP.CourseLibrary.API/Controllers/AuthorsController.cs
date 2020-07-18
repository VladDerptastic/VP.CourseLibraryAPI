using AutoMapper;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using VP.CourseLibrary.API.DtoModels;
using VP.CourseLibrary.API.ResourceParameters;

namespace VP.CourseLibrary.API.Controllers
{
    [ApiController]
    [Route("api/authors")]
    public class AuthorsController : ControllerBase
    {
        private readonly ICourseLibraryRepository _courseLibraryRepository;
        private readonly IMapper _mapper;

        public AuthorsController(ICourseLibraryRepository courseLibraryRepository, IMapper mapper)
        {
            _courseLibraryRepository = courseLibraryRepository ?? throw new ArgumentNullException(nameof(courseLibraryRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(courseLibraryRepository));
        }

        [HttpGet()]
        [HttpHead]
        public ActionResult<IEnumerable<AuthorDto>> GetAuthors([FromQuery]AuthorsResourceParameters authorsParameters)
        {
            var efAuthors = _courseLibraryRepository.GetAuthors(authorsParameters);

            //return JsonResult(authors);
            //Don't use NotFound codes => if we connect to this method, we're getting an existing resource on the DB even if its empty table
            //NotFound would be confusing
            return Ok(_mapper.Map<IEnumerable<AuthorDto>>(efAuthors));
        }

        [HttpGet("{authorId:guid}")] 
        //[HttpGet("{authorId:int}")] => if we had an int ID as well as a guid, we could use this on another method => Route Constraints
        public ActionResult<AuthorDto> GetAuthor(Guid authorId)
        {
            var author = _courseLibraryRepository.GetAuthor(authorId);

            if (author == null)
                return NotFound();

            return Ok(_mapper.Map<AuthorDto>(author));
        }
    }
}
