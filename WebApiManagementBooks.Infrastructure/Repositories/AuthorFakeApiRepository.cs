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
    public class AuthorFakeApiRepository : IAuthorRepository
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://fakerestapi.azurewebsites.net/api/v1/Authors";

        public AuthorFakeApiRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<Author>> GetAllAsync()
        {
            var response = await _httpClient.GetAsync(BaseUrl);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Author>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<Author> GetByIdAsync(int Id)
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/{Id}");
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Author>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<Author> AddAsync(Author author)
        {
            var fakeApiAuthor = new
            {
                idBook = author.IdBook, // Utiliza IdBook de tu entidad
                firstName = author.FirstName,
                lastName = author.LastName
            };

            var jsonContent = new StringContent(
                JsonSerializer.Serialize(fakeApiAuthor, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
                Encoding.UTF8,
                "application/json"
            );
            var response = await _httpClient.PostAsync(BaseUrl, jsonContent);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Author>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        }

        public async Task<Author> UpdateAsync(Author author)
        {
            var fakeApiAuthor = new
            {
                id = author.Id,
                idBook = author.IdBook,
                firstName = author.FirstName,
                lastName = author.LastName
            };

            var jsonContent = new StringContent(
                JsonSerializer.Serialize(fakeApiAuthor, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
                Encoding.UTF8,
                "application/json"
            );
            var response = await _httpClient.PutAsync($"{BaseUrl}/{author.Id}", jsonContent);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Author>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<bool> DeleteAsync(int Id)
        {
            var response = await _httpClient.DeleteAsync($"{BaseUrl}/{Id}");
            return response.IsSuccessStatusCode;
        }
    }
}
