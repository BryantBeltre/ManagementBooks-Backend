using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApiManagementBooks.Application.DTOs.Book;
using WebApiManagementBooks.Application.Services.Interface;

namespace WebApiManagementBooks.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetBooks()
        {
            var books = await _bookService.GetAllAsync();
            return Ok(books);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BookDto>> GetBook(int id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null) return NotFound();
            return Ok(book);
        }

        [HttpPost]
        public async Task<ActionResult<BookDto>> PostBook([FromBody] CreateBookDTO createBook)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newBook = await _bookService.AddBookAsync(createBook);
            if (newBook == null)
            {
                return StatusCode(500, "Ha ucurrido un error mientras se agregaba el libro.");
            }
            return CreatedAtAction(nameof(GetBook), new { id = newBook.Id }, newBook);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<BookDto>> PutBook(int id, [FromBody] UpdateBookDTO updateBook)
        {
            if (id != updateBook.Id) return BadRequest("El id no coincide");

            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updatedBook = await _bookService.UpdateBookAsync(updateBook);
            if (updateBook == null) return NotFound();

            return Ok(updateBook);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var result = await _bookService.DeteleBookAsync(id);
            if (!result) return NotFound();

            return NoContent();            
            
        }

    }
}
