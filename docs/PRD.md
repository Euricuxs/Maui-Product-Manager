# Product Requirements Document

## 1. Project Overview

### Product Name
Maui Product Manager

### Purpose
A simple mobile application built with .NET MAUI to demonstrate CRUD operations, MVVM, REST API integration, navigation, form handling, and basic error handling.

### Target Platform
Android

---

## 2. Goals

The application should demonstrate:

- .NET MAUI fundamentals
- XAML UI development
- MVVM architecture
- Data binding
- Commands
- Navigation
- REST API integration
- CRUD operations
- Loading and error states
- Dependency injection

---

## 3. Tech Stack

### Mobile
- .NET MAUI
- C#
- XAML
- MVVM

### Backend
- ASP.NET Core Web API
- Entity Framework Core
- SQLite

---

## 4. Core Features

### Product List

The user can:

- View all products
- Search products
- Refresh the product list
- Navigate to product details
- Navigate to the create product page

Each product displays:

- Name
- Price
- Category

### Product Detail

The user can:

- View product information
- Navigate to edit
- Delete the product

### Create Product

The user can:

- Enter product name
- Enter price
- Select category
- Save the product

### Edit Product

The user can:

- Modify product information
- Save changes

### Delete Product

The user can:

- Delete a product
- Return to the product list

---

## 5. API

The MAUI application communicates with the backend through HTTP.

Endpoints:

GET /api/products
GET /api/products/{id}
POST /api/products
PUT /api/products/{id}
DELETE /api/products/{id}

The MAUI application must not access the database directly.

---

## 6. UI Requirements

The application should contain:

- Product List Page
- Product Detail Page
- Product Form Page

The UI should support:

- Loading state
- Empty state
- Error state
- Form validation

---

## 7. Architecture

Use MVVM.

Responsibilities:

View:
- Display UI
- Bind to ViewModel

ViewModel:
- Manage UI state
- Handle commands
- Call services

Service:
- Handle API communication

Model:
- Represent application data

---

## 8. Out of Scope

Do not implement:

- User registration
- Payment
- Push notifications
- Offline synchronization
- Advanced authentication
- Complex state management
- Cloud deployment

The project should remain small and focused on .NET MAUI fundamentals.