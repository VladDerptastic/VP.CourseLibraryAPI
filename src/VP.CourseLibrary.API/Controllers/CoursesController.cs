using AutoMapper;
using CourseLibrary.API.Entities;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using VP.CourseLibrary.API.DtoModels;

namespace VP.CourseLibrary.API.Controllers
{
    [ApiController]
    [Route("api/authors/{authorId}/courses")]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseLibraryRepository _courseLibraryRepository;
        private readonly IMapper _mapper;

        public CoursesController(ICourseLibraryRepository courseLibraryRepository, IMapper mapper)
        {
            _courseLibraryRepository = courseLibraryRepository ?? throw new ArgumentNullException(nameof(courseLibraryRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(courseLibraryRepository));
        }

        [HttpGet]
        public ActionResult<IEnumerable<CourseDto>> GetCoursesForAuthor(Guid authorId)
        {
            if (_courseLibraryRepository.AuthorExists(authorId) == false)
                return NotFound();
            
            var coursesForAuthor = _courseLibraryRepository.GetCourses(authorId);
            return Ok(_mapper.Map<IEnumerable<CourseDto>>(coursesForAuthor));
        }

        [HttpGet("{courseId}", Name = nameof(GetCourseForAuthor))]
        public ActionResult<CourseDto> GetCourseForAuthor(Guid authorId, Guid courseId)
        {
            if (_courseLibraryRepository.AuthorExists(authorId) == false)
                return NotFound();

            var course = _courseLibraryRepository.GetCourse(authorId, courseId);

            if (course == null)
                return NotFound();

            return Ok(_mapper.Map<CourseDto>(course));
        }

        [HttpPost]
        public ActionResult<CourseDto> CreateCourseForAuthor(Guid authorId, CourseForCreationDto courseToCreate)
        {
            if (_courseLibraryRepository.AuthorExists(authorId) == false)
                return NotFound();

            var courseEntity = _mapper.Map<Course>(courseToCreate);
            _courseLibraryRepository.AddCourse(authorId, courseEntity);
            _courseLibraryRepository.Save();

            var courseToReturn = _mapper.Map<CourseDto>(courseEntity);

            return CreatedAtRoute(nameof(GetCourseForAuthor), new { authorId = authorId, courseId = courseToReturn.Id}, courseToReturn);
        }

        [HttpPut("{courseId}")]
        public ActionResult UpdateCourseForAuthor(Guid authorId, Guid courseId, CourseForUpdateDto course)
        {
            if (_courseLibraryRepository.AuthorExists(authorId) == false)
                return NotFound();

            var existingCourse = _courseLibraryRepository.GetCourse(authorId, courseId);

            if (existingCourse == null)
                return NotFound();

            _mapper.Map(course, existingCourse);
            //UpdateCourse method is empty, the AutoMapper.Map is already setting the entity's state to changed
            //therefore all that's left is to call Save() on the context and it will update the entity
            _courseLibraryRepository.UpdateCourse(existingCourse);
            _courseLibraryRepository.Save();

            return NoContent(); //or Ok() with object representation, both are valid
        }
    }
}
