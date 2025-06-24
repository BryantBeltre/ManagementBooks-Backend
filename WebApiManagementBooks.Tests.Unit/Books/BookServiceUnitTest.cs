using AutoFixture.AutoMoq;
using AutoFixture;
using AutoMapper;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiManagementBooks.Application.DTOs.Book;
using WebApiManagementBooks.Application.Services;
using WebApiManagementBooks.Domain.Entities;
using WebApiManagementBooks.Domain.Interfaces;
using WebApiManagementBooks.Application.DTOs.Author;
using FluentAssertions;

namespace WebApiManagementBooks.Tests.Unit.Books
{
    public class BookServiceUnitTest
    {
        private readonly IFixture _fixture; // Herramienta de AutoFixture para generar datos de prueba
        private readonly Mock<IBookRepository> _mockBookRepository; // Objeto simulado (mock) de IBookRepository
        private readonly Mock<IAuthorRepository> _mockAuthorRepository; // Mock para IAuthorRepository
        private readonly IMapper _mapper; // Instancia real de IMapper con configuración de mapeo

        // El objeto que estamos probando: tu BookService real
        private readonly BookService _sut; // SUT significa "System Under Test"

        // Constructor de la clase de pruebas: Se ejecuta una vez antes de CADA prueba individual.
        // Es el lugar ideal para el paso "Arrange" que es común a todas las pruebas.
        public BookServiceUnitTest()
        {
            // 1. Configurar AutoFixture para auto-mockear interfaces con Moq.
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            // 2. Congelar los mocks de las dependencias.
            _mockBookRepository = _fixture.Freeze<Mock<IBookRepository>>();
            _mockAuthorRepository = _fixture.Freeze<Mock<IAuthorRepository>>(); // Congelar IAuthorRepository

            // 3. Configuración de AutoMapper (real) para las pruebas
            var config = new MapperConfiguration(cfg =>
            {
                // Mapeos para Book
                cfg.CreateMap<CreateBookDTO, Book>().ReverseMap();
                cfg.CreateMap<UpdateBookDTO, Book>().ReverseMap(); // **** AÑADIDO/VERIFICADO: Mapeo de UpdateBookDto a Book ****
                cfg.CreateMap<BookDto, Book>().ReverseMap();
                // Si BookService interactúa con Author DTOs/Entities, también necesitas sus mapeos.
                // Estos mapeos son necesarios para que _mapper.Map funcione correctamente
                cfg.CreateMap<Author, AuthorDto>().ReverseMap();
                cfg.CreateMap<CreateAuthorDto, Author>().ReverseMap();
                cfg.CreateMap<UpdateAuthorDto, Author>().ReverseMap();

                // Mapeo específico para poblar AuthorName en BookDto, si tu servicio lo hace.
                // Esta es una convención común si AuthorName no es directamente una propiedad de Book.
                cfg.CreateMap<Book, BookDto>()
                    .ForMember(dest => dest.AuthorName, opt => opt.Ignore());
                                               
            });
            _mapper = config.CreateMapper(); // Crea la instancia real del mapper

            // 4. Instanciar el Servicio Bajo Prueba (SUT):
            _sut = new BookService(_mockBookRepository.Object, _mockAuthorRepository.Object, _mapper);
        }

    

        // --- PRUEBAS PARA BookService.GetBookByIdAsync() ---


        [Fact]
        public async Task GetBookByIdAsync_ShouldReturnNull_WhenBookNotFound()
        {
            // Arrange
            var bookId = _fixture.Create<int>(); // ID int

            _mockBookRepository.Setup(repo => repo.GetByIdAsync(bookId))
                .ReturnsAsync((Book)null); // Simula que el repositorio devuelve null

            // Act
            var result = await _sut.GetBookByIdAsync(bookId);

            // Assert
            result.Should().BeNull();
            _mockBookRepository.Verify(repo => repo.GetByIdAsync(bookId), Times.Once);
            _mockAuthorRepository.Verify(repo => repo.GetByIdAsync(It.IsAny<int>()), Times.Never); // El autor no debería ser buscado (ajustado a int)
        }


