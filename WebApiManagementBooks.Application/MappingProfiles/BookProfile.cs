using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiManagementBooks.Application.DTOs.Author;
using WebApiManagementBooks.Application.DTOs.Book;
using WebApiManagementBooks.Domain.Entities;

namespace WebApiManagementBooks.Application.MappingProfiles
{
    public class BookProfile : Profile
    {
        public BookProfile()
        {
            //Mapeo de Entidad a DTO para GET
            CreateMap<Book, BookDto>().ForMember(dest => dest.AuthorName, o => o.Ignore()).ForMember(d => d.AuthorId, o => o.Ignore()).ReverseMap();

            // Mapeo de Request a Entidad para POST 
            CreateMap<CreateBookDTO, Book>().ReverseMap();

            //Mapeo de Request a Entidad para PUT
            CreateMap<UpdateBookDTO, Book>().ReverseMap();
        }
    }
}
