using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiManagementBooks.Application.DTOs.Author
{
    public class AuthorDto
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        public int BooksCount { get; set; } = 0; // Propiedad calculada, no de la API externa


    }
}
