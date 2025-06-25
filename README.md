# HopIn\_Server

**HopIn\_Server** is a backend server for a ride-sharing and messaging platform, built using ASP.NET Core and MongoDB. It implements modern backend architecture principles such as clean separation of concerns, use of DTOs, service layers, and supports real-time communication via SignalR.

---

## Table of Contents

* [Technologies Used](#technologies-used)
* [Project Structure](#project-structure)
* [Architecture Overview](#architecture-overview)

  * [Models](#models)
  * [DTOs (Data Transfer Objects)](#dtos-data-transfer-objects)
  * [Mappers](#mappers)
  * [Services](#services)
  * [Controllers](#controllers)
* [Real-Time Messaging](#real-time-messaging)
* [Configuration](#configuration)
* [How to Run](#how-to-run)
* [API Endpoints](#api-endpoints)
* [Extending the Project](#extending-the-project)
* [Summary](#summary)

---

## Technologies Used

* **ASP.NET Core 8.0** – Web API framework for building RESTful services
* **MongoDB** – NoSQL database for storing users, rides, messages, and more
* **MongoDB.Driver** – Official C# driver for MongoDB
* **SignalR** – Real-time communication for messaging
* **Swagger** – API documentation and testing
* **Dependency Injection** – Built-in support for service lifecycle management

---

## Project Structure

```
├── Configurations/      # Database settings and configuration classes
├── Controllers/         # API controllers (entry points for HTTP requests)
├── Dtos/                # Data Transfer Objects for request/response shaping
├── Hubs/                # SignalR hubs for real-time messaging
├── Mappers/             # Mapper classes for converting DTOs to Models
├── Models/              # Domain models/entities (User, Ride, Vehicle, etc.)
├── Services/            # Business logic and data access services
├── Properties/          # Launch settings
├── Program.cs           # Application entry point and DI setup
├── appsettings.json     # Configuration file (DB connection, etc.)
```

---

## Architecture Overview

### Models

* Located in the `Models/` directory
* Represent core entities like `User`, `Ride`, `UserVehicle`, `Chat`, `Message`, and `Inbox`
* Decorated with MongoDB attributes for seamless serialization/deserialization

### DTOs (Data Transfer Objects)

* Located in the `Dtos/` directory
* Define the data shape used in API requests and responses
* Help separate internal models from external API contracts
* Examples: `CreateUserDto`, `CreateRideDto`, `ChatDto`

### Mappers

* Located in the `Mappers/` directory
* Convert between DTOs and Models using static mapping classes
* Example: `RideMapper` maps `CreateRideDto` to `Ride`

### Services

* Located in the `Services/` directory
* Contain business logic and interact with MongoDB
* One service per major entity: `UserService`, `RideService`, `VehicleService`, etc.
* Encapsulate all data access and manipulation logic

### Controllers

* Located in the `Controllers/` directory
* Handle HTTP requests and route them to the appropriate services
* Perform input validation and return structured API responses
* Examples: `UserController`, `RideController`, `ChatController`

---

## Real-Time Messaging

* Implemented with **SignalR**
* `MessagingHub` handles real-time chat between users
* Messages are broadcast to all connected clients in the relevant chat group
* Used for instant delivery of chat content in ride-related conversations

---

## Configuration

* MongoDB connection settings defined in `appsettings.json` under `MongoDBConfigs`
* Dependency injection is configured in `Program.cs`
* All services and configurations are injected where needed using .NET Core’s built-in DI system

---

## How to Run

1. **Set up MongoDB**
   Ensure you have a running MongoDB instance. Update the connection string in `appsettings.json`.

2. **Restore dependencies**

   ```bash
   dotnet restore
   ```

3. **Build the project**

   ```bash
   dotnet build
   ```

4. **Run the server**

   ```bash
   dotnet run
   ```

---

## API Documentation

* On the **same machine** running the server:
  Visit [http://localhost:5000/swagger](http://localhost:5000/swagger)

* On **other devices on the same network** (phone, tablet, etc.):
  First, find your machine’s IPv4 address (e.g., `192.168.1.10`), then open:

  ```
  http://<your-ipv4-address>:5000/swagger
  ```

  Example:

  ```
  http://192.168.1.10:5000/swagger
  ```

> Make sure your firewall allows incoming traffic on port `5000`, and the server is bound to all network interfaces (use `--urls http://0.0.0.0:5000` if needed in `launchSettings.json` or when running manually).

---

## API Endpoints

Base URL: `  http://<your-ipv4-address>:5000/`

* **User Management**: `/api/User`
* **Ride Management**: `/api/Rides`
* **Vehicle Management**: `/api/Vehicles`
* **Chat Management**: `/api/Chat`
* **Inbox Management**: `/api/Inbox`
* **Messaging**: `/api/Messaging`

Each controller exposes CRUD operations and custom endpoints. Use Swagger UI to explore full documentation and test the endpoints.

---

## Extending the Project

* **Add Features**: Create new Models, DTOs, and Mappers
* **Business Logic**: Implement logic inside Services, not Controllers
* **Validation**: Use DTOs and `ModelState` for input validation
* **SignalR**: Extend hubs to add more real-time capabilities

---

## Summary

HopIn\_Server demonstrates a clean and scalable backend design using ASP.NET Core and MongoDB, with the following layered architecture:

* **Models**: Define domain structure
* **DTOs**: Define API contracts
* **Mappers**: Transform data between layers
* **Services**: Contain core business logic
* **Controllers**: Handle HTTP interactions
* **SignalR**: Enables real-time messaging

