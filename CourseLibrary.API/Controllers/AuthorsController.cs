﻿
using AutoMapper;
using CourseLibrary.API.Entities;
using CourseLibrary.API.Helpers;
using CourseLibrary.API.Models;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Mvc;
using ResourceParameters;

namespace CourseLibrary.API.Controllers;

[ApiController]
[Route("api/authors")]
public class AuthorsController : ControllerBase
{
    private readonly ICourseLibraryRepository _courseLibraryRepository;
    private readonly IMapper _mapper;

    public AuthorsController(
        ICourseLibraryRepository courseLibraryRepository,
        IMapper mapper)
    {
        _courseLibraryRepository = courseLibraryRepository ??
            throw new ArgumentNullException(nameof(courseLibraryRepository));
        _mapper = mapper ??
            throw new ArgumentNullException(nameof(mapper));
    }

    // HttpHead = support for Head requests (response include Headers only without the payload)
    [HttpGet]
    [HttpHead]
    public async Task<ActionResult<IEnumerable<AuthorDto>>> GetAuthors([FromQuery] AuthorsResourceParameters authorsResourceParameters)
    {

        // get authors from repo
        var authorsFromRepo = await _courseLibraryRepository
            .GetAuthorsAsync(authorsResourceParameters);

        // return them
        return Ok(_mapper.Map<IEnumerable<AuthorDto>>(authorsFromRepo));
    }

    [HttpGet("{authorId}", Name = "GetAuthor")]
    public async Task<ActionResult<AuthorDto>> GetAuthor(Guid authorId)
    {
        // get author from repo
        var authorFromRepo = await _courseLibraryRepository.GetAuthorAsync(authorId);

        if (authorFromRepo == null)
        {
            return NotFound();
        }

        // return author
        return Ok(_mapper.Map<AuthorDto>(authorFromRepo));
    }


    // Creates a new author
    [HttpPost]
    public async Task<ActionResult<AuthorDto>> CreateAuthor(AuthorForCreationDto author)
    {
        var authorEntity = _mapper.Map<Entities.Author>(author);

        _courseLibraryRepository.AddAuthor(authorEntity);
        await _courseLibraryRepository.SaveAsync();

        var authorToReturn = _mapper.Map<AuthorDto>(authorEntity);

        return CreatedAtRoute("GetAuthor",
            new { authorId = authorToReturn.Id },
            authorToReturn);
    }

    [HttpOptions]
    public IActionResult GetAuthorsOptions()
    {
        // Return what options we allow in our API. We add this data to the Response Header
        Response.Headers.Add("Allow", "GET,HEAD,POST,OPTIONS");
        return Ok();
    }



}
