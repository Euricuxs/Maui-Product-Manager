# Project Instructions

## 1. Project

This is a small .NET MAUI Android application built for learning and technical interview preparation.

The application is a Product Manager that communicates with an ASP.NET Core Web API.

---

## 2. Technology

Use:

- .NET MAUI
- C#
- XAML
- MVVM
- ASP.NET Core Web API
- Entity Framework Core
- SQLite

---

## 3. Architecture

Follow a simple MVVM architecture.

Structure:

MauiProductManager/
├── Models/
├── Views/
├── ViewModels/
├── Services/
├── Resources/
├── App.xaml
├── AppShell.xaml
└── MauiProgram.cs

### Models

Contain application data models.

### Views

Contain XAML pages and minimal code-behind.

Do not put business logic inside Views.

### ViewModels

Contain:

- UI state
- Commands
- User interaction logic
- Service calls

### Services

Contain API communication and external service logic.

---

## 4. MVVM Rules

Use data binding between Views and ViewModels.

Prefer commands over click handlers.

Keep code-behind minimal.

Do not put business logic in XAML code-behind.

Use observable properties where appropriate.

---

## 5. API Communication

The MAUI application communicates with the backend through HTTP.

Use HttpClient through dependency injection.

Do not access the database directly from the MAUI application.

Keep API communication inside services.

Handle:

- Successful responses
- HTTP errors
- Network errors
- Empty responses

---

## 6. Dependency Injection

Register services through MauiProgram.cs.

Prefer constructor injection.

Do not instantiate services manually inside ViewModels.

---

## 7. Navigation

Use .NET MAUI Shell navigation.

Keep navigation logic simple.

Use route names consistently.

---

## 8. Error Handling

The application should provide clear UI states for:

- Loading
- Empty data
- Network errors
- API errors
- Validation errors

Do not silently ignore exceptions.

Avoid exposing technical exception details directly to users.

---

## 9. Validation

Validate user input before sending requests.

Required fields:

- Product name
- Price
- Category

Price must be greater than zero.

---

## 10. Coding Style

Follow standard C# conventions.

Use:

- PascalCase for classes and public members
- camelCase for private fields and local variables
- Meaningful names
- Small focused methods
- Async/await for asynchronous operations

Avoid unnecessary abstractions.

Do not introduce additional frameworks or libraries without a clear reason.

---

## 11. Scope

Keep the project simple.

Do not add features outside PRD.md unless explicitly requested.

Do not introduce:

- CQRS
- MediatR
- Repository pattern without a clear need
- Complex Clean Architecture
- Unnecessary design patterns
- Unnecessary third-party libraries

The primary goal is to demonstrate understanding of .NET MAUI, MVVM, REST API integration, and clean C# code.

---

## 12. Development Workflow

Before implementing a feature:

1. Read PRD.md.
2. Inspect the existing project structure.
3. Reuse existing patterns.
4. Implement the smallest reasonable solution.
5. Build the project.
6. Fix compilation errors.
7. Verify the affected functionality.

Do not rewrite unrelated code.

---

## 13. Testing

Prioritize testing:

- ViewModel behavior
- Validation
- API service behavior
- Error handling

Keep tests simple and focused.

---

## 14. Important Rule

Do not generate code that the developer cannot reasonably explain during a technical interview.

Prefer simple, readable implementations over clever abstractions.