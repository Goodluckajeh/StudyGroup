# StudyGroup API 

A modern, feature-rich ASP.NET Core Web API for managing study groups with intelligent matching, instant joining, and comprehensive authentication & authorization.

## Features

- ** JWT Authentication** - Secure user authentication with flexible token handling
- ** Smart Tag Matching** - Automatic skill and course tag generation for intelligent group discovery
- ** Instant Joining** - No approval required - join groups immediately
- ** Role-Based Authorization** - Creator-only operations with privacy protection
- ** Public Discovery** - Browse study groups without authentication
- ** Clean Architecture** - 4-layer architecture with proper separation of concerns
- ** RESTful API** - Comprehensive CRUD operations with rich JSON responses
- ** Swagger Documentation** - Interactive API documentation with JWT integration

## Project Structure

```
StudyGroup/
├── StudyGroup.Api/              # API Layer - Controllers, Middleware, Configuration
│   ├── Controllers/             # REST API Controllers
│   ├── Authorization/           # Custom authorization attributes
│   └── Services/               # API-specific services (AutoTagging)
├── StudyGroup.Service/          # Business Logic Layer
│   ├── Services/               # Business logic implementations
│   ├── Interfaces/             # Service contracts
│   ├── DTOs/                   # Data Transfer Objects
│   └── Settings/               # Configuration models
├── StudyGroup.Data/             # Data Access Layer
│   ├── Repositories/           # Dapper-based data access
│   ├── Interfaces/             # Repository contracts
│   ├── Models/                 # Database entity models
│   └── SqlQueries/             # Parameterized SQL queries
└── DatabaseProject/             # Database Schema & Scripts
    ├── dbo/Tables/             # Table definitions
    └── dbo/Scripts/            # Setup and seed scripts
```

## Installation & Setup

### 1. Clone the Repository
```bash
git clone https://github.com/Goodluckajeh/StudyGroup.git
cd StudyGroup
```

### 2. Build the Solution
```bash
dotnet build
```

## Configuration

### 1. Configure Connection String

