# Movie Stream
## Overview
Movie Stream is a web application built using ASP.NET Core. It integrates with AWS services for authentication and database management. The application includes features such as user login, movie management, and more.

## Getting Started

### Prerequisites
- .NET 6.0 SDK or later
- AWS account with appropriate credentials

### Installation
1. Clone the repository:
    ```sh
    git clone https://github.com/yourusername/Lab3_NhatNguyen_WebApp.git
    cd Lab3_NhatNguyen_WebApp
    ```

2. Restore the dependencies:
    ```sh
    dotnet restore
    ```

3. Update the [`appsettings.json`]
### Running the Application
1. Build the project:
    ```sh
    dotnet build
    ```

2. Run the project:
    ```sh
    dotnet run
    ```

3. Open your browser and navigate to [`https://localhost:5001`]

## Configuration
The application uses [`appsettings.json`]
```json
{
  "AmazonCredential": {
    "AccessKey": "your-access-key",
    "SecretKey": "your-secret-key"
  },
  "ConnectionStrings": {
    "Connection2RDS": "your-rds-connection-string"
  },
  "DBUsername": "your-db-username",
  "DBPassword": "your-db-password"
}