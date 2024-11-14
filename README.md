# Facebook Microservice Backend

This project is a backend service for a Facebook-like application, built using various modern technologies and architectural patterns.

## Project Members
- **3121410470**: [Tran Trung Thien](https://github.com/thientranreal)
- **3121410546**: [Duong Thanh Truong](https://github.com/BT2701)
- **3121410128**: [Pham Van Du](https://github.com/vandu178)
- **3121410149**: [Pham Tan Dat](https://github.com/phamtandat655)
- **3121410542**: [Nguyen Nhat Truong](https://github.com/nhattruong16062003)
- **3121410309**: [Le Trong Luc](https://github.com/luccute)

## Project Overview

### Version
- **Current Version**: 1.0.0

### Services
The project is divided into five main microservices, each responsible for a specific domain within the application:

1. **User Service**: Manages user information, authentication, and authorization, including user login, registration, and password recovery.
2. **Contact Service**: Handles user contacts and friend relationships, including messaging, audio calls, and video calls.
3. **Notification Service**: Manages real-time notifications for user activities such as likes, comments, friend requests, and confirmations.
4. **Request Service**: Handles friend requests and other user interactions.
5. **Post Service**: Manages user posts, including creation, deletion, retrieval, likes, comments, and story creation.

Each service is designed to be independently deployable and scalable, ensuring that the application can handle a large number of users and interactions efficiently.

## Technologies Used

- **ASP.NET Core WebAPI**: The main framework for building the backend services.
- **Microservices**: The application is divided into multiple microservices to ensure scalability and maintainability.
- **Docker**: Each microservice is containerized using Docker for easy deployment and management.
- **API Gateway (Ocelot)**: Ocelot is used as the API Gateway to route requests to the appropriate microservices.
- **Entity Framework**: Used for data access and ORM (Object-Relational Mapping).
- **MySQL**: The database system used for storing application data.
- **Repository Design Pattern**: Implemented to abstract data access logic and promote a clean architecture.
- **WebSocket (SignalR Hub)**: Used for real-time communication between the server and clients.

## Version Control

- **Git/GitHub**: The project is version-controlled using Git, and the repository is hosted on GitHub.

## Development Tools

- **JetBrains Rider**: An IDE used for development.
- **Visual Studio Code**: A lightweight code editor used for development.

## Getting Started

### Prerequisites

- Docker installed on your machine.
- .NET Core SDK installed.
- MySQL server running.

### Running the Application

1. Clone the repository:
    ```sh
    git clone https://github.com/thientranreal/facebook-microservice-be.git
    ```
2. Navigate to the project directory:
    ```sh
    cd facebook-microservice-be
    ```
3. Build and run the Docker containers:
    ```sh
    docker-compose up --build
    ```

### Development

- Open the project in JetBrains Rider or Visual Studio Code.
- Use the provided Docker configuration for running and debugging the microservices.

## Contributing

Contributions are welcome! Please fork the repository and create a pull request with your changes.

## License

This project is licensed under the MIT License.
