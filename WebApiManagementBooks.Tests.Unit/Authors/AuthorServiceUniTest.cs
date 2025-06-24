using AutoFixture.AutoMoq;
using AutoFixture;
using AutoMapper;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiManagementBooks.Application.DTOs.Author;
using WebApiManagementBooks.Domain.Interfaces;
using WebApiManagementBooks.Application.Services;
using WebApiManagementBooks.Domain.Entities;
using WebApiManagementBooks.Application.DTOs.Book;
using FluentAssertions;

namespace WebApiManagementBooks.Tests.Unit.Authors
{
    public class AuthorServiceUniTest
    {
        // --- Dependencias que el AuthorService va a usar, que deben ser mockeadas ---
        private readonly IFixture _fixture; // Herramienta de AutoFixture para generar datos de prueba
        private readonly Mock<IAuthorRepository> _mockAuthorRepository; // Objeto simulado (mock) de IAuthorRepository
        private readonly Mock<IBookRepository> _mockBookRepository; // Mock para IBookRepository
        private readonly IMapper _mapper; // Instancia real de IMapper con configuración de mapeo

        // El objeto que estamos probando: tu AuthorService real
        private readonly AthorService _sut; // SUT significa "System Under Test"

        // Constructor de la clase de pruebas: Se ejecuta una vez antes de CADA prueba individual.
        // Es el lugar ideal para el paso "Arrange" que es común a todas las pruebas.
        public AuthorServiceUniTest()
        {
            // 1. Configurar AutoFixture para auto-mockear interfaces con Moq.
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            // 2. Congelar los mocks de las dependencias.
            _mockAuthorRepository = _fixture.Freeze<Mock<IAuthorRepository>>();
            _mockBookRepository = _fixture.Freeze<Mock<IBookRepository>>(); // Congelar IBookRepository

            // 3. Configuración de AutoMapper (real) para las pruebas
            var config = new MapperConfiguration(cfg =>
            {
                // Mapeos para Author
                cfg.CreateMap<CreateAuthorDto, Author>().ReverseMap();
                cfg.CreateMap<UpdateAuthorDto, Author>().ReverseMap();
                cfg.CreateMap<AuthorDto, Author>().ReverseMap();
                // Mapeos para Book (si AuthorService interactúa con Book DTOs/Entities)
                cfg.CreateMap<Book, BookDto>().ReverseMap();
                cfg.CreateMap<CreateBookDTO, Book>().ReverseMap();
                cfg.CreateMap<UpdateBookDTO, Book>().ReverseMap();
            });
            _mapper = config.CreateMapper(); // Crea la instancia real del mapper

            // 4. Instanciar el Servicio Bajo Prueba (SUT):
            // Asegúrate de que el constructor de tu clase AuthorService (en tu proyecto WebApiManagementBooks.Application)
            // tenga EXACTAMENTE esta firma: 'public AuthorService(IAuthorRepository authorRepository, IBookRepository bookRepository, IMapper mapper)'
            _sut = new AthorService(_mockAuthorRepository.Object, _mockBookRepository.Object, _mapper);
        }

        // --- PRUEBAS PARA AuthorService.GetAllAuthorsAsync() ---
        [Fact]
        public async Task GetAllAuthorsAsync_ShouldReturnAllAuthors()
        {
            // Arrange
            var domainAuthors = _fixture.CreateMany<Author>(3).ToList();
            var authorDtos = _mapper.Map<List<AuthorDto>>(domainAuthors); // Usamos el mapper real para generar DTOs

            _mockAuthorRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(domainAuthors);

          
            var result = await _sut.GetAllAsync(); // Asumo que tu servicio tiene un método GetAllAuthorsAsync

            // Assert
            result.Should().BeEquivalentTo(authorDtos);
            _mockAuthorRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
            // Si has añadido setup para _mockBookRepository, verifica su llamada aquí.
        }

        // --- PRUEBAS PARA AuthorService.GetAuthorByIdAsync() ---
        [Fact]
        public async Task GetAuthorByIdAsync_ShouldReturnAuthorDtoWhenFound()
        {
            // Arrange
            var authorId = _fixture.Create<int>(); 
            var domainAuthor = _fixture.Create<Author>();
            var authorDto = _mapper.Map<AuthorDto>(domainAuthor); // Usamos el mapper real

            _mockAuthorRepository.Setup(repo => repo.GetByIdAsync(authorId))
                .ReturnsAsync(domainAuthor);
            // Si tu GetAuthorByIdAsync en AuthorService también consulta IBookRepository (ej. para BooksCount), mockéalo aquí.
            // Por ejemplo:
            // _mockBookRepository.Setup(repo => repo.GetBooksByAuthorId(domainAuthor.Id)).ReturnsAsync(new List<Book>()); // Devuelve libros mock para este autor

            // Act
            var result = await _sut.GetAuthorByIdAsync(authorId);

            // Assert
            result.Should().BeEquivalentTo(authorDto);
            _mockAuthorRepository.Verify(repo => repo.GetByIdAsync(authorId), Times.Once);
            // Si has añadido setup para _mockBookRepository, verifica su llamada aquí también.
        }

        [Fact]
        public async Task GetAuthorByIdAsync_ShouldReturnNull_WhenAuthorNotFound()
        {
            // Arrange
            var authorId = _fixture.Create<int>();

            _mockAuthorRepository.Setup(repo => repo.GetByIdAsync(authorId))
                .ReturnsAsync((Author)null); // Simula que el repositorio devuelve null

            // Act
            var result = await _sut.GetAuthorByIdAsync(authorId);

            // Assert
            result.Should().BeNull();
            _mockAuthorRepository.Verify(repo => repo.GetByIdAsync(authorId), Times.Once);
            // Si tu AuthorService llama a _mockBookRepository incluso si el autor no se encuentra,
            // considera verificar Times.Never para el _mockBookRepository aquí.
        }

