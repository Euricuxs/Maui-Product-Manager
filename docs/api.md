# MauiProductManager API

## Overview

The MauiProductManager API is a RESTful web service that provides CRUD (Create, Read, Update, Delete) operations for managing a product catalog. It is the backend for the MauiProductManager mobile application.

## Base URL

```
http://localhost:5000
```

When running locally via `dotnet run`, the API is available at `http://localhost:5000` by default.

## Authentication

No authentication is required. The API is open for development and testing purposes.

## API Version

- **Current Version**: v1
- **OpenAPI Document**: `/swagger/v1/swagger.json`

---

## Product Endpoints

### GET /api/products

Retrieves all products.

**Response**

| Status | Description |
|--------|-------------|
| 200 OK | Returns a collection of all products. |

**Response Body**

```json
[
  {
    "id": 1,
    "name": "Mechanical Keyboard",
    "price": 89.99,
    "category": "Electronics"
  },
  {
    "id": 2,
    "name": "Desk Chair",
    "price": 199.99,
    "category": "Furniture"
  }
]
```

---

### GET /api/products/{id}

Retrieves a specific product by ID.

**Parameters**

| Parameter | Type | Location | Description |
|-----------|------|----------|-------------|
| id | integer | path | The unique ID of the product. |

**Responses**

| Status | Description |
|--------|-------------|
| 200 OK | Returns the product. |
| 404 Not Found | No product with the given ID exists. |

**200 OK Response Body**

```json
{
  "id": 1,
  "name": "Mechanical Keyboard",
  "price": 89.99,
  "category": "Electronics"
}
```

**404 Not Found Response Body**

```json
{
  "message": "Product with id 999 not found."
}
```

---

### POST /api/products

Creates a new product.

**Request Body**

```json
{
  "name": "Mechanical Keyboard",
  "price": 89.99,
  "category": "Electronics"
}
```

**Responses**

| Status | Description |
|--------|-------------|
| 201 Created | Product was created successfully. |
| 400 Bad Request | Validation failed. Check the response body for details. |

**201 Created Response Body**

```json
{
  "id": 3,
  "name": "Mechanical Keyboard",
  "price": 89.99,
  "category": "Electronics"
}
```

**400 Bad Request Response Body**

```json
{
  "Name": ["Name is required."],
  "Price": ["Price must be greater than zero and 1,000,000 or less."]
}
```

**Validation Rules**

| Field | Rules |
|-------|-------|
| name | Required. Maximum 100 characters. |
| category | Required. Maximum 50 characters. |
| price | Required. Must be greater than 0 and at most 1,000,000. |

---

### PUT /api/products/{id}

Updates an existing product.

**Parameters**

| Parameter | Type | Location | Description |
|-----------|------|----------|-------------|
| id | integer | path | The unique ID of the product to update. |

**Request Body**

```json
{
  "name": "Mechanical Keyboard",
  "price": 99.99,
  "category": "Electronics"
}
```

**Responses**

| Status | Description |
|--------|-------------|
| 200 OK | Product was updated successfully. |
| 400 Bad Request | Validation failed. Check the response body for details. |
| 404 Not Found | No product with the given ID exists. |

**200 OK Response Body**

```json
{
  "id": 1,
  "name": "Mechanical Keyboard",
  "price": 99.99,
  "category": "Electronics"
}
```

---

### DELETE /api/products/{id}

Deletes a product.

**Parameters**

| Parameter | Type | Location | Description |
|-----------|------|----------|-------------|
| id | integer | path | The unique ID of the product to delete. |

**Responses**

| Status | Description |
|--------|-------------|
| 200 OK | Product was deleted successfully. |
| 404 Not Found | No product with the given ID exists. |

**200 OK Response Body**

```json
{
  "message": "Product deleted successfully."
}
```

---

## Data Models

### Product

Represents a product in the catalog.

| Field | Type | Description |
|-------|------|-------------|
| id | integer | Unique identifier (auto-generated). |
| name | string | Name of the product. Required, max 100 chars. |
| price | decimal | Product price. Must be greater than 0 and at most 1,000,000. |
| category | string | Product category. Required, max 50 chars. |

### ProductDto

Data transfer object used for creating and updating products.

| Field | Type | Required | Constraints |
|-------|------|----------|-------------|
| name | string | Yes | Max 100 characters |
| category | string | Yes | Max 50 characters |
| price | decimal | Yes | Greater than 0, max 1,000,000 |

---

## HTTP Status Codes

| Code | Description |
|------|-------------|
| 200 OK | The request succeeded. |
| 201 Created | A new resource was created. |
| 400 Bad Request | The request was invalid or failed validation. |
| 404 Not Found | The requested resource was not found. |

---

## How to Open Swagger UI

1. Start the API:
   ```bash
   cd MauiProductManager.Api
   dotnet run
   ```
2. Open your browser and navigate to:
   ```
   http://localhost:5000/swagger
   ```
3. The Swagger UI provides an interactive interface to explore and test all endpoints.

---

## How to Test the API Locally

### Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) or later
- A tool like [curl](https://curl.se/), [Postman](https://www.postman.com/), or HTTPie

### Steps

1. **Start the API**:
   ```bash
   cd MauiProductManager.Api
   dotnet run
   ```
   The API starts on `http://localhost:5000`.

2. **List all products**:
   ```bash
   curl http://localhost:5000/api/products
   ```

3. **Get a product by ID**:
   ```bash
   curl http://localhost:5000/api/products/1
   ```

4. **Create a product**:
   ```bash
   curl -X POST http://localhost:5000/api/products \
     -H "Content-Type: application/json" \
     -d '{"name":"Laptop","price":999.99,"category":"Electronics"}'
   ```

5. **Update a product**:
   ```bash
   curl -X PUT http://localhost:5000/api/products/1 \
     -H "Content-Type: application/json" \
     -d '{"name":"Laptop","price":1099.99,"category":"Electronics"}'
   ```

6. **Delete a product**:
   ```bash
   curl -X DELETE http://localhost:5000/api/products/1
   ```

7. **Test a 404 response**:
   ```bash
   curl http://localhost:5000/api/products/9999
   ```

8. **Test validation errors**:
   ```bash
   curl -X POST http://localhost:5000/api/products \
     -H "Content-Type: application/json" \
     -d '{"name":"","price":-10,"category":""}'
   ```

---

## Database

The API uses SQLite for data storage. The database file (`products.db`) is created automatically in the application directory on first run. Database migrations are applied automatically on startup.
