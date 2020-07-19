using AutoMapper;
using CourseLibrary.API.Entities;
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

        [HttpGet]
        [HttpHead]
        public ActionResult<IEnumerable<AuthorDto>> GetAuthors([FromQuery]AuthorsResourceParameters authorsParameters)
        {
            var efAuthors = _courseLibraryRepository.GetAuthors(authorsParameters);

            //return JsonResult(authors);
            //Don't use NotFound codes => if we connect to this method, we're getting an existing resource on the DB even if its empty table
            //NotFound would be confusing
            return Ok(_mapper.Map<IEnumerable<AuthorDto>>(efAuthors));
        }

        [HttpGet("{authorId:guid}", Name = "GetAuthor")] 
        //[HttpGet("{authorId:int}")] => if we had an int ID as well as a guid, we could use this on another method => Route Constraints
        public ActionResult<AuthorDto> GetAuthor(Guid authorId)
        {
            var author = _courseLibraryRepository.GetAuthor(authorId);

            if (author == null)
                return NotFound();

            return Ok(_mapper.Map<AuthorDto>(author));
        }

        [HttpPost]
        public ActionResult<AuthorDto> CreateAuthor(AuthorForCreationDto authorToCreate)
        {
            //Using the [ApiController] and ASP.NET Core 3.x - this is done automatically
            //if (authorToCreate == null)
            //    return BadRequest();

            var authorEntity = _mapper.Map<Author>(authorToCreate);
            _courseLibraryRepository.AddAuthor(authorEntity);

            //if the DB is down a 500 server error will be thrown (and a default message provided in middleware)
            _courseLibraryRepository.Save();
            var returnAuthorDto = _mapper.Map<AuthorDto>(authorEntity);
            return CreatedAtRoute("GetAuthor", new { authorId = returnAuthorDto.Id}, returnAuthorDto);
        }

        [HttpOptions]
        public IActionResult GetAuthorsOptions()
        {
            Response.Headers.Add("Allow", "GET,OPTIONS,POST");

            //Successfull OPTIONS call should always result in a 200 OK even if empty
            return Ok();
        }
    }
}
