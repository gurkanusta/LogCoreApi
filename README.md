# LogCoreApi

LogCoreApi is a backend-focused ASP.NET Core Web API project built to demonstrate real-world backend architecture and practices.

The project is designed for learning and showcasing clean and professional backend development rather than frontend complexity.

---

## Tech Stack

- ASP.NET Core Web API (.NET 8)
- Entity Framework Core with SQL Server
- ASP.NET Identity
- JWT Authentication and Authorization
- AutoMapper
- FluentValidation
- Serilog (Logging)
- Swagger / OpenAPI

---

## Features

- User registration and login
- JWT-based authentication
- Role-based authorization (Admin / User)
- Notes CRUD API
- DTO-based request and response models
- Global exception handling
- Centralized validation using FluentValidation
- Structured logging with Serilog
- Swagger UI with JWT support

---

## Architecture Highlights

- Clean separation of concerns
- DTO usage instead of exposing entities
- Global exception middleware
- Custom validation and error handling
- Scalable and maintainable structure

---

## Purpose

This project was built to practice professional backend development and to serve as a reference project for internships and junior backend roles.

---

## Running the Project

1. Configure the SQL Server connection string in `appsettings.json`
2. Run database migrations
3. Start the application
4. Test endpoints via Swagger UI

---

## Default Admin User

Email: admin@logcoreapi.com  
Password: Admin12345

---

## License

This project is intended for educational and demonstration purposes.
