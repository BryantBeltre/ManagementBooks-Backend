# WebApiManagementBooks - Backend API

Este proyecto implementa una API REST para la gesti�n de libros y autores, actuando como un proxy para la API de prueba FakeRestAPI. Sigue una arquitectura de capas (Clean/Onion Architecture) para asegurar la 
separaci�n de preocupaciones y la mantenibilidad del c�digo.

## Estructura del Proyecto

La soluci�n est� dividida en los siguientes proyectos:

* **`WebApiManagementBooks`**: La capa de presentaci�n (API REST). Contiene los controladores y la configuraci�n de la aplicaci�n.
* **`WebApiManagementBooks.Application`**: La capa de aplicaci�n. Contiene la l�gica de negocio (Servicios), DTOs (Data Transfer Objects) y la configuraci�n de mapeo (AutoMapper).
* **`WebApiManagementBooks.Domain`**: La capa de dominio (el coraz�n de la aplicaci�n). Contiene las entidades de negocio (Libro, Autor) y las interfaces de repositorio.
* **`WebApiManagementBooks.Infrastructure`**: La capa de infraestructura. Contiene las implementaciones de los repositorios que interact�an con la FakeRestAPI, as� como la configuraci�n de inyecci�n de dependencias para los componentes de infraestructura.

## Tecnolog�as Utilizadas

* **Backend**: C# .NET 8 
* **Arquitectura**: Clean Architecture (con enfoque Onion)
* **Patrones**: Repositorio, Servicio, DTO, Inyecci�n de Dependencias
* **Mapeo de Objetos**: AutoMapper
* **Comunicaci�n Externa**: `System.Net.Http.HttpClient`
* **API Testing**: Swagger/OpenAPI

## Requisitos Previos

* .NET SDK 8
* Visual Studio 2022 (o Visual Studio Code con las extensiones de C#)

## Gu�a de Instalaci�n y Ejecuci�n

1.  **Clonar el Repositorio:**
    ```bash
    git clone https://github.com/BryantBeltre/ManagementBooks-Frontend.git
    cd WebApiManagementBooks
    ```

2.  **Abrir la Soluci�n:**
    Abre el archivo `WebApiManagementBooks.sln` en Visual Studio 2022.

3.  **Restaurar Paquetes NuGet:**
    Visual Studio deber�a restaurar autom�ticamente los paquetes NuGet al abrir la soluci�n. Si no, puedes hacerlo manualmente desde la Consola del Administrador de Paquetes
    (Tools > NuGet Package Manager > Package Manager Console) ejecutando:
    ```powershell
    Update-Package -reinstall
    ```
    O haciendo clic derecho en la soluci�n y seleccionando "Restore NuGet Packages".

4.  **Verificar Referencias de Proyectos:**
    Aseg�rate de que las referencias entre los proyectos est�n configuradas correctamente:
    * `WebApiManagementBooks` -> `WebApiManagementBooks.Application`
    * `WebApiManagementBooks` -> `WebApiManagementBooks.Infrastructure`
    * `WebApiManagementBooks.Application` -> `WebApiManagementBooks.Domain`
    * `WebApiManagementBooks.Infrastructure` -> `WebApiManagementBooks.Domain`

5.  **Configurar Proyecto de Inicio:**
    Aseg�rate de que `WebApiManagementBooks` (el proyecto de la API) est� configurado como el proyecto de inicio. Haz clic derecho en el proyecto `WebApiManagementBooks` en el Explorador de soluciones y 
    selecciona "Set as Startup Project".

6.  **Ejecutar la Aplicaci�n:**
    Presiona `F5` o haz clic en el bot�n "Run" (el tri�ngulo verde) en Visual Studio. Esto iniciar� la API y abrir� la interfaz de Swagger UI en tu navegador (normalmente en `https://localhost:XXXX/swagger`, 
    donde XXXX es el puerto configurado).

## Endpoints de la API

La API expone los siguientes endpoints:

### Libros (Books)

* `GET /api/books`: Obtiene una lista de todos los libros.
* `GET /api/books/{id}`: Obtiene un solo libro por su ID.
* `POST /api/books`: Agrega un nuevo libro.
* `PUT /api/books/{id}`: Actualiza un libro existente.
* `DELETE /api/books/{id}`: Elimina un libro.

### Autores (Authors)

* `GET /api/authors`: Obtiene una lista de todos los autores.
* `GET /api/authors/{id}`: Obtiene un solo autor por su ID.
* `POST /api/authors`: Agrega un nuevo autor.
* `PUT /api/authors/{id}`: Actualiza un autor existente.
* `DELETE /api/authors/{id}`: Elimina un autor.

## Consideraciones sobre FakeRestAPI

* La **FakeRestAPI** no siempre persiste los datos de `POST`, `PUT`, `DELETE` de forma permanente. Es una API de simulaci�n.
* La relaci�n entre `Books` y `Authors` en **FakeRestAPI** es peculiar: los `Books` no contienen un `AuthorId` directo, pero los `Authors` contienen un `IdBook`. La l�gica de la capa de aplicaci�n (`BookService` y `AuthorService`) ha sido implementada para inferir y presentar esta relaci�n de la mejor manera posible, cumpliendo con el requisito de "mostrar autor relacionado" y "n�mero de libros publicados". Esta es una **interpretaci�n funcional** basada en los datos disponibles de la API externa.