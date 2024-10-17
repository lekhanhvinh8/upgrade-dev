# Project Overview

This repository contains multiple services written in .NET 6 and .NET 8. The following services are included:

- `iam-service`
- `monitor-service`
- `order-service-command`
- `order-service-query`
- `serviceinfo-service`

## Table of Contents
- [Project Overview](#project-overview)
- [Services](#services)
  - [IAM Service](#iam-service)
  - [Monitor Service](#monitor-service)
  - [Order Service - Command](#order-service---command)
  - [Order Service - Query](#order-service---query)
  - [Service Info Service](#service-info-service)
- [Common Configuration](#common-configuration)
- [Getting Started](#getting-started)
- [Contributing](#contributing)
- [License](#license)

## Services

### IAM Service
- **Path:** `iam-service/`
- **Description:** The IAM (Identity and Access Management) Service uses [Duende IdentityServer](https://duendesoftware.com/products/identityserver) to manage OAuth2 and OpenID Connect protocols. It includes an admin dashboard for managing clients, users, and roles.
- **Admin Dashboard URL:** [https://localhost:5144/](https://localhost:5144/)
- **.NET Version:** .NET 8
- **Dependencies:** 
  - Duende IdentityServer
  - Entity Framework Core (for storing client and user data)
- **Features:**
  - Authentication and authorization
  - Token management
  - Admin dashboard for identity management
- **Configuration:**
  - Default port: `https://localhost:5144/`

### Monitor Service
- **Path:** `monitor-service/`
- **Description:** The Monitor Service provides a user interface to monitor the health and performance of all other services within the system. It offers real-time data and status updates on the connected services.
- **.NET Version:** .NET 6
- **Dependencies:** 
  - ASP.NET Core for the UI
  - HealthChecks packages for service monitoring
- **Features:**
  - Health checks and status monitoring for connected services
  - UI for real-time performance and status updates
  - Configurable monitoring intervals and alerts
- **Configuration:**
  - Default port: `https://localhost:7240/`

  
### Order Service - Command
- **Path:** `order-service-command/`
- **Description:** Part of the order management system that implements event sourcing for writing data. This service is responsible for handling commands related to creating and updating orders. It connects to MongoDB to store events and produces events for the read service.
- **.NET Version:** .NET 8
- **Dependencies:** 
  - MongoDB Driver for data storage
  - Event sourcing library
- **Features:**
  - Event sourcing for reliable data persistence
  - Integration with MongoDB to store events
  - Event publishing to the read project
- **Configuration:**
  - Connects to a MongoDB instance for event storage
  - Event producer setup for event-driven architecture
  - Default port: `https://localhost:7119/`
  

### Order Service - Query
- **Path:** `order-service-query/`
- **Description:** Also part of the order management system implementing event sourcing, this service is responsible for reading data. It connects to SQL Server to create a read model based on the events produced by the command service.
- **.NET Version:** .NET 8
- **Dependencies:** 
  - SQL Server (for read data storage)
  - Event sourcing library
- **Features:**
  - Consumes events to maintain a read model
  - Integration with SQL Server for data retrieval and querying
- **Configuration:**
  - Connects to SQL Server to store and query read data
  - Consumes events to synchronize the read model
  - Default port: `https://localhost:7120/`
  

### Service Info Service
- **Path:** `serviceinfo-service/`
- **Description:** This service provides an API to consume information from other projects. It acts as a service registry and metadata provider for other services within the system.
- **.NET Version:** .NET 8
- **Dependencies:** 
  - HTTP Client for external API consumption
- **Features:**
  - Service discovery and information retrieval
  - Aggregates metadata from other services
- **Configuration:**
  - Configurable API endpoints to retrieve data from other services

## Common Configuration
- All services share a common configuration file, which is dynamically updated through Vault. Remember to adjust the file path on your local machine to ensure correct access.
- Shared settings, such as logging levels, health checks, and authentication tokens, can be customized as needed.

