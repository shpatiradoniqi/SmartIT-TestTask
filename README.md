# SmartIT-TestTask
API is a simple and efficient ASP.NET Core Web API for managing contracts between facilities and equipment. The project is built with scalability and security in mind, featuring background processing, validation, and API key protection (in the local version).

There are two versions of the project:

    Local Version: Designed for developers to run locally with database migrations and API key security.
    Azure Hosted Version: Deployed on Azure App Services with a live SQL database (API key security is not included in this version).

Features
Facilities and Equipment Management

The API includes endpoints to manage facilities and their associated equipment. Example facilities and equipment are pre-seeded for testing purposes.
Contracts

Users can create contracts between facilities and equipment and retrieve a list of all contracts. The contracts are validated to ensure required fields are provided.
API Key Security

In the local version, one endpoint is secured using a static API key. This ensures that only authorized users can access the protected endpoint.
Background Processing

The project implements a background service using IHostedService to queue and process tasks asynchronously.
Unit Testing

The solution includes a test project that uses an in-memory database for testing. The tests validate scenarios like invalid facility codes, successful contract creation, and background task queuing.
How to Use
Local Version

    Clone the repository and restore dependencies.
    Set up the database using migrations.
    Run the application locally and access it via Swagger UI.
    Use the API key to access the secured endpoint.

Azure Hosted Version

The API is live and accessible via the provided Azure link. Note that API key security is not implemented in this version for simplicity.
Hosted Version

The API is hosted on Azure and can be accessed here:
https://shpatitesttask-ghe7fae6fbh8e0fd.westeurope-01.azurewebsites.net/index.html

Testing

The project includes a test suite with scenarios for:

    Handling invalid data (e.g., invalid facility codes).
    Verifying successful contract creation.
    Ensuring background tasks are queued and processed correctly.

To run the tests, use the dotnet test command.
Available Data for Contract Creation

When creating a contract, you can use the following pre-seeded data:
Production Facilities

    PF001 - Main Manufacturing Plant - 5000
    PF002 - Industrial Robotics Facility - 3000
    PF003 - Electronics Assembly Warehouse - 4000

Process Equipment Types

    EQ001 - CNC Machine - 500
    EQ002 - Industrial Robotic Arm - 300
    EQ003 - Soldering and Testing Station - 200

Additional Notes

    The local version requires a database setup using migrations.
    The hosted version uses an Azure SQL Database and does not require any setup.
    To publish your own version, update the appsettings.json file with uncommenting the connection string.

Feel free to explore, test, and customize the API as needed!
