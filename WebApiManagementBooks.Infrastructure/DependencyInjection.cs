using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiManagementBooks.Domain.Interfaces;
using WebApiManagementBooks.Infrastructure.Repositories;


namespace WebApiManagementBooks.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddHttpClient<IBookRepository, BookFakeApiRepository>();
            services.AddHttpClient<IAuthorRepository, AuthorFakeApiRepository>();

            return services;
        }
    }
}
