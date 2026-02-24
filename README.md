# Library Management System  
### ASP.NET Core MVC • Entity Framework Core • SQLite • Tailwind CSS  

> A full-stack database-driven Library Management System built using ASP.NET Core MVC with complete CRUD functionality, relational modeling, migrations, and structured seeding.

---

## Author

**Divyesh Hirapara**  
CSCI 6809 – Advanced Applications Development  
Faieleigh Dickinson University  
Vancouver, Canada  

---

## Project Overview

This project is a fully functional **Library Management System** developed using:

- ASP.NET Core MVC
- Entity Framework Core
- SQLite
- Tailwind CSS

The application replaces hard-coded mock data with a real relational database and provides complete **CRUD (Create, Read, Update, Delete)** functionality for:

- Books  
- Authors  
- Customers  
- Library Branches  

The system uses proper database migrations, foreign key relationships, validation, and automatic data seeding.

---

## Project Objectives

- Replace hard-coded data with persistent SQLite database
- Implement Entity Framework Core with migrations
- Design relational models with foreign keys
- Provide full UI-based CRUD operations
- Seed each entity with 20+ meaningful records
- Create a responsive interface using Tailwind CSS
- Maintain clean MVC architecture

---

## Technology Stack

### Backend
- ASP.NET Core MVC
- Entity Framework Core
- SQLite
- Razor Views

### Frontend
- Tailwind CSS (CDN)
- Razor Layout System

---

## System Architecture

The project follows the **MVC (Model–View–Controller)** pattern.

### Folder Structure

```bash
LibraryManagementSystem/
│
├── Controllers/
├── Models/
├── ViewModels/
├── Data/
│   ├── LibraryDbContext.cs
│   └── DbSeeder.cs
├── Views/
│   ├── Books/
│   ├── Authors/
│   ├── Customers/
│   ├── LibraryBranches/
│   └── Shared/
├── Database/
│   └── library.db
├── Program.cs
└── appsettings.json
```

---

## Database Design

### Core Entities

| Entity | Description |
|--------|------------|
| Author | One-to-many relationship with Books |
| Book | Linked to Author and optional Library Branch |
| Customer | Optional Preferred Branch |
| LibraryBranch | One-to-many with Books & Customers |

### Relationships

- One Author → Many Books  
- One Library Branch → Many Books  
- One Library Branch → Many Customers  

### Constraints

- Unique ISBN  
- Unique Customer Email  
- Required foreign keys  
- Restrictive delete behavior  

---

## Features

### Books
- Add, edit, delete books
- Search by title, ISBN, author
- Filter by genre
- ISBN uniqueness validation
- Dropdown for Author & Branch

### Authors
- Full CRUD
- Prevent deletion if books exist

### Customers
- Unique email validation
- Optional preferred branch
- Membership tracking

### Library Branches
- Manage branch details
- Display related book/member counts

---

## Database Migrations & Seeding

The application automatically:

- Applies migrations on startup
- Seeds 20+ records per entity
- Prevents duplicate seeding
- Maintains referential integrity

Database file location:

```
Database/library.db
```

---

### Setup Instructions

### Prerequisites

- .NET SDK installed
- EF Core CLI tools (optional but recommended)

Install EF tool:

```bash
dotnet tool install --global dotnet-ef
```

---

###  Run the Project

```bash
dotnet restore
dotnet ef migrations add InitialCreate   # First time only
dotnet ef database update
dotnet run
```

Then open:

```
https://localhost:xxxx
```

---

##  Verification Checklist

- [ ] Database file created
- [ ] Tables generated
- [ ] 20+ records visible per entity
- [ ] Create/Edit/Delete works
- [ ] Data persists after restart
- [ ] Validation working properly

---

## Data Validation

Implemented using:

- `[Required]`
- `[StringLength]`
- `[EmailAddress]`
- `[Range]`
- Server-side validation
- Client-side validation (jQuery)

---

## UI Design

- Responsive layout
- Clean table designs
- Clear action buttons
- Form validation styling
- Tailwind utility classes

---

## Future Enhancements

### Phase B
- Book loan system (Borrow/Return)
- Due dates & overdue detection
- Fine calculation
- Category normalization

### Phase C
- Authentication & Authorization
- Role-based access control
- Audit logging
- Pagination
- Data export (CSV)

---

## Current Limitations

- No authentication
- No loan tracking (Phase A only)
- No pagination
- Basic search functionality

---

## License

This project is developed for academic purposes.

---

## Project Highlights

- Real database integration
- Proper MVC architecture
- Clean relational modeling
- Production-style migration workflow
- Structured seeding strategy
- Maintainable and scalable design

---

