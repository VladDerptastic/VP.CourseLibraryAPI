using AutoMapper;
using CourseLibrary.API.Entities;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
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

            return CreatedAtRoute(nameof(GetCourseForAuthor),
                new { authorId = authorId, courseId = courseToReturn.Id },
                courseToReturn);
        }

        [HttpPut("{courseId}")]
        public IActionResult UpdateCourseForAuthor(Guid authorId, Guid courseId, CourseForUpdateDto course)
        {
            if (_courseLibraryRepository.AuthorExists(authorId) == false)
                return NotFound();

            var existingCourse = _courseLibraryRepository.GetCourse(authorId, courseId);

            if (existingCourse == null)
            {
                //return NotFound() => if we don't want to allow Upsertion

                //Upsertion allowed
                var courseToAdd = _mapper.Map<Course>(course);
                courseToAdd.Id = courseId;
                _courseLibraryRepository.AddCourse(authorId, courseToAdd);
                _courseLibraryRepository.Save();

                var courseToReturn = _mapper.Map<CourseDto>(courseToAdd);

                return CreatedAtRoute(nameof(GetCourseForAuthor),
                    new { authorId = authorId, courseId = courseToReturn.Id },
                    courseToReturn);
            }

            _mapper.Map(course, existingCourse);
            //UpdateCourse method is empty, the AutoMapper.Map is already setting the entity's state to changed
            //therefore all that's left is to call Save() on the context and it will update the entity
            _courseLibraryRepository.UpdateCourse(existingCourse);
            _courseLibraryRepository.Save();

            return NoContent(); //or Ok() with object representation, both are valid
        }

        [HttpPatch("{courseId}")]
        public ActionResult PartiallyUpdateCourseForAuthor(Guid authorId, Guid courseId, JsonPatchDocument<CourseForUpdateDto> patchDocument)
        {
            if (_courseLibraryRepository.AuthorExists(authorId) == false)
                return NotFound();

            var courseFromRepo = _courseLibraryRepository.GetCourse(authorId, courseId);

            if (courseFromRepo == null)
            {
                //return NotFound() => if we don't want to allow Upsertion

                //Upsert allowed - CARE - if fields are not passed in, they'll keep their null values, since it's PATCH
                var courseDto = new CourseForUpdateDto();
                patchDocument.ApplyTo(courseDto, ModelState);

                if (TryValidateModel(courseDto) == false)
                    //does not take by default into account the ModelState override 
                    //we do in start-up (setupAction.InvalidModelStateResponseFactory)
                    return ValidationProblem(ModelState);

                var courseToAdd = _mapper.Map<Course>(courseDto);
                courseToAdd.Id = courseId;

                _courseLibraryRepository.AddCourse(authorId, courseToAdd);
                _courseLibraryRepository.Save();

                var courseToReturn = _mapper.Map<CourseDto>(courseToAdd);

                return CreatedAtRoute(nameof(GetCourseForAuthor),
                    new { authorId = authorId, courseId = courseToReturn.Id },
                    courseToReturn);
            }

            //map to update object
            var courseToPatch = _mapper.Map<CourseForUpdateDto>(courseFromRepo);

            //apply patch document commands to object, taking into account the model validation
            patchDocument.ApplyTo(courseToPatch, ModelState);

            if(TryValidateModel(courseToPatch) == false)
                //does not take by default into account the ModelState override 
                //we do in start-up (setupAction.InvalidModelStateResponseFactory)
                return ValidationProblem(ModelState); 

            //map back to an entity that can be saved by our repository (while also applying the updates)
            _mapper.Map(courseToPatch, courseFromRepo);
            _courseLibraryRepository.UpdateCourse(courseFromRepo);
            _courseLibraryRepository.Save();

            return NoContent();
        }

        //without this, the default ValidationProblem() does not honor the override of InvalidModelStateResponseFactory in Startup.cs
        public override ActionResult ValidationProblem(
            [ActionResultObjectValue] ModelStateDictionary modelStateDictionary)
        {
            var options = HttpContext.RequestServices.GetRequiredService<IOptions<ApiBehaviorOptions>>();
            return (ActionResult)options.Value.InvalidModelStateResponseFactory(ControllerContext);
        }
    }
}
