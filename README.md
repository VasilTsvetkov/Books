# 📚 Books

Books is a RESTful API built with ASP.NET Core that allows users to manage a collection of books and submit ratings.  
The API supports full CRUD operations for books and includes endpoints for users to add, update, delete, and retrieve their own ratings.  
Each book also exposes its average rating based on all submitted user ratings.

## 🔧 Features / Available Endpoints

### 📘 Books

- `GET /api/books/all` – Get all books  
- `GET /api/books/{idOrSlug}` – Get a book by ID or slug  
- `POST /api/books` – Create a new book  
- `PUT /api/books/{id}` – Update an existing book  
- `DELETE /api/books/{id}` – Delete a book  

Each book includes:
- `title`, `author`, `yearOfRelease`, `genre`
- `rating` (average from all users)
- `userRating` (rating from the current user)
- `slug` (auto-generated from title and year)

### ⭐ Ratings

- `POST /api/books/{id}/ratings` – Add or update a rating for a book  
- `DELETE /api/books/{id}/ratings` – Remove your rating for a book  
- `GET /api/ratings/me` – Get all ratings submitted by the current user

### 💓 Health

- `GET /_health` – Health check endpoint (returns `200 OK` if the service is running)

## 🛠️ Technologies Used

- **.NET 8**
- **Dapper**
- **SQL Server**
- **FluentValidation**
- **JWT (JSON Web Tokens)**
- **API Versioning**
- **Swagger / Swashbuckle**

Architecture is split into:
- `Books.Api` – Main web API project
- `Books.Application` – Business logic and service layer
- `Books.Contracts` – Shared models and contracts across projects