        // --- PRUEBAS PARA AuthorService.CreateAuthorAsync() ---
        [Fact]
        public async Task CreateAuthorAsync_ShouldAddNewAuthorAndReturnDto()
        {
            // Arrange
            var createAuthorDto = _fixture.Create<CreateAuthorDto>();
            var authorEntityAfterAdd = _mapper.Map<Author>(createAuthorDto);
            authorEntityAfterAdd.Id = _fixture.Create<int>(); // Simula que la DB asigna un ID

            var authorDtoResult = _mapper.Map<AuthorDto>(authorEntityAfterAdd);

            _mockAuthorRepository.Setup(repo => repo.AddAsync(It.IsAny<Author>()))
                .ReturnsAsync(authorEntityAfterAdd); // Devuelve la entidad con ID asignado

            // Si tu CreateAuthorAsync necesita interactuar con IBookRepository (ej. para validar IdBook), mockéalo aquí.
            // var mockBook = _fixture.Create<Book>();
            // mockBook.Id = createAuthorDto.IdBook; // Asumiendo que IdBook en CreateAuthorDto es Guid
            // _mockBookRepository.Setup(repo => repo.GetByIdAsync(createAuthorDto.IdBook)).ReturnsAsync(mockBook);

            // Act
            var result = await _sut.AddAuthorAsync(createAuthorDto);

            // Assert
            result.Should().BeEquivalentTo(authorDtoResult);
            _mockAuthorRepository.Verify(repo => repo.AddAsync(It.Is<Author>(a =>
                a.FirstName == createAuthorDto.FirstName && a.LastName == createAuthorDto.LastName
            // ... y otras propiedades para asegurar que el mapeo fue correcto
            )), Times.Once);
            // Si has añadido setup para _mockBookRepository, verifica su llamada aquí.
        }

        // --- PRUEBAS PARA AuthorService.UpdateAuthorAsync() ---
        [Fact]
        public async Task UpdateAuthorAsync_ShouldUpdateAuthorAndReturnUpdatedDto()
        {
            // Arrange
            var updateAuthorDto = _fixture.Create<UpdateAuthorDto>();
            var existingAuthor = _fixture.Create<Author>();
            updateAuthorDto.Id = existingAuthor.Id;

            _mockAuthorRepository.Setup(repo => repo.GetByIdAsync(updateAuthorDto.Id))
                .ReturnsAsync(existingAuthor);

            _mockAuthorRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Author>()))
                .ReturnsAsync((Author author) => author);

            // Si tu UpdateAuthorAsync necesita interactuar con IBookRepository (ej. para validar nuevo IdBook), mockéalo aquí.
            // var mockBook = _fixture.Create<Book>();
            // mockBook.Id = updateAuthorDto.IdBook; // Asumiendo que IdBook en UpdateAuthorDto es Guid
            // _mockBookRepository.Setup(repo => repo.GetByIdAsync(updateAuthorDto.IdBook)).ReturnsAsync(mockBook);

            // Act
            var result = await _sut.UpdateAuthorAsync(updateAuthorDto);

            // Assert
            result.Should().NotBeNull();
            result.FirstName.Should().Be(updateAuthorDto.FirstName);
            result.LastName.Should().Be(updateAuthorDto.LastName);
            result.Id.Should().Be(updateAuthorDto.Id);

            _mockAuthorRepository.Verify(repo => repo.GetByIdAsync(updateAuthorDto.Id), Times.Once);
            _mockAuthorRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Author>()), Times.Once);
        }

        // --- PRUEBAS PARA AuthorService.DeleteAuthorAsync() ---
        [Fact]
        public async Task DeleteAuthorAsync_ShouldCallRepositoryDelete()
        {
            // Arrange
            var authorIdToDelete = _fixture.Create<int>();

            _mockAuthorRepository.Setup(repo => repo.DeleteAsync(authorIdToDelete))
                .ReturnsAsync(true); // Asumo que DeleteAsync devuelve true si la eliminación fue exitosa

            // Act
            var result = await _sut.DeteleAuthorAsync(authorIdToDelete);

            // Assert
            result.Should().BeTrue();
            _mockAuthorRepository.Verify(repo => repo.DeleteAsync(authorIdToDelete), Times.Once);
        }

        // --- Manejo de errores (ejemplos) ---
        [Fact]
        public async Task CreateAuthorAsync_ShouldThrowExceptionOnRepositoryFailure()
        {
            // Arrange
            var createAuthorDto = _fixture.Create<CreateAuthorDto>();

            _mockAuthorRepository.Setup(repo => repo.AddAsync(It.IsAny<Author>()))
                .ThrowsAsync(new InvalidOperationException("Simulated database error"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.AddAuthorAsync(createAuthorDto));

            _mockAuthorRepository.Verify(repo => repo.AddAsync(It.IsAny<Author>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAuthorAsync_ShouldReturnNull_WhenAuthorNotFound()
        {
            // Arrange
            var updateAuthorDto = _fixture.Create<UpdateAuthorDto>();

            _mockAuthorRepository.Setup(repo => repo.GetByIdAsync(updateAuthorDto.Id))
                .ReturnsAsync((Author)null);

            // Act
            var result = await _sut.UpdateAuthorAsync(updateAuthorDto);

            // Assert
            result.Should().BeNull();
            _mockAuthorRepository.Verify(repo => repo.GetByIdAsync(updateAuthorDto.Id), Times.Once);
            _mockAuthorRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Author>()), Times.Never); // No debería intentar actualizar
        }
    }
}