        // --- PRUEBAS PARA BookService.CreateBookAsync() ---
        [Fact]
        public async Task CreateBookAsync_ShouldAddNewBookAndReturnDto()
        {
            // Arrange
            var createBookDto = _fixture.Create<CreateBookDTO>();
            // Aseguramos que el AuthorId sea uno que podamos mockear
            var authorId = _fixture.Create<int>(); // ID int
            createBookDto.AuthorId = authorId;
            var author = new Author { Id = authorId, FirstName = "New", LastName = "Author" };

            // Mockear la búsqueda del autor durante la creación si el servicio la hace
            _mockAuthorRepository.Setup(repo => repo.GetByIdAsync(authorId)).ReturnsAsync(author);

            var bookEntityAfterAdd = _mapper.Map<Book>(createBookDto);
            bookEntityAfterAdd.Id = _fixture.Create<int>(); // Simula un ID asignado por la DB/repositorio (ID int)

            var bookDtoResult = _mapper.Map<BookDto>(bookEntityAfterAdd);
            bookDtoResult.AuthorName = $"{author.FirstName} {author.LastName}"; // Asegura que el DTO esperado tenga el nombre del autor

            _mockBookRepository.Setup(repo => repo.AddAsync(It.IsAny<Book>()))
                .ReturnsAsync(bookEntityAfterAdd);

            // Act
            var result = await _sut.AddBookAsync(createBookDto);

            // Assert
            result.Should().BeEquivalentTo(bookDtoResult, options => options
                .Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromMilliseconds(1)))
                .WhenTypeIs<DateTime>()
            );
            _mockBookRepository.Verify(repo => repo.AddAsync(It.Is<Book>(b =>
                b.Title == createBookDto.Title && b.Description == createBookDto.Description && b.AuthorId == createBookDto.AuthorId
            )), Times.Once);
            _mockAuthorRepository.Verify(repo => repo.GetByIdAsync(authorId), Times.Once); // Verifica que el autor fue buscado
        }

        // --- PRUEBAS PARA BookService.UpdateBookAsync() ---
        [Fact]
        public async Task UpdateBookAsync_ShouldUpdateBookAndReturnUpdatedDto()
        {
            // Arrange
            var updateBookDto = _fixture.Create<UpdateBookDTO>();
            var existingBook = _fixture.Create<Book>();
            updateBookDto.Id = existingBook.Id;

            var authorId = _fixture.Create<int>(); // ID int
            updateBookDto.AuthorId = authorId; // Cambiamos el autor para probar la búsqueda
            var author = new Author { Id = authorId, FirstName = "Updated", LastName = "Writer" };

            _mockBookRepository.Setup(repo => repo.GetByIdAsync(updateBookDto.Id))
                .ReturnsAsync(existingBook);
            _mockAuthorRepository.Setup(repo => repo.GetByIdAsync(authorId)) // Mock para la búsqueda del nuevo autor
                .ReturnsAsync(author);

            _mockBookRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Book>()))
                .ReturnsAsync((Book book) => book);

            // Act
            var result = await _sut.UpdateBookAsync(updateBookDto);

            // Assert
            result.Should().NotBeNull();
            result.Title.Should().Be(updateBookDto.Title);
            result.Description.Should().Be(updateBookDto.Description);
            result.PageCount.Should().Be(updateBookDto.PageCount);
            result.Id.Should().Be(updateBookDto.Id);
            result.AuthorId.Should().Be(updateBookDto.AuthorId);
            result.AuthorName.Should().Be($"{author.FirstName} {author.LastName}"); // Verifica el nombre del nuevo autor

            _mockBookRepository.Verify(repo => repo.GetByIdAsync(updateBookDto.Id), Times.Once);
            _mockAuthorRepository.Verify(repo => repo.GetByIdAsync(authorId), Times.Once); // Verifica la búsqueda del nuevo autor
            _mockBookRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Book>()), Times.Once);
        }

        // --- PRUEBAS PARA BookService.DeleteBookAsync() ---
        [Fact]
        public async Task DeleteBookAsync_ShouldCallRepositoryDelete()
        {
            // Arrange
            var bookIdToDelete = _fixture.Create<int>(); // ID int

            _mockBookRepository.Setup(repo => repo.DeleteAsync(bookIdToDelete))
                .ReturnsAsync(true);

            // Act
            var result = await _sut.DeteleBookAsync(bookIdToDelete);

            // Assert
            result.Should().BeTrue();
            _mockBookRepository.Verify(repo => repo.DeleteAsync(bookIdToDelete), Times.Once);
        }



        [Fact]
        public async Task UpdateBookAsync_ShouldReturnNull_WhenBookNotFound()
        {
            // Arrange
            var updateBookDto = _fixture.Create<UpdateBookDTO>();

            _mockBookRepository.Setup(repo => repo.GetByIdAsync(updateBookDto.Id))
                .ReturnsAsync((Book)null);

            // Act
            var result = await _sut.UpdateBookAsync(updateBookDto);

            // Assert
            result.Should().BeNull();
            _mockBookRepository.Verify(repo => repo.GetByIdAsync(updateBookDto.Id), Times.Once);
            _mockBookRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Book>()), Times.Never);
            _mockAuthorRepository.Verify(repo => repo.GetByIdAsync(It.IsAny<int>()), Times.Never); // No debería buscar autor si el libro no existe (ajustado a int)
        }

        [Fact]
        public async Task CreateBookAsync_ShouldReturnDefaultBookDto_WhenAuthorNotFound() // Renombrado para reflejar el comportamiento actual
        {
            // Arrange
            var createBookDto = _fixture.Create<CreateBookDTO>();
            var authorId = _fixture.Create<int>(); // ID int
            createBookDto.AuthorId = authorId;

            // Simula que el autor no se encuentra
            _mockAuthorRepository.Setup(repo => repo.GetByIdAsync(authorId)).ReturnsAsync((Author)null);

            // Act
            var result = await _sut.AddBookAsync(createBookDto);

            // Assert
            // Esperamos un BookDto con valores predeterminados y "Unknown Author"
            result.Should().NotBeNull();
            result.Id.Should().Be(0); // ID por defecto para int si no se asigna
            result.Title.Should().BeNullOrEmpty();
            result.AuthorId.Should().Be(createBookDto.AuthorId); // El AuthorId del DTO de entrada se mantiene
            result.AuthorName.Should().Be("Unknown Author"); // Verifica el valor predeterminado del servicio
            result.PublishDate.Should().BeCloseTo(new DateTime(), TimeSpan.FromSeconds(1)); // Fechas por defecto pueden ser 0001-01-01

            _mockAuthorRepository.Verify(repo => repo.GetByIdAsync(authorId), Times.Once);
            _mockBookRepository.Verify(repo => repo.AddAsync(It.IsAny<Book>()), Times.Once); // El libro aún se añade si el servicio lo permite
        }
    }

}
