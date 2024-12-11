# KnowIT Project

KnowIT is a knowledge management application designed to manage and organize articles within specific categories. This project provides functionality for users to browse articles by category, create new categories and articles (for admins), and edit or delete existing content.

---

## Features

- **Category Management**: Admin users can create, edit, delete, and manage categories.
- **Article Management**: Admin users can create, edit, delete, and view articles, with each article associated with a category.
- **Role-Based Access Control**: Admin users can access and manage content, while general users can only view articles.
- **Category Filtering**: Users can view articles based on the selected category, with an option to view articles without a category.

---

## Project Structure

### Controllers

#### 1. `CategoryController`
Manages the creation, editing, deletion, and viewing of categories. Key functionalities include:

- **Index**: Displays a list of all categories and articles, allowing users to filter articles by category.
- **ShowArticles**: Displays articles for a specific category or those without a category.
- **Manage**: Allows admin users to manage and view all categories.
- **Create**: Admin-only functionality to create new categories.
- **Edit**: Admin-only functionality to edit existing categories.
- **Delete**: Admin-only functionality to delete a category while ensuring associated articles are not deleted.
- **Cancel**: Cancels ongoing category creation or editing and redirects to the categories index.

#### 2. `HomeController`
Manages the homepage of the application and user authentication. Key functionalities include:

- **Index**: Displays a list of featured categories on the homepage.
- **Login**: Manages the login process for users.
- **Logout**: Allows users to log out and redirects them to the homepage.

#### 3. `KBController`
Handles the creation, editing, viewing, and deletion of articles. Key functionalities include:

- **Index**: Displays a list of articles, optionally filtered by category.
- **Create**: Admin-only functionality to create new articles and assign them to categories.
- **Edit**: Admin-only functionality to edit existing articles, including category changes.
- **Delete**: Admin-only functionality to delete articles.
- **Details**: Displays detailed information about a specific article.

---

## `Program.cs` Functionality Breakdown

The `Program.cs` file in KnowIT contains key configurations for the application, including:

1. **Service Registration**:
   - Sets up controllers, Razor Pages, Identity for user management, and logging.

2. **Database Connection**:
   - The application dynamically determines which database credentials to use:
   - If the `MYSQL_CONNECTION_STRING` environment variable is set, it is used directly for connecting to the database. This is typically used for local development or testing.
	- If `MYSQL_CONNECTION_STRING` is not set, the application fetches the connection string from AWS Secrets Manager. This is typically used for production deployments to securely manage credentials.
	- This flexible configuration ensures seamless deployment across different environments.

3. **AWS Secrets Manager Integration**:
   - Provides secure management of sensitive database credentials. (used for production deployment)

4. **Admin User & Role Initialization**:
   - Automatically creates an "Admin" role and an admin user during startup, using credentials stored in AWS Secrets Manager.

5. **Middleware Configuration**:
   - Configures middleware for error handling, static files, authentication, and routing.

---

## Getting Started

### 1. Clone the Repository

Clone the repository to your local machine:


git clone https://github.com/Rdanilowicz/KnowIT.git

### 2. Run Locally

To run the application locally:

1. Ensure you have the required dependencies and .NET SDK installed.

Dependencies:
.NET Core 6 or higher
Docker (optional, for containerization)
Entity Framework Core
Microsoft Identity for authentication and authorization

2. Set up your database connection:

   - If the `MYSQL_CONNECTION_STRING` environment variable is set, the application will use it for local testing or development.
   - If not, the application will fetch the connection string from AWS Secrets Manager for production deployments.
   - **Note**: The `MYSQL_CONNECTION_STRING` will be provided separately upon request. Please contact the project maintainers for access.

Optionally, set environment variables locally:
   
   $env:MYSQL_CONNECTION_STRING="your-connection-string"
   $env:AWS_ACCESS_KEY_ID="your-access-key-id"
   $env:AWS_SECRET_ACCESS_KEY="your-secret-access-key"


Run the application:

Open the project in Visual Studio and press F5.

Or, use the terminal:

dotnet run

3. Docker

Run the application using Docker:

Build the Docker image:


docker build -t knowit .
Run the Docker container:


docker run -d -p 8080:80 knowit
Access the application at http://localhost:8080.

---

### Environment Variables
The application uses the following environment variables:

ASPNETCORE_ENVIRONMENT: Set to Development or Production.

MYSQL_CONNECTION_STRING: Used for local development and testing to specify the database connection string.

AWS_ACCESS_KEY_ID, AWS_SECRET_ACCESS_KEY: Required for AWS integration and used for production deployments.