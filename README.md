# Business Card Management System

## Project Overview
A web application to manage business card information, allowing users to import, export, filter, and organize their contacts.

## Setup Instructions
### Prerequisites
- Node.js and npm installed
- .NET Core SDK installed
- SQL Server installed

### Frontend Setup (Angular)
1. Navigate to the `/frontend` folder.
2. Run `npm install` to install dependencies.
3. Run `ng serve` to start the Angular app.

### Backend Setup (C#/.NET)
1. Navigate to the `/backend` folder.
2. Run `dotnet restore` to install dependencies.
3. Update the database connection string in `appsettings.json`.
4. Run `dotnet run` to start the backend API.

## Database Setup
1. Restore the provided SQL/Oracle DB dump file.
2. Update the connection string in the backend app.
3. Run migrations using `dotnet ef database update` if applicable.

## Usage
- Visit `http://localhost:4200` to access the application.
- Use the app to add, view, or manage business cards.