Create or update `appsettings.json` in the root directory:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=StudyGroupDB;Trusted_Connection=true;MultipleActiveResultSets=true;"
  },
  "Jwt": {
    "Key": "YourSuperSecretKeyThatShouldBeAtLeast32CharactersLong123456789",
    "Issuer": "StudyGroupAPI",
    "Audience": "StudyGroupUsers",
    "ExpirationHours": 24
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### 2. Environment-Specific Configuration

For **Production**, create `appsettings.Production.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Your-Production-Connection-String"
  },
  "Jwt": {
    "Key": "Your-Production-JWT-Secret-Key-Make-It-Very-Secure",
    "Issuer": "StudyGroupAPI-Prod",
    "Audience": "StudyGroupUsers-Prod",
    "ExpirationHours": 24
  }
}
```

## Database Setup

### 1. Create Database Tables

Run the following SQL scripts in order (found in `DatabaseProject/dbo/Tables/`):

```sql
-- Create the main database
CREATE DATABASE StudyGroupDB;
USE StudyGroupDB;

-- Create EntityTypes table first (required for foreign keys)
CREATE TABLE EntityTypes (
    EntityTypeId INT IDENTITY(1,1) PRIMARY KEY,
    EntityTypeName NVARCHAR(50) NOT NULL UNIQUE
);

-- Create MembershipStatus table
CREATE TABLE MembershipStatus (
    StatusId INT IDENTITY(1,1) PRIMARY KEY,
    StatusName NVARCHAR(20) NOT NULL UNIQUE
);

-- Create Users table
CREATE TABLE Users (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    Skills NVARCHAR(MAX),
    Visibility BIT,
    Bio NVARCHAR(500)
);

-- Create Courses table
CREATE TABLE Courses (
    CourseId INT IDENTITY(1,1) PRIMARY KEY,
    CourseName NVARCHAR(100) NOT NULL UNIQUE,
    CourseDescription NVARCHAR(500)
);

-- Create StudyGroups table
CREATE TABLE StudyGroups (
    GroupId INT IDENTITY(1,1) PRIMARY KEY,
    CourseId INT FOREIGN KEY REFERENCES Courses(CourseId),
    CreatorId INT FOREIGN KEY REFERENCES Users(UserId),
    Topic NVARCHAR(200),
    TimeSlot NVARCHAR(100),
    Description NVARCHAR(500)
);

-- Create GroupMembers table
CREATE TABLE GroupMembers (
    GroupMemberId INT IDENTITY(1,1) PRIMARY KEY,
    GroupId INT FOREIGN KEY REFERENCES StudyGroups(GroupId) ON DELETE CASCADE,
    UserId INT FOREIGN KEY REFERENCES Users(UserId),
    StatusId INT FOREIGN KEY REFERENCES MembershipStatus(StatusId)
);

-- Create Tags table
CREATE TABLE Tags (
    TagId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL,
    EntityTypeId INT FOREIGN KEY REFERENCES EntityTypes(EntityTypeId),
    EntityId INT NOT NULL
);
```

### 2. Seed Initial Data

```sql
-- Insert EntityTypes for tagging system
INSERT INTO EntityTypes (EntityTypeName) VALUES 
('User'),       -- EntityTypeId = 1
('StudyGroup'); -- EntityTypeId = 2

-- Insert MembershipStatus options
INSERT INTO MembershipStatus (StatusName) VALUES 
('Pending'),    -- StatusId = 1
('Approved'),   -- StatusId = 2  
('Rejected');   -- StatusId = 3

-- Insert sample courses
INSERT INTO Courses (CourseName, CourseDescription) VALUES 
('Introduction to Psychology', 'Basic principles of psychology and human behavior'),
('Calculus I', 'Differential and integral calculus'),
('Computer Science 101', 'Introduction to programming and computer science concepts'),
('Biology 101', 'Fundamentals of biological sciences');
```

## Running the Application

### Development Mode
```bash
dotnet run
```

The API will be available at:
- **HTTPS**: `https://localhost:7001`
- **HTTP**: `http://localhost:5000`
- **Swagger UI**: `https://localhost:7001/swagger`

### Production Mode
```bash
dotnet run --environment Production
```

## API Documentation

### Swagger/OpenAPI
- **URL**: `https://localhost:7001/swagger`
- **Interactive Documentation**: Test endpoints directly in browser
- **JWT Authentication**: Built-in authentication support

### Accessing Swagger:
1. Start the application (`dotnet run`)
2. Navigate to `https://localhost:7001/swagger`
3. Use the "Authorize" button to enter JWT tokens
4. Test endpoints interactively

### Key Swagger Features:
- **JWT Integration** - Authenticate directly in Swagger
- **Interactive Testing** - Execute API calls with real data
- **Comprehensive Docs** - Detailed endpoint descriptions and examples
- **Real-time Testing** - Immediate feedback on API responses

## Authentication Guide

### 1. Register a New User
```http
POST /api/auth/register
Content-Type: application/json

{
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@example.com",
  "password": "SecurePassword123!",
  "skills": "Java, Python, Web Development",
  "bio": "Computer Science student looking for study groups"
}
```

### 2. Login to Get JWT Token
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "john.doe@example.com",
  "password": "SecurePassword123!"
}
```

**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "message": "Login successful",
  "user": {
    "userId": 1,
    "firstName": "John",
    "lastName": "Doe",
    "email": "john.doe@example.com"
  }
}
```

### 3. Use JWT Token in Requests
```http
GET /api/studygroups/available
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

**Alternative formats supported:**
- `Bearer {token}` (standard)
- `{token}` (direct token)

## API Endpoints

### Public Endpoints (No Authentication)
| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/studygroups` | Browse all study groups |
| `GET` | `/api/studygroups/{id}` | View specific study group |
| `GET` | `/api/studygroups/by-course/{courseName}` | Find groups by course/topic |
| `POST` | `/api/auth/register` | Register new user |
| `POST` | `/api/auth/login` | Login user |

