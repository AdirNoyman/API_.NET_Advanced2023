
using AutoMapper;
using CourseLibrary.API.Models;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CourseLibrary.API.Controllers;

[ApiController]
[Route("api/authors/{authorId}/courses")]
public class CoursesController : ControllerBase
{
    private readonly ICourseLibraryRepository _courseLibraryRepository;
    private readonly IMapper _mapper;

    public CoursesController(ICourseLibraryRepository courseLibraryRepository,
        IMapper mapper)
    {
        _courseLibraryRepository = courseLibraryRepository ??
            throw new ArgumentNullException(nameof(courseLibraryRepository));
        _mapper = mapper ??
            throw new ArgumentNullException(nameof(mapper));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CourseDto>>> GetCoursesForAuthor(Guid authorId)
    {
        if (!await _courseLibraryRepository.AuthorExistsAsync(authorId))
        {
            return NotFound();
        }

        var coursesForAuthorFromRepo = await _courseLibraryRepository.GetCoursesAsync(authorId);
        return Ok(_mapper.Map<IEnumerable<CourseDto>>(coursesForAuthorFromRepo));
    }

    // The Name is just alias name for the route
    [HttpGet("{courseId}", Name = "GetCourseForAuthor")]
    public async Task<ActionResult<CourseDto>> GetCourseForAuthor(Guid authorId, Guid courseId)
    {
        if (!await _courseLibraryRepository.AuthorExistsAsync(authorId))
        {
            return NotFound();
        }

        var courseForAuthorFromRepo = await _courseLibraryRepository.GetCourseAsync(authorId, courseId);

        if (courseForAuthorFromRepo == null)
        {
            return NotFound();
        }
        return Ok(_mapper.Map<CourseDto>(courseForAuthorFromRepo));
    }


    [HttpPost]
    public async Task<ActionResult<CourseDto>> CreateCourseForAuthor(
            Guid authorId, CourseForCreationDto course)
    {
        if (!await _courseLibraryRepository.AuthorExistsAsync(authorId))
        {
            return NotFound();
        }

        var courseEntity = _mapper.Map<Entities.Course>(course);
        _courseLibraryRepository.AddCourse(authorId, courseEntity);
        await _courseLibraryRepository.SaveAsync();

        var courseToReturn = _mapper.Map<CourseDto>(courseEntity);
        // Return response that displays the route to the created resource and the JSON of the resource created
        return CreatedAtRoute("GetCourseForAuthor", new { authorId, courseId = courseToReturn.Id }, courseToReturn);

    }


    [HttpPut("{courseId}")]
    public async Task<IActionResult> UpdateCourseForAuthor(Guid authorId,
      Guid courseId,
      CourseForUpdateDto course)
    {
        // Check if the requested author exist in the DB
        if (!await _courseLibraryRepository.AuthorExistsAsync(authorId))
        {
            return NotFound();
        }

        // Check if the requested course exist in the DB
        var courseForAuthorFromRepo = await _courseLibraryRepository.GetCourseAsync(authorId, courseId);

        // If course doesn't exist => create it!
        if (courseForAuthorFromRepo == null)
        {
            // Copy the course dto to a new entry in course table
            var courseToAdd = _mapper.Map<Entities.Course>(course);
            // Assign the new course to the Id that was passed by the client
            courseToAdd.Id = courseId;
            _courseLibraryRepository.AddCourse(authorId, courseToAdd);
            await _courseLibraryRepository.SaveAsync();

            var courseToReturn = _mapper.Map<CourseDto>(courseToAdd);
            // Return response that displays the route to the created resource and the JSON of the resource created
            return CreatedAtRoute("GetCourseForAuthor", new { authorId, courseId = courseToReturn.Id }, courseToReturn);

        }

        // Map course dto => course DB entity
        _mapper.Map(course, courseForAuthorFromRepo);

        _courseLibraryRepository.UpdateCourse(courseForAuthorFromRepo);

        await _courseLibraryRepository.SaveAsync();
        return NoContent();
    }

    [HttpPatch("{courseId}")]
    public async Task<IActionResult> PartiallyUpdateCourseForAuthor(Guid authorId, Guid courseId, JsonPatchDocument<CourseForUpdateDto> patchDocument)
    {
        // Check if the requested author exist in the DB
        if (!await _courseLibraryRepository.AuthorExistsAsync(authorId))
        {
            return NotFound();
        }

        // Check if the requested course exist in the DB
        var courseForAuthorFromRepo = await _courseLibraryRepository.GetCourseAsync(authorId, courseId);

        // If course doesn't exist => create it!
        if (courseForAuthorFromRepo == null)
        {
            var courseDto = new CourseForUpdateDto();
            // Apply json patch data to the CourseForUpdateDto
            patchDocument.ApplyTo(courseDto);
            var courseToAdd = _mapper.Map<Entities.Course>(courseDto);
            // Assign the new course to the Id that was passed by the client
            courseToAdd.Id = courseId;

            _courseLibraryRepository.AddCourse(authorId, courseToAdd);
            await _courseLibraryRepository.SaveAsync();

            var courseToReturn = _mapper.Map<CourseDto>(courseToAdd);
            // Return response that displays the route to the created resource and the JSON of the resource created
            return CreatedAtRoute("GetCourseForAuthor", new { authorId, courseId = courseToReturn.Id }, courseToReturn);



        }

        // Map course DB entity => courseToPatch dto. Loading the course DB entity requested entry, to CourseForUpdateDto POCO
        var courseToPatch = _mapper.Map<CourseForUpdateDto>(courseForAuthorFromRepo);
        // Applying the data requested changes (updates) we got from the JSON to the CourseForUpdateDto POCO
        patchDocument.ApplyTo(courseToPatch);
        // Map courseToPatch dto => courseForAuthorFromRepo DB entity
        _mapper.Map(courseToPatch, courseForAuthorFromRepo);
        // Update the repository in memory with the planned changes to the DB course entity
        _courseLibraryRepository.UpdateCourse(courseForAuthorFromRepo);
        // Commit the changes in the repository to the database
        await _courseLibraryRepository.SaveAsync();
        // Return successful result
        return NoContent();
    }

    [HttpDelete("{courseId}")]
    public async Task<ActionResult> DeleteCourseForAuthor(Guid authorId, Guid courseId)
    {
        if (!await _courseLibraryRepository.AuthorExistsAsync(authorId))
        {
            return NotFound();
        }

        var courseForAuthorFromRepo = await _courseLibraryRepository.GetCourseAsync(authorId, courseId);

        if (courseForAuthorFromRepo == null)
        {
            return NotFound();
        }

        _courseLibraryRepository.DeleteCourse(courseForAuthorFromRepo);
        await _courseLibraryRepository.SaveAsync();

        return NoContent();
    }

}