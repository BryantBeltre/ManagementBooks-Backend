using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WebApiManagementBooks.Domain.Entities;
using WebApiManagementBooks.Domain.Interfaces;

namespace WebApiManagementBooks.Infrastructure.Repositories
{
    public class BookFakeApiRepository : IBookRepository
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://fakerestapi.azurewebsites.net/api/v1/Books";
        public BookFakeApiRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<Book>> GetAllAsync()
        {
            var response = await _httpClient.GetAsync(BaseUrl);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Book>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true});
        }

        public async Task<Book> GetByIdAsync(int Id)
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/{Id}");
            if (!response.IsSuccessStatusCode) return null;

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Book>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
        }

        public async Task<Book> AddAsync(Book book)
        {
            var fakeApiBook = new
            {
                title = book.Title,
                description = book.Description,
                pageCount = book.PageCount,
                excerpt = book.Excerpt,
                publishDate = book.PublishDate

            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(fakeApiBook, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync(BaseUrl, jsonContent);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<Book>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<Book> UpdateAsync(Book book)
        {
            var fakeApiBook = new
            {
                id = book.Id,
                title = book.Title,
                description = book.Description,
                pageCount = book.PageCount,
                excerpt = book.Excerpt,
                publishDate = book.PublishDate

            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(fakeApiBook, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PutAsync($"{BaseUrl}/{book.Id}", jsonContent);
            if (!response.IsSuccessStatusCode) return null;
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Book>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<bool> DeleteAsync(int Id)
        {
            var response = await _httpClient.DeleteAsync($"{BaseUrl}/{Id}");
            return response.IsSuccessStatusCode;
        }
    }
}
