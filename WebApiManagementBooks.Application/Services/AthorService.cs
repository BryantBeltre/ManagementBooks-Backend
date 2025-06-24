using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiManagementBooks.Application.DTOs.Author;
using WebApiManagementBooks.Application.Services.Interface;
using WebApiManagementBooks.Domain.Entities;
using WebApiManagementBooks.Domain.Interfaces;

namespace WebApiManagementBooks.Application.Services
{
    public class AthorService : IAuthorService
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;

        public AthorService(IAuthorRepository authorRepository, IBookRepository bookRepository, IMapper mapper)
        {
            _authorRepository = authorRepository;
            _bookRepository = bookRepository;
            _mapper = mapper;
        }

        public async Task<AuthorDto> AddAuthorAsync(CreateAuthorDto createAuthorDTO)
        {
            var author = _mapper.Map<Author>(createAuthorDTO);
            var newAuthor = await _authorRepository.AddAsync(author);
            if (newAuthor == null) return null;

            var newAuthorDTO = _mapper.Map<AuthorDto>(newAuthor);
            newAuthorDTO.BooksCount = 0;

            return newAuthorDTO;
        }

        public async Task<bool> DeteleAuthorAsync(int Id)
        {
            return await _authorRepository.DeleteAsync(Id);            
        }

        public async Task<IEnumerable<AuthorDto>> GetAllAsync()
        {
            var authors = await _authorRepository.GetAllAsync();
            var allBooks = await _bookRepository.GetAllAsync();

            var authorDtos = new List<AuthorDto>();

            foreach (var author in authors)
            {
                var authorDto = _mapper.Map<AuthorDto>(author);

                authorDto.BooksCount = allBooks.Count(b => b.Id == author.IdBook);
                authorDtos.Add(authorDto);
            }
            return authorDtos;
        }

        public async Task<AuthorDto> GetAuthorByIdAsync(int Id)
        {
            var author = await _authorRepository.GetByIdAsync(Id);
            if (author == null) return null;

            var authorDto = _mapper.Map<AuthorDto>(author);

            var allBooks = await _bookRepository.GetAllAsync();
            authorDto.BooksCount = allBooks.Count(book => book.Id == author.IdBook);
            

      
            return authorDto;
        }

        public async Task<AuthorDto> UpdateAuthorAsync(UpdateAuthorDto updatAuthorDTO)
        {
            var existingAuthor = await _authorRepository.GetByIdAsync(updatAuthorDTO.Id);
            if (existingAuthor == null) return null;

            _mapper.Map(updatAuthorDTO, existingAuthor);

            var updatedAuthor = await _authorRepository.UpdateAsync(existingAuthor);
            if (updatedAuthor == null) return null;

            var updatedAuthorDTo = _mapper.Map<AuthorDto>(updatedAuthor);
            var allBooks = await _bookRepository.GetAllAsync();
            updatedAuthorDTo.BooksCount = allBooks.Count(b => b.Id == updatedAuthor.IdBook);

            return updatedAuthorDTo;
        }
    }
}
