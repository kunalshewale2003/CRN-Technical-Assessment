# CRN Technical Assessment - RESTful Backend API

## Overview

This project is a RESTful Backend API developed as part of the CRN Technical Assessment.

The solution provides Product CRUD operations with authentication, authorization, validation, pagination, refresh-token based authentication, centralized error handling, logging, and automated testing.

## Technology Stack

- .NET 8
- C#
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server / LocalDB
- JWT Authentication
- Refresh Token Strategy
- FluentValidation
- Swagger / OpenAPI
- xUnit
- Moq
- WebApplicationFactory
- Docker / Docker Compose

## Solution Architecture

The solution follows a layered architecture:

```text
Solution
│
├── API
│   ├── Controllers
│   ├── Middleware
│   └── Program.cs
│
├── Application
│   ├── DTOs
│   ├── Interfaces
│   ├── Services
│   └── Validators
│
├── Domain
│   └── Entities
│
├── Infrastructure
│   ├── Data
│   │   ├── Configurations
│   │   └── Repositories
│   └── Identity
│
└── Tests
    └── API.Tests