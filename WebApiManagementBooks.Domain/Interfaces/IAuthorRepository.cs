using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiManagementBooks.Domain.Entities;

namespace WebApiManagementBooks.Domain.Interfaces
{
    public interface IAuthorRepository
    {
        Task<IEnumerable<Author>> GetAllAsync();

        Task<Author> GetByIdAsync(int Id);

        Task<Author> AddAsync(Author author);

        Task<Author> UpdateAsync(Author author);

        Task<bool> DeleteAsync(int Id);
    }
}
