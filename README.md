# Book Management System

A full-stack **Book Management and Borrowing System** built with
**ASP.NET Core Web API** and **Angular**.\
This project demonstrates modern full-stack development practices
including **JWT authentication**, **role-based access**, **clean
architecture**, and **Angular Material UI**.

------------------------------------------------------------------------

## Tech Stack

### Backend (API)

-   ASP.NET Core 8 Web API
-   C#
-   Entity Framework Core
-   SQL Server / LocalDB
-   JWT Authentication
-   Repository + Service pattern
-   Integration and xUnit testing

### Frontend (Client)

-   Angular 16+
-   TypeScript
-   Angular Material
-   HTTP Interceptors
-   Route Guards
-   Standalone components
-   Testing using Karma

------------------------------------------------------------------------

## Core Features

### Authentication

-   User registration
-   Secure login with JWT
-   Password hashing
-   Role-based access (Admin / User)

### Book Management

-   Create, update, delete books (Admin)
-   View books list
-   Book details page
-   Search and filtering
-   Restore book

### Borrowing System

-   Borrow books
-   Return books
-   View active borrowings

------------------------------------------------------------------------

## Project Structure

    BookManagement
    |
    |-- BookManagement.Api              # ASP.NET Core Web API
    |-- BookManagement.Api.Tests        # Integration & unit tests
    |
    |-- Book-Management-UI              # Angular frontend
    |
    |-- README.md

------------------------------------------------------------------------

## Prerequisites

Install the following:

-   .NET SDK 8+

-   Node.js (18+ recommended)

-   Angular CLI

        npm install -g @angular/cli

-   SQL Server or LocalDB

-   Git

------------------------------------------------------------------------

## Backend Setup (.NET API)

1.  Navigate to the API folder:

```{=html}
<!-- -->
```
    cd BookManagement.Api

2.  Restore dependencies:

```{=html}
<!-- -->
```
    dotnet restore

3.  Apply database migrations:

```{=html}
<!-- -->
```
    dotnet ef database update

4.  Run the API:

```{=html}
<!-- -->
```
    dotnet run

------------------------------------------------------------------------

## Frontend Setup (Angular)

1.  Navigate to the Angular project:

```{=html}
<!-- -->
```
    cd book-management-client

2.  Install dependencies:

```{=html}
<!-- -->
```
    npm install

3.  Start the Angular app:

```{=html}
<!-- -->
```
    ng serve

Frontend will run at:

    http://localhost:4200

------------------------------------------------------------------------

## Default Test Users

  Role    Username   Password
  ------- ---------- -----------
  Admin   admin      Admin@123
  User    user       User@123

------------------------------------------------------------------------

## API Endpoints

### Auth

    POST /api/auth/register (Admin only)
    POST /api/auth/login
    GET /api/auth/users (Admin only)

### Books

    GET    /api/books
    GET    /api/books/{id}
    POST   /api/books               (Admin only)
    PUT    /api/books/{id}          (Admin only)
    DELETE /api/books/{id}          (Admin only)
    POST   /api/books/{id}/restore  (Admin only)

### Borrowing

    POST /api/BookBorrowedDetails/borrow            (Admin only)
    POST /api/BookBorrowedDetails/return            (Admin only)
    GET  /api/BookBorrowedDetails/active            (Admin only)
    GET  /api/BookBorrowedDetails/overdue           (Admin only)
    GET  /api/BookBorrowedDetails/overdue/count     (Admin only)

------------------------------------------------------------------------

## Running Tests

### Backend Tests (.NET)

Navigate to the test project:

cd BookManagement.Api.Tests
dotnet test

Includes:
- Integration tests
- Create, Update and Delete workflow tests
- Borrow/return workflow tests
- Authentication scenarios


### Frontend Tests (Angular)

Navigate to the Angular project:

cd Book-Management-UI

Run unit tests:

ng test

This will:
- Launch the Angular test runner
- Execute component and service tests
- Show coverage and results in the browser

### Frontend Test Coverage

Run:

ng test --code-coverage

Coverage report will be generated in:

/coverage/index.html

------------------------------------------------------------------------

## Architecture Overview

Controller → Service → Repository → Database

Principles used: - SOLID - Dependency Injection - DTOs - Secure JWT
authentication - Angular interceptors for token handling

------------------------------------------------------------------------

## User Guide
The system user guide is available here:

[Download User Guide](docs/User%20Guide.docx)


## Author

**Udaya Susumi M Manage**\
Full-Stack Developer

GitHub: https://github.com/SusumiManage\

------------------------------------------------------------------------

