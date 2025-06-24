using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiManagementBooks.Application.DTOs.Book;

namespace WebApiManagementBooks.Application.Services.Interface
{
    public interface IBookService
    {
        Task<IEnumerable<BookDto>> GetAllAsync();
        Task<BookDto> GetBookByIdAsync(int Id);
        Task<BookDto> AddBookAsync(CreateBookDTO createBookDTO);
        Task<BookDto> UpdateBookAsync(UpdateBookDTO updateBookDTO);
        Task<bool> DeteleBookAsync(int Id);


    }
}
