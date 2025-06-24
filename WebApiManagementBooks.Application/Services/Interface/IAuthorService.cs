using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiManagementBooks.Application.DTOs.Author;

namespace WebApiManagementBooks.Application.Services.Interface
{
    public interface IAuthorService
    {
        Task<IEnumerable<AuthorDto>> GetAllAsync();
        Task<AuthorDto> GetAuthorByIdAsync(int Id);
        Task<AuthorDto> AddAuthorAsync(CreateAuthorDto createAuthorDTO);
        Task<AuthorDto> UpdateAuthorAsync(UpdateAuthorDto updatAuthorDTO);
        Task<bool> DeteleAuthorAsync(int Id);
    }
}
