# DLXHub API

This API is built using ASP.NET Core 9.0 following the Vertical Slice Architecture pattern. This architecture organizes code by features rather than technical layers, improving maintainability and reducing coupling.

## Table of Contents

1. [Getting Started](#getting-started)

   - [Prerequisites](#prerequisites)
   - [Development Setup](#development-setup)
   - [Project Structure](#project-structure)
   - [Technology Stack](#technology-stack)

2. [Core Architecture](#core-architecture)

   - [Authentication & Authorization](#authentication--authorization)
   - [Database Configuration](#database-configuration)
   - [API Response Pattern](#api-response-pattern)
   - [Global Exception Handling](#global-exception-handling)
   - [Request Validation](#request-validation)

3. [Domain Features](#domain-features)

   - [Base Entity and Audit System](#base-entity-and-audit-system)
   - [Media Entity Hierarchy](#media-entity-hierarchy)
   - [Slug System](#slug-system)
   - [Genre System](#genre-system)
   - [Page System](#page-system)
   - [Download System](#download-system)
   - [Language System](#language-system)

4. [Infrastructure](#infrastructure)

   - [Soft Delete System](#soft-delete-system)
   - [Pagination System](#pagination-system)
   - [Caching System](#caching-system)
   - [Job System](#job-system)
   - [Feature Flag System](#feature-flag-system)

5. [API Documentation](#api-documentation)

6. [Best Practices](#best-practices)
   - [Security](#security-best-practices)
   - [Error Handling](#error-handling)
   - [Development Guidelines](#development-guidelines)

## Getting Started

### Prerequisites

- .NET 9.0 SDK
- PostgreSQL 16 or later
- Redis 7.0 or later
- A PostgreSQL client (e.g., pgAdmin, DBeaver)

### Development Setup

1. Database Setup:

   - Install PostgreSQL 16 or later
   - Create a new database named 'dlxhub'
   - Update the connection string in `appsettings.json` if needed
   - Update JWT settings in `appsettings.json` with your secure key

2. Application Setup:

   ```bash
   # Clone the repository
   git clone <repository-url>
   cd API

   # Restore packages
   dotnet restore

   # Update database with migrations
   dotnet ef database update

   # Run the application
   dotnet run
   ```

3. Verify Setup:
   - Access API documentation at `https://localhost:5001/scalar/v1`
   - Verify database connection through your PostgreSQL client
   - Test authentication endpoints

### Project Structure

```
DLXHub/
├── API/                    # Backend API project
│   ├── Features/          # Feature-specific code
│   │   └── {FeatureName}/ # Each feature has its own directory
│   │       ├── Commands/  # Write operations
│   │       ├── Queries/   # Read operations
│   │       ├── Models/    # Feature-specific models
│   │       └── Mappings/  # Feature-specific AutoMapper profiles
│   │
│   └── Shared/           # Cross-cutting concerns
│       ├── Behaviors/    # MediatR pipeline behaviors
│       ├── Extensions/   # Extension methods
│       ├── Infrastructure/ # Cross-cutting infrastructure
│       ├── Interfaces/   # Shared interfaces
│       └── Models/       # Shared models
│
└── API.sln               # Solution file
```

### Technology Stack

- **Framework**: ASP.NET Core 9.0
- **Architecture**: Vertical Slice Architecture
- **Database**: PostgreSQL 16
- **Cache**: Redis
- **Authentication**: JWT with ASP.NET Core Identity
- **Packages**:
  - MediatR (12.2.0) - CQRS pattern implementation
  - FluentValidation (11.3.0) - Request validation
  - AutoMapper (12.0.1) - Object mapping
  - Npgsql.EntityFrameworkCore.PostgreSQL (9.0.1) - PostgreSQL provider for EF Core
  - Microsoft.AspNetCore.Identity.EntityFrameworkCore (9.0.2) - Identity framework
  - Microsoft.AspNetCore.Authentication.JwtBearer (9.0.2) - JWT authentication
  - Microsoft.Extensions.Caching.StackExchangeRedis (9.0.2) - Redis cache integration
  - Serilog (8.0.1) - Structured logging
  - Ardalis.GuardClauses (4.5.0) - Defensive programming

## Core Architecture

### Authentication & Authorization

The API uses ASP.NET Core Identity with JWT (JSON Web Tokens) for authentication and authorization. The system includes:

#### Identity Configuration

- Custom `ApplicationUser` extending `IdentityUser`
- Role-based authorization
- Configurable password policies
- User lockout protection
- Email uniqueness enforcement

#### JWT Authentication

Configuration in `appsettings.json`:

```json
{
  "JwtSettings": {
    "SecretKey": "your-super-secret-key-with-at-least-32-characters",
    "Issuer": "DLXHub",
    "Audience": "DLXHub.API",
    "ExpiryInMinutes": 60
  }
}
```

Security Features:

- Token expiration
- Issuer validation
- Audience validation
- Secure key handling
- HTTPS enforcement

### Database Configuration

The application uses PostgreSQL as its primary database. The connection is configured in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=dlxhub;Username=postgres;Password=postgres"
  }
}
```

Entity Framework Core is configured to use snake_case for table and column names to follow PostgreSQL conventions.

### API Response Pattern

All API endpoints return a consistent response format using `ApiResponse<T>`:

```csharp
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public List<string>? Errors { get; set; }
}
```

### Global Exception Handling

The application includes centralized exception handling via middleware that handles:

- Validation exceptions (400 Bad Request)
- Not found exceptions (404 Not Found)
- Unauthorized access (401 Unauthorized)
- Internal server errors (500 Internal Server Error)

### Request Validation

All requests are automatically validated using FluentValidation through the MediatR pipeline.

## Domain Features

### Base Entity and Audit System

All domain entities in the application inherit from `BaseEntity`, which provides automatic auditing and soft-delete capabilities:

#### Features:

1. **Automatic Auditing**:

   - Creation tracking (when and by whom)
   - Modification tracking (when and by whom)
   - Deletion tracking (when and by whom)
   - All audit fields are automatically populated

2. **User References**:

   - Direct navigation properties to ApplicationUser
   - Foreign key relationships with referential integrity
   - Lazy loading support through virtual properties

3. **Soft Delete**:

   - Entities are never physically deleted
   - IsDeleted flag for logical deletion
   - Automatic filtering of deleted entities
   - Deletion time and user tracking

4. **Database Integration**:
   - Proper foreign key constraints
   - Cascade delete prevention
   - snake_case naming convention
   - Global query filters for soft deletes

### Media Entity Hierarchy

The application implements a hierarchical entity structure for media types (movies and TV shows) to promote code reuse and maintainability. The `MediaEntity` class serves as a base class for all media-related entities, inheriting from `BaseEntity` and providing common properties shared between movies and TV shows.

#### Benefits:

1. **Code Reuse**:

   - Eliminates duplication of common properties
   - Centralizes property definitions and documentation
   - Simplifies maintenance and updates

2. **Consistent Data Structure**:

   - Ensures uniform property naming across media types
   - Standardizes property types and nullability
   - Facilitates consistent API responses

3. **Extensibility**:
   - Makes it easy to add new media types (e.g., books, games)
   - Allows for specialized properties in derived classes
   - Supports polymorphic operations on media entities

### Slug System

The application implements a slug system for media entities, providing user-friendly URLs and improved SEO. The `Slug` property is defined in the base `MediaEntity` class, ensuring consistent slug handling across all media types.

#### Features:

1. **URL-Friendly Identifiers**:

   - Human-readable URLs (e.g., `/movies/the-matrix` instead of `/movies/123`)
   - Improved SEO with descriptive URLs
   - Better user experience with meaningful links

2. **Automatic Generation**:

   - Slugs can be automatically generated from titles/names
   - Special characters are removed or replaced
   - Spaces are converted to hyphens
   - Uniqueness is enforced at the database level

3. **Lookup Optimization**:

   - Unique database indexes for efficient lookups
   - NULL values are allowed (with appropriate filter)
   - Fast routing and entity retrieval

4. **Consistency Across Media Types**:
   - Unified slug handling for all media entities
   - Consistent URL patterns for different content types
   - Simplified routing and controller logic

### Genre System

The application implements a comprehensive genre system that allows categorizing movies and TV shows by genre. This is achieved through a many-to-many relationship between media entities and genres.

#### Features:

1. **Unified Genre System**:

   - Single set of genres shared between movies and TV shows
   - Consistent genre IDs across different media types
   - Simplified genre management

2. **Many-to-Many Relationships**:

   - Each movie or TV show can have multiple genres
   - Each genre can be associated with multiple movies and TV shows
   - Efficient database representation

3. **TMDB Integration**:

   - Import genres directly from TMDB
   - Separate genre sets for movies and TV shows
   - Automatic genre mapping

4. **API Endpoints**:
   - Get all genres
   - Get genre by ID
   - Import genres from TMDB (admin only)

### Page System

The application implements a comprehensive page management system that allows creating and managing dynamic pages with versioning support. Each page can have multiple components and supports SEO optimization.

#### Features:

1. **Page Management**:

   - Create and update pages
   - SEO optimization with title and meta description
   - URL-friendly slugs
   - Internal link targets for routing
   - Component-based layout
   - Version control with drafts

2. **Versioning System**:

   - Draft and published states
   - Version tracking
   - Original page reference
   - Publishing workflow
   - Audit trail

3. **Component System**:

   - Flexible component types
   - JSON configuration
   - Ordered layout
   - Unique component IDs
   - Dynamic rendering support

4. **Caching**:
   - Redis caching for published pages
   - Cache by slug and link target
   - Automatic cache invalidation
   - Performance optimization

### Download System

The application implements a comprehensive download system that allows managing downloads for various media types (movies, TV shows, seasons, and episodes). Each download can be associated with specific quality settings and language options.

#### Features:

1. **Flexible Media Association**:

   - Can be linked to movies, TV shows, seasons, or episodes
   - Enforces single media type association
   - Maintains referential integrity
   - Supports cascading deletes

2. **Rich Metadata**:

   - Title for easy identification
   - Language support (e.g., "English", "German", "French")
   - Quality options (e.g., "1080p", "4K", "HDR")
   - Automatic auditing through BaseEntity

3. **Efficient Querying**:

   - Indexed fields for fast filtering
   - Support for complex search queries
   - Optimized database schema

4. **API Support**:
   - Filter downloads by media type
   - Filter by language and quality
   - Sort by various attributes
   - Paginated results

### Language System

The application implements a comprehensive language system that supports multiple languages for content and user interface. This system allows for dynamic language management and content translation.

#### Features:

1. **Language Management**:

   - Add and remove languages
   - Set default language
   - Enable/disable languages
   - Language-specific flag icons
   - ISO code standardization

2. **URL Structure**:

   - Language prefix in URLs (e.g., /de/movies, /en/movies)
   - Automatic redirection to default language
   - SEO-friendly URLs
   - Language-specific content routing

3. **Middleware Integration**:

   - Automatic language detection
   - Language context in requests
   - Default language fallback
   - Language-specific routing

4. **Content Translation**:
   - Support for translated content
   - Language-specific metadata
   - Translation management
   - Fallback to default language

## Infrastructure

### Soft Delete System

The application implements a comprehensive soft delete system that automatically handles entity deletion without physically removing records from the database.

#### Features:

1. **Automatic Soft Delete**:

   - Intercepts all delete operations
   - Converts deletes to updates
   - Sets IsDeleted flag to true
   - Records deletion time and user
   - Maintains referential integrity

2. **Global Filtering**:

   - Automatically excludes soft-deleted entities
   - Applied consistently across all queries
   - No need for manual filtering in queries
   - Works with Include() and navigation properties

3. **Data Recovery**:
   - Soft-deleted data can be recovered
   - Full audit trail of deletion
   - Historical record preservation
   - Compliance with data retention policies

### Pagination System

The application implements a robust pagination system using the `PaginatedList<T>` class, which provides standardized pagination functionality across all list-based API endpoints.

#### Features:

1. **Standardized Pagination**:

   - Consistent pagination across all endpoints
   - Type-safe generic implementation
   - Works with any entity or DTO type
   - Automatic page calculation

2. **Comprehensive Metadata**:

   - Current page number
   - Total number of pages
   - Total count of items
   - Page size
   - Navigation helpers (HasPreviousPage, HasNextPage)

3. **Easy Integration with Entity Framework**:

   - Static CreateAsync method for EF Core queries
   - Efficient SQL generation with Skip/Take
   - Single count query for total items
   - Proper async/await pattern

4. **Input Validation**:
   - Prevents negative page numbers
   - Ensures minimum page size
   - Handles edge cases gracefully

### Caching System

The application uses Redis as a distributed caching solution, providing high-performance data caching with support for various caching patterns and data structures.

#### Features:

1. **Look-aside Cache**:

   - Check cache first
   - If cache miss, get from database
   - Update cache with new value
   - Return result

2. **Cache Invalidation**:

   - Remove on update/delete
   - Prefix-based invalidation for related items
   - Automatic expiration for stale data

3. **Bulk Operations**:
   - Batch cache updates
   - Pipeline commands for performance
   - Atomic operations support

### Job System

The application implements a flexible job system that allows scheduling and executing various maintenance tasks. The system supports both immediate and recurring jobs using cron expressions.

#### Features:

1. **Job Management**:

   - Create and schedule jobs
   - Start and cancel jobs
   - Monitor job status
   - Handle job errors
   - Support for cron expressions
   - Job parameters for configuration

2. **Job Types**:

   - Search Index Updates
   - Sitemap Generation
   - Temporary File Cleanup
   - Extensible for new job types

3. **Scheduling**:

   - One-time execution
   - Recurring execution with cron
   - Automatic next run calculation
   - Job enabling/disabling

4. **Error Handling**:
   - Detailed error messages
   - Automatic status updates
   - Logging of job execution
   - Failed job handling

### Feature Flag System

The application implements a flexible feature flag system that allows dynamic enabling and disabling of features. This system supports both simple boolean flags and more complex configuration-based feature toggles.

#### Features:

1. **Flag Management**:

   - Create and update feature flags
   - Enable/disable features
   - Time-based activation
   - User group targeting
   - Percentage rollouts
   - Configuration storage

2. **Targeting Options**:

   - Global on/off switches
   - Time-based activation windows
   - User group restrictions
   - Percentage-based rollouts
   - Complex configuration support

3. **Caching**:

   - Redis-based caching
   - Automatic cache invalidation
   - Cache duration configuration
   - Distributed cache support

4. **Integration**:
   - Middleware integration
   - MVC filter support
   - API endpoint protection
   - Client-side flag access

## API Documentation

The API is documented using Scalar, which provides a modern and interactive API documentation interface. The documentation is available at `/scalar/v1` when running in development mode. XML comments are automatically included in the documentation.

Authentication is integrated into the Scalar UI, allowing you to:

- Test protected endpoints
- Use JWT tokens for authentication
- View authorization requirements

## Best Practices

### Security Best Practices

1. **Authentication**:

   - Use strong password policies
   - Implement account lockout
   - Enforce email verification
   - Use secure token storage

2. **Authorization**:

   - Implement role-based access control
   - Use policy-based authorization where needed
   - Validate user permissions
   - Implement proper token validation

3. **Data Protection**:

   - Use HTTPS everywhere
   - Implement proper CORS policies
   - Protect against CSRF attacks
   - Secure sensitive data

4. **API Security**:
   - Rate limiting
   - Request validation
   - Input sanitization
   - Proper error handling

### Error Handling

Errors are handled consistently throughout the application:

1. **Validation Errors**: Return 400 Bad Request with validation details
2. **Not Found**: Return 404 Not Found when resources don't exist
3. **Authorization**: Return 401 Unauthorized for authentication issues
4. **Server Errors**: Return 500 Internal Server Error for unhandled exceptions

### Development Guidelines

1. **Database**:

   - Use migrations for database changes
   - Follow PostgreSQL naming conventions (snake_case)
   - Implement appropriate indexes
   - Use appropriate data types

2. **Feature Organization**:

   - Keep all feature-related code together
   - Use CQRS pattern with MediatR
   - Implement feature-specific validators

3. **Validation**:

   - Use FluentValidation for all requests
   - Implement domain validation in command/query handlers

4. **Performance**:

   - Use async/await for I/O operations
   - Implement caching where appropriate
   - Use efficient LINQ queries
   - Consider using database-specific features when appropriate

5. **Documentation**:
   - Use XML comments for all public APIs
   - Keep documentation up-to-date with code changes
   - Include request/response examples
   - Document authentication requirements
   - Document role and policy requirements
