using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using WebApiManagementBooks.Application.DTOs.Author;
using WebApiManagementBooks.Application.Services.Interface;

namespace WebApiManagementBooks.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorService _authorService;

        public AuthorsController(IAuthorService authorService)
        {
            _authorService = authorService;              
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuthorDto>>> GetAuthors()
        {
            var authors = await _authorService.GetAllAsync();
            return Ok(authors);
        }

        
        [HttpGet("{id}")]
        public async Task<ActionResult<AuthorDto>> GetAuthor(int id)
        {
            var author = await _authorService.GetAuthorByIdAsync(id);
            if (author == null) return NotFound();

            return Ok(author);
        }

        [HttpPost]
        public async Task<ActionResult<AuthorDto>> PostAuthor([FromBody] CreateAuthorDto createAuthor)
        {
            if (!ModelState.IsValid) return BadRequest();

            var newAuthor = await _authorService.AddAuthorAsync(createAuthor);
            if (newAuthor == null) return StatusCode(500, "Ha ucurrido un error mientras se agregaba el libro.");

            return CreatedAtAction(nameof(GetAuthor), new { id = newAuthor.Id }, newAuthor);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<AuthorDto>> PutAuthor(int id, [FromBody] UpdateAuthorDto updateAuthor)
        {
            if (id != updateAuthor.Id) return BadRequest("No coincidio el Id");

            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updatedAuthor = await _authorService.UpdateAuthorAsync(updateAuthor);
            if (updateAuthor == null) return NotFound();

            return Ok(updateAuthor);
                        
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            var result = await _authorService.DeteleAuthorAsync(id);
            if (!result) return NotFound();

            return NoContent();
        }
    }
}
