using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiManagementBooks.Domain.Entities;

namespace WebApiManagementBooks.Domain.Interfaces
{
    public interface IBookRepository
    {
        Task<IEnumerable<Book>> GetAllAsync();

        Task<Book> GetByIdAsync(int Id);

        Task<Book> AddAsync(Book book);

        Task<Book> UpdateAsync(Book book);

        Task<bool> DeleteAsync(int Id);
    }
}
