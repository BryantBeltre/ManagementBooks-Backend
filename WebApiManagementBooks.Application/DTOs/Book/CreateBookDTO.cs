using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiManagementBooks.Application.DTOs.Book
{
    public class CreateBookDTO
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [MaxLength(1500)]
        public string Description { get; set; }

        public int PageCount { get; set; }

        [MaxLength(1500)]
        public string Excerpt { get; set; }

        public DateTime PublishDate { get; set; } = DateTime.Now;

        public int AuthorId { get; set; } // Para enviar una sugerencia de autor


    }
}
