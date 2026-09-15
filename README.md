# Task Management API

A RESTful backend API built with ASP.NET Core for managing projects, team members, tasks, and comments. The project focuses on authentication, role-based authorization, project-level access control, and common backend features used in real-world applications.

## Features

* JWT-based user authentication
* Role-based authorization with Admin, Project Manager, and Member roles
* Project creation and management
* Add and remove members from projects
* Task creation, assignment, updating, and deletion
* Task status and priority management
* Comments for tasks
* Project-level access control to ensure users can only access permitted resources
* Task filtering, searching, sorting, and pagination
* DTOs and service-layer architecture
* Dependency Injection
* Global exception handling using ProblemDetails
* Unit testing with xUnit
* GitHub Actions for automated build and test validation

## Tech Stack

* C#
* .NET 10
* ASP.NET Core Web API
* Entity Framework Core
* SQL Server
* JWT Authentication
* xUnit
* Swagger / OpenAPI
* GitHub Actions

## Project Overview

The API is designed around projects and their members. Users can participate in multiple projects, with membership managed through a separate `ProjectMember` entity.

Tasks belong to projects and can be assigned to project members. Access to project and task operations is controlled using both user roles and project membership, providing more granular authorization than role checks alone.

The API also includes pagination, filtering, searching, and sorting for task queries to make the application more practical for handling larger datasets.

## Backend Concepts Practiced

* ASP.NET Core Web API
* Entity Framework Core & SQL Server
* Entity Relationships
* LINQ & IQueryable
* DTOs
* Dependency Injection
* JWT Authentication
* Role-Based & Resource-Based Authorization
* Pagination, Filtering & Sorting
* Global Exception Handling
* Unit Testing
* CI with GitHub Actions
