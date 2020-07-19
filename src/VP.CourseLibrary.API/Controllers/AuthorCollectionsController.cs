using AutoMapper;
using CourseLibrary.API.Entities;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using VP.CourseLibrary.API.DtoModels;
using VP.CourseLibrary.API.Helpers;

namespace VP.CourseLibrary.API.Controllers
{
    [ApiController]
    [Route("api/authorcollections")]
    public class AuthorCollectionsController : ControllerBase
    {
        private readonly ICourseLibraryRepository _courseLibraryRepository;
        private readonly IMapper _mapper;

        public AuthorCollectionsController(ICourseLibraryRepository courseLibraryRepository, IMapper mapper)
        {
            _courseLibraryRepository = courseLibraryRepository ?? throw new ArgumentNullException(nameof(courseLibraryRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(courseLibraryRepository));
        }

        [HttpGet("({ids})", Name = "GetAuthorCollection")]
        public ActionResult<IEnumerable<AuthorDto>> GetAuthorCollection(
            [FromRoute] 
            [ModelBinder(BinderType = typeof(ArrayModelBinder))]
            IEnumerable<Guid> ids)
        {
            if (ids == null)
                return BadRequest();

            var authorEntities = _courseLibraryRepository.GetAuthors(ids);

            if (ids.Count() != authorEntities.Count())
                return NotFound();

            var authorsToReturn = _mapper.Map<IEnumerable<AuthorDto>>(authorEntities);
            return Ok(authorsToReturn);
        }

        [HttpPost]
        public ActionResult<IEnumerable<AuthorDto>> CreateAuthorCollection(IEnumerable<AuthorForCreationDto> authorsCollection)
        {
            //Using the [ApiController] and ASP.NET Core 3.x - this is done automatically
            //if (authorToCreate == null)
            //    return BadRequest();

            var authorEntities = _mapper.Map<IEnumerable<Author>>(authorsCollection);
            foreach (var author in authorEntities)
            {
                _courseLibraryRepository.AddAuthor(author);
            }
            _courseLibraryRepository.Save();

            var returnAuthorsDto = _mapper.Map<IEnumerable<AuthorDto>>(authorEntities);
            var idsAsString = string.Join(",", returnAuthorsDto.Select(x => x.Id));
            return CreatedAtRoute("GetAuthorCollection", new { ids = idsAsString}, returnAuthorsDto);

        }     
    }
}
