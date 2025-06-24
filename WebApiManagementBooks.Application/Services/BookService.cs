using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiManagementBooks.Application.DTOs.Book;
using WebApiManagementBooks.Application.Services.Interface;
using WebApiManagementBooks.Domain.Entities;
using WebApiManagementBooks.Domain.Interfaces;

namespace WebApiManagementBooks.Application.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IAuthorRepository _authorRepository;
        private readonly IMapper _mapper;

        public BookService(IBookRepository bookRepository, IAuthorRepository authorRepository, IMapper mapper)
        {
            _bookRepository = bookRepository;
            _authorRepository = authorRepository;
            _mapper = mapper;
        }

        public async Task<BookDto> AddBookAsync(CreateBookDTO createBookDTO)
        {
            var book = _mapper.Map<Book>(createBookDTO);
            var newBook = await _bookRepository.AddAsync(book);

            if (newBook == null) return null;

            var newBookDto = _mapper.Map<BookDto>(newBook);

           
            newBookDto.AuthorId = createBookDTO.AuthorId; // Se toma de la request para el DTO
            var author = await _authorRepository.GetByIdAsync(createBookDTO.AuthorId);
            newBookDto.AuthorName = author != null ? $"{author.FirstName} {author.LastName}" : "Unknown Author";

            return newBookDto;
        }

        public async Task<bool> DeteleBookAsync(int Id)
        {
            return await _bookRepository.DeleteAsync(Id);            
        }

        public async Task<IEnumerable<BookDto>> GetAllAsync()
        {
            var books = await _bookRepository.GetAllAsync();
            var allAuthors = await _authorRepository.GetAllAsync(); // Obtener autores una vez

            var bookDtos = new List<BookDto>();

            foreach (var book in books)
            {
                var bookDto = _mapper.Map<BookDto>(book); // Usar AutoMapper para mapear propiedades directas

                var relatedAuthor = allAuthors.FirstOrDefault(a => a.IdBook == book.Id);
                bookDto.AuthorId = relatedAuthor?.Id ?? 0;
                bookDto.AuthorName = relatedAuthor != null ? $"{relatedAuthor.FirstName} {relatedAuthor.LastName}" : "Unknown Author";
                bookDtos.Add(bookDto);
            }

            return bookDtos;
        }

        public async Task<BookDto> GetBookByIdAsync(int Id)
        {
            var book = await _bookRepository.GetByIdAsync(Id);
            if (book == null) return null;

            var bookDto = _mapper.Map<BookDto>(book);

            var allAuthors = await _authorRepository.GetAllAsync();
            var relatedAuthor = allAuthors.FirstOrDefault(a => a.IdBook == book.Id);
            bookDto.AuthorId = relatedAuthor?.Id ?? 0;
            bookDto.AuthorName = relatedAuthor != null ? $"{relatedAuthor.FirstName} {relatedAuthor.LastName}" : "Unknown Author";

            return bookDto;
        }


        public async Task<BookDto> UpdateBookAsync(UpdateBookDTO updateBookDTO)
        {
            var existingBook = await _bookRepository.GetByIdAsync(updateBookDTO.Id);
            if (existingBook == null) return null;


            var updatedBook = await _bookRepository.UpdateAsync(existingBook);
            if (updatedBook == null) return null;

            _mapper.Map(updateBookDTO, existingBook);

            var updatedBookDto = _mapper.Map<BookDto>(updatedBook);

            updatedBookDto.AuthorId = updateBookDTO.AuthorId;
            var author = await _authorRepository.GetByIdAsync(updateBookDTO.AuthorId);
            updatedBookDto.AuthorName = author != null ? $"{author.FirstName} {author.LastName}" : "Unknown Author";

            return updatedBookDto;
        }
    }
}
