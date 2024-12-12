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
   - **Note**: The `MYSQL_CONNECTION_STRING` as well as KnowIT admin username and password will be provided separately upon request. Please contact the project maintainers for access.

Optionally, set environment variables locally (see the setting up environmental variables section below)


Run the application:

Open the project in Visual Studio (or your IDE of choice) and press F5.

Or, use the terminal:

dotnet run

### 3. Docker

Run the application using Docker. You will likely need to sete up environmental variables, see the section on this below: 

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

KNOWIT_ADMIN_USERNAME: Admin username for role initialization.

KNOWIT_ADMIN_PASSWORD: Admin password for role initialization.

AWS_ACCESS_KEY_ID, AWS_SECRET_ACCESS_KEY: Required for AWS integration and used for production deployments.These allow for the AWS IAM profile to interact with secrets manager, you do not need to set these if not using this functionality.


### Setting Environment Variables (Windows - PowerShell)

1. Set Environment Variables for Current Session

This method sets the environment variables for the current terminal session only. These will be cleared when the session ends.

$env:MYSQL_CONNECTION_STRING="your-connection-string"
$env:KNOWIT_ADMIN_USERNAME="admin@example.com"
$env:KNOWIT_ADMIN_PASSWORD="your-secure-password"

2. Set Environment Variables Permanently

To persist the environment variables so they are available across terminal sessions, use the following commands:

[System.Environment]::SetEnvironmentVariable("MYSQL_CONNECTION_STRING", "your-connection-string", "User")
[System.Environment]::SetEnvironmentVariable("KNOWIT_ADMIN_USERNAME", "admin@example.com", "User")
[System.Environment]::SetEnvironmentVariable("KNOWIT_ADMIN_PASSWORD", "your-secure-password", "User")

Note: The "User" scope sets the variable for the current user only. To set it system-wide, replace "User" with "Machine". However, setting system-wide variables requires administrative privileges.

3. Verify Environment Variables

To verify that the variables have been set, use the following commands:

echo $env:MYSQL_CONNECTION_STRING
echo $env:KNOWIT_ADMIN_USERNAME
echo $env:KNOWIT_ADMIN_PASSWORD

For permanently set variables, use:

[System.Environment]::GetEnvironmentVariable("MYSQL_CONNECTION_STRING", "User")
[System.Environment]::GetEnvironmentVariable("KNOWIT_ADMIN_USERNAME", "User")
[System.Environment]::GetEnvironmentVariable("KNOWIT_ADMIN_PASSWORD", "User")

This ensures that the application will use the correct database connection string and admin credentials during startup.

### Setting Up Environment Variables (Linux/macOS)

For Linux and macOS users, you can set the required environment variables by using the export command in your terminal. You can also make the variables persist across sessions by adding them to your shell configuration file (e.g., .bashrc, .zshrc, or .bash_profile).

One-Time Setup (For Current Session Only)
To set the environment variables for the current terminal session:


export MYSQL_CONNECTION_STRING="your-connection-string"
export KNOWIT_ADMIN_USERNAME="admin@example.com"
export KNOWIT_ADMIN_PASSWORD="securepassword"
export AWS_ACCESS_KEY_ID="your-access-key-id"
export AWS_SECRET_ACCESS_KEY="your-secret-access-key"
Verify that the variables are set:


echo $MYSQL_CONNECTION_STRING
echo $KNOWIT_ADMIN_USERNAME
Persistent Setup (Across Sessions)
To make the environment variables persist across terminal sessions:

Open your shell configuration file in an editor (e.g., .bashrc, .zshrc, or .bash_profile):


nano ~/.bashrc
# or
nano ~/.zshrc
Add the following lines to the file:


export MYSQL_CONNECTION_STRING="your-connection-string"
export KNOWIT_ADMIN_USERNAME="admin@example.com"
export KNOWIT_ADMIN_PASSWORD="securepassword"
export AWS_ACCESS_KEY_ID="your-access-key-id"
export AWS_SECRET_ACCESS_KEY="your-secret-access-key"
Save and close the file.

Reload the shell configuration to apply the changes:


source ~/.bashrc
# or
source ~/.zshrc
Verify that the variables are set:


echo $MYSQL_CONNECTION_STRING
echo $KNOWIT_ADMIN_USERNAME

With these steps, your environment variables will be available every time you open a new terminal session.

### Running the Docker Container with Environment Variables

When running the Docker container for the KnowIT application, you can pass environment variables using the -e option with the docker run command. Here's how you can do it:

Steps:

Build the Docker Image
Ensure you have built the Docker image for the application.
Run the following command in the root directory of the project:


docker build -t knowit .
Run the Docker Container with Environment Variables
Use the -e flag to set each environment variable when starting the container. Ignore the AWS varaibles if not setting up with AWS RDS:


docker run -d -p 8080:80 \
-e MYSQL_CONNECTION_STRING="your-connection-string" \
-e KNOWIT_ADMIN_USERNAME="admin@example.com" \
-e KNOWIT_ADMIN_PASSWORD="securepassword" \
-e AWS_ACCESS_KEY_ID="your-access-key-id" \
-e AWS_SECRET_ACCESS_KEY="your-secret-access-key" \
knowit

This command will:

Map port 8080 on your local machine to port 80 inside the container.
Pass the required environment variables to the container.
Run the application in the background.
Verify the Application
Once the container is running, you can access the application in your browser at:
http://localhost:8080

Optional: Use an .env File for Simplicity
To avoid typing the environment variables every time, you can use an .env file to store them.

Create an .env file in the root of your project. Again, ignore the AWS varaibles if not setting up with AWS RDS:

MYSQL_CONNECTION_STRING="your-connection-string"
KNOWIT_ADMIN_USERNAME="admin@example.com"
KNOWIT_ADMIN_PASSWORD="securepassword"
AWS_ACCESS_KEY_ID="your-access-key-id"
AWS_SECRET_ACCESS_KEY="your-secret-access-key"

Run the Docker container with the --env-file option:

docker run -d -p 8080:80 --env-file .env knowit
Stop the Container
If you need to stop the running container, use:

docker ps  # Get the container ID
docker stop <container_id>


Check Logs
If the application doesn’t start as expected, you can check the logs for debugging:

docker logs <container_id>