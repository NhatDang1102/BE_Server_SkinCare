# SkinSense - API Server

A comprehensive ASP.NET Core Web API for a skincare application featuring AI-powered recommendations, routine management, and e-commerce functionality.

## Features

### Core Functionality
- **User Management**: Registration, authentication, profile management with JWT tokens
- **Product Catalog**: Complete CRUD operations for skincare products and categories
- **AI Integration**: OpenAI-powered skincare and facial images analysis for generating routines and product recommendations
- **Routine Management**: Daily skincare routine tracking with feedback system, with tracking usages for each session
- **Blog System**: Content management with commenting functionality
- **Payment Integration**: VIP subscription system with PayOS payment gateway
- **Admin Dashboard**: Administrative controls and analytics

### Advanced Features
- **Redis Caching**: Session management and performance optimization
- **Rate Limiting**: API protection and abuse prevention
- **Image Upload**: Cloudinary integration for product and user images
- **Email Services**: Password reset and notification system
- **Firebase Integration**: Additional authentication options
- **Export Functionality**: Excel data export capabilities

## Technology Stack

### Backend Framework
- **ASP.NET Core 8.0** - Main web framework
- **Entity Framework Core** - Database ORM
- **SQL Server** - Primary database

### Authentication & Security
- **JWT Bearer Authentication** - Token-based security
- **Firebase Admin SDK** - Additional auth provider
- **Rate Limiting** - API protection

### External Services
- **OpenAI API** - AI-powered recommendations
- **Redis** - Caching and session management
- **Cloudinary** - Image storage and optimization
- **PayOS** - Payment processing
- **SMTP** - Email services

### Architecture
- **Clean Architecture** - 4-layer separation (Main, Service, Repository, Contract)
- **Dependency Injection** - IoC container pattern
- **Repository Pattern** - Data access abstraction
- **DTO Pattern** - Data transfer optimization

## Project Structure

```
├── Main/                   # API Controllers and Program.cs
├── Service/               # Business logic layer
├── Repository/            # Data access layer
├── Contract/              # DTOs and shared contracts
└── README.md
```
## API Documentation

**Live API Documentation**: [https://skincareapp.somee.com/swagger/index.html](https://skincareapp.somee.com/swagger/index.html)

Test the API endpoints directly using the interactive Swagger interface.

## Development
### Code Style
- Follow C# coding conventions
- Use dependency injection
- Implement proper error handling
- Maintain clean architecture principles

## Performance Features

- Redis caching for frequent queries
- Rate limiting for API protection
- Optimized database queries
- Image optimization with Cloudinary
- JWT token blacklisting for security

## Security

- JWT-based authentication
- Role-based authorization
- Input validation and sanitization
- Rate limiting protection
- Secure password handling
- CORS configuration

