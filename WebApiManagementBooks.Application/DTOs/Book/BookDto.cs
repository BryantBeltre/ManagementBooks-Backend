using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiManagementBooks.Application.DTOs.Book
{
    public class BookDto
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int PageCount { get; set; }

        public string Excerpt { get; set; }

        public DateTime PublishDate { get; set; }


        public int AuthorId { get; set; } // Propiedad para la relación inferida
        public string AuthorName { get; set; } // Para mostrar en el frontend



    }
}
