using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiManagementBooks.Application.DTOs.Author;
using WebApiManagementBooks.Domain.Entities;

namespace WebApiManagementBooks.Application.MappingProfiles
{
    public class AuthorProfile : Profile
    {
        public AuthorProfile()
        {
            //Mapeo de Entidad a DTO para GET
            CreateMap<Author, AuthorDto>().ForMember(d => d.BooksCount, o => o.Ignore()).ReverseMap();

            // Mapeo de Request a Entidad para POST 
            CreateMap<CreateAuthorDto, Author>().ReverseMap();

            //Mapeo de Request a Entidad para PUT
            CreateMap<UpdateAuthorDto, Author>().ReverseMap();
            
        }
    }
}