### Authenticated Endpoints
| Method | Endpoint | Description | Authorization |
|--------|----------|-------------|---------------|
| `GET` | `/api/users/{id}` | Get user profile | Owner only |
| `PUT` | `/api/users/{id}` | Update user profile | Owner only |
| `POST` | `/api/studygroups` | Create study group | Authenticated |
| `GET` | `/api/studygroups/available` | Get joinable groups | Authenticated |
| `GET` | `/api/studygroups/my-groups` | Get created groups | Authenticated |
| `PUT` | `/api/studygroups/{id}` | Update study group | Creator only |
| `DELETE` | `/api/studygroups/{id}` | Delete study group | Creator only |
| `POST` | `/api/groupmembers` | Join study group | Self only |
| `GET` | `/api/groupmembers/groups/{id}/members` | View group members | Members only |
| `DELETE` | `/api/groupmembers/{id}/remove` | Remove member | Creator only |
| `DELETE` | `/api/groupmembers/groups/{id}/leave` | Leave group | Self only |

### Tag & Discovery Endpoints
| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/tags/stats` | Tag system statistics |
| `GET` | `/api/tags/users/{id}` | User's skill tags |
| `GET` | `/api/tags/studygroups/{id}` | Group's course tags |

## Usage Examples

### Creating a Study Group with Auto-Tagging
```http
POST /api/studygroups
Authorization: Bearer {your-jwt-token}
Content-Type: application/json

{
  "courseName": "Introduction to Psychology",
  "creatorId": 1,
  "topic": "Midterm Exam Preparation",  
  "timeSlot": "Tuesdays 6-8 PM",
  "description": "Study group for Psychology 101 midterm exam",
  "location": "Library Room 204"
}
```

### Joining a Study Group Instantly
```http
POST /api/groupmembers
Authorization: Bearer {your-jwt-token}
Content-Type: application/json

{
  "groupId": 5,
  "userId": 2,
  "statusId": 2
}
```

### Discovering Groups by Course
```http
GET /api/studygroups/by-course/psychology
```

## Basic Testing

### Manual Testing with Swagger
1. **Start Application**: Run `dotnet run`
2. **Open Swagger**: Navigate to `https://localhost:7001/swagger`
3. **Register User**: Use `/api/auth/register` endpoint
4. **Login**: Use `/api/auth/login` to get JWT token
5. **Authorize**: Click "Authorize" button and paste token
6. **Test Endpoints**: Try different endpoints with authentication

## Architecture

### Clean Architecture Implementation
- **API Layer**: Controllers handle HTTP requests/responses only
- **Service Layer**: Business logic and validation
- **Data Layer**: Database access with Dapper ORM
- **Database**: SQL Server with proper normalization

### Key Design Patterns
- **Dependency Injection**: Constructor injection throughout
- **Repository Pattern**: Data access abstraction
- **DTO Pattern**: Clean data transfer between layers
- **Authentication/Authorization**: JWT with custom authorization attributes

### Advanced Features
- **Auto-Tagging System**: Intelligent tag generation and matching
- **Privacy Protection**: Member-only access to sensitive data
- **Role-Based Authorization**: Creator vs. member permissions
- **Flexible Authentication**: Supports multiple token formats

## Technologies Used

### Backend Framework
- **.NET 8** - Latest LTS version
- **ASP.NET Core Web API** - RESTful API framework
- **C# 12** - Latest language features

### Database & ORM
- **SQL Server** - Relational database
- **Dapper** - Lightweight ORM for performance
- **Parameterized Queries** - SQL injection protection

### Authentication & Security
- **JWT Bearer Tokens** - Stateless authentication
- **SHA256 Hashing** - Password security
- **Role-Based Authorization** - Access control
- **HTTPS Enforcement** - Secure communication

### Documentation & Testing
- **Swagger/OpenAPI** - Interactive API documentation
- **XML Documentation** - Comprehensive code comments
- **NUnit** (for testing) - Unit and integration tests



### Common Issues

**1. Database Connection Issues**
```bash
# Check connection string in appsettings.json
# Verify SQL Server is running
sqlcmd -S (localdb)\mssqllocaldb -E -Q "SELECT @@VERSION"
```

**2. JWT Token Issues**
- Verify JWT configuration in `appsettings.json`
- Ensure token is properly formatted in requests
- Check token expiration (24 hours by default)

**3. Build Errors**
```bash
# Clean and restore
dotnet clean
dotnet restore
dotnet build
```

**4. Port Conflicts**
- Default ports: 7001 (HTTPS), 5000 (HTTP)
- Change in `Properties/launchSettings.json` if needed



1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

---

**Built using ASP.NET Core and modern C# practices**

For more information, visit the [GitHub repository](https://github.com/Goodluckajeh/StudyGroup).