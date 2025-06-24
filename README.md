# WebApiManagementBooks - Backend API

Este proyecto implementa una API REST para la gestión de libros y autores, actuando como un proxy para la API de prueba FakeRestAPI. Sigue una arquitectura de capas (Clean/Onion Architecture) para asegurar la 
separación de preocupaciones y la mantenibilidad del código.

## Estructura del Proyecto

La solución está dividida en los siguientes proyectos:

* **`WebApiManagementBooks`**: La capa de presentación (API REST). Contiene los controladores y la configuración de la aplicación.
* **`WebApiManagementBooks.Application`**: La capa de aplicación. Contiene la lógica de negocio (Servicios), DTOs (Data Transfer Objects) y la configuración de mapeo (AutoMapper).
* **`WebApiManagementBooks.Domain`**: La capa de dominio (el corazón de la aplicación). Contiene las entidades de negocio (Libro, Autor) y las interfaces de repositorio.
* **`WebApiManagementBooks.Infrastructure`**: La capa de infraestructura. Contiene las implementaciones de los repositorios que interactúan con la FakeRestAPI, así como la configuración de inyección de dependencias para los componentes de infraestructura.

## Tecnologías Utilizadas

* **Backend**: C# .NET 8 
* **Arquitectura**: Clean Architecture (con enfoque Onion)
* **Patrones**: Repositorio, Servicio, DTO, Inyección de Dependencias
* **Mapeo de Objetos**: AutoMapper
* **Comunicación Externa**: `System.Net.Http.HttpClient`
* **API Testing**: Swagger/OpenAPI

## Requisitos Previos

* .NET SDK 8
* Visual Studio 2022 (o Visual Studio Code con las extensiones de C#)

## Guía de Instalación y Ejecución

1.  **Clonar el Repositorio:**
    ```bash
    git clone https://github.com/BryantBeltre/ManagementBooks-Backend.git
    cd WebApiManagementBooks
    ```

2.  **Abrir la Solución:**
    Abre el archivo `WebApiManagementBooks.sln` en Visual Studio 2022.

3.  **Restaurar Paquetes NuGet:**
    Visual Studio debería restaurar automáticamente los paquetes NuGet al abrir la solución. Si no, puedes hacerlo manualmente desde la Consola del Administrador de Paquetes
    (Tools > NuGet Package Manager > Package Manager Console) ejecutando:
    ```powershell
    Update-Package -reinstall
    ```
    O haciendo clic derecho en la solución y seleccionando "Restore NuGet Packages".

4.  **Verificar Referencias de Proyectos:**
    Asegúrate de que las referencias entre los proyectos estén configuradas correctamente:
    * `WebApiManagementBooks` -> `WebApiManagementBooks.Application`
    * `WebApiManagementBooks` -> `WebApiManagementBooks.Infrastructure`
    * `WebApiManagementBooks.Application` -> `WebApiManagementBooks.Domain`
    * `WebApiManagementBooks.Infrastructure` -> `WebApiManagementBooks.Domain`

5.  **Configurar Proyecto de Inicio:**
    Asegúrate de que `WebApiManagementBooks` (el proyecto de la API) esté configurado como el proyecto de inicio. Haz clic derecho en el proyecto `WebApiManagementBooks` en el Explorador de soluciones y 
    selecciona "Set as Startup Project".

6.  **Ejecutar la Aplicación:**
    Presiona `F5` o haz clic en el botón "Run" (el triángulo verde) en Visual Studio. Esto iniciará la API y abrirá la interfaz de Swagger UI en tu navegador (normalmente en `https://localhost:XXXX/swagger`, 
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

* La **FakeRestAPI** no siempre persiste los datos de `POST`, `PUT`, `DELETE` de forma permanente. Es una API de simulación.
* La relación entre `Books` y `Authors` en **FakeRestAPI** es peculiar: los `Books` no contienen un `AuthorId` directo, pero los `Authors` contienen un `IdBook`. La lógica de la capa de aplicación (`BookService` y `AuthorService`) ha sido implementada para inferir y presentar esta relación de la mejor manera posible, cumpliendo con el requisito de "mostrar autor relacionado" y "número de libros publicados". Esta es una **interpretación funcional** basada en los datos disponibles de la API externa.
