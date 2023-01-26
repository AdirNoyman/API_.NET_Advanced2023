using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CourseLibrary.API.Entities;
using CourseLibrary.API.Helpers;
using CourseLibrary.API.Models;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CourseLibrary.API.Controllers
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
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        // Get a list of authors
        [HttpGet("({authorsIds})", Name = "GetAuthorsCollection")]
        public async Task<ActionResult<IEnumerable<AuthorForCreationDto>>> GetAuthorsCollection([ModelBinder(BinderType = typeof(ArrayModelBinder))][FromRoute] IEnumerable<Guid> authorsIds)
        {
            // Load to memory all the existing authors from the DB
            var authorEntities = await _courseLibraryRepository.GetAuthorsAsync(authorsIds);

            // Check if we have all the authors that ere passed in the http request
            if (authorsIds.Count() != authorEntities.Count())
            {
                return NotFound("Sorry, didn't find the authors 😩");
            }

            // Map the authors we got from the DB to a list of authordtos and return it to the client
            var authorsToReturn = _mapper.Map<IEnumerable<AuthorDto>>(authorEntities);

            return Ok(authorsToReturn);
        }

        // Creates a collection of new authors 
        [HttpPost]
        public async Task<ActionResult<IEnumerable<AuthorDto>>> CreateAuthorsCollection(IEnumerable<AuthorForCreationDto> authorsCollection)
        {
            var authorEntities = _mapper.Map<IEnumerable<Author>>(authorsCollection);
            foreach (var author in authorEntities)
            {
                _courseLibraryRepository.AddAuthor(author);
            }

            await _courseLibraryRepository.SaveAsync();

            var authorsCollectionToReturn = _mapper.Map<IEnumerable<AuthorDto>>(authorEntities);

            // Create a list of authors ID's
            var authorsIdsAsString = string.Join(",", authorsCollectionToReturn.Select(a => a.Id));

            return CreatedAtRoute("GetAuthorsCollection", new { authorsIds = authorsIdsAsString }, authorsCollectionToReturn);
        }
    }
}