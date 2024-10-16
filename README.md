# Business Card Management System

## Project Overview

This API provides backend services for managing business cards, developed in C# with .NET Core. The API supports creating, viewing, deleting, importing, exporting, and filtering business cards.

Features
Entity Definition: BusinessCard Model
The BusinessCard entity includes the following properties:

Name: The name of the individual.
Date of Birth (DOB): The individual's birthdate.
Gender: Gender of the individual (male, female, etc.).
Email: Email address.
Phone: Contact number.
Address: Optional field for the business card address.
Photo: Optional field to store a photo in Base64 format.
CreatedAt: Timestamp when the business card was created.
API Endpoints
1. Create New Business Card
Method: POST /api/businesscards
Description: Adds a new business card to the system.
Input: Accepts business card data from the request body (JSON or form data) or via file imports (CSV, XML, optional QR code).
Validation: Ensures required fields like name, email, and phone are valid.
Response: Success message with the business card ID.
2. View Business Cards
Method: GET /api/businesscards
Description: Retrieves a list of all business cards stored in the system.
Response: List of all business cards with details such as name, email, phone, etc.
3. View Business Card by ID
Method: GET /api/businesscards/{id}
Description: Retrieves the details of a specific business card using its ID.
Response: Business card data for the specified ID.
4. Delete Business Card
Method: DELETE /api/businesscards/{id}
Description: Deletes a business card by its unique ID.
Response: Success message if the business card is deleted.
5. Export Business Cards
Method (CSV): GET /api/businesscards/export/csv
Method (XML): GET /api/businesscards/export/xml
Description: Exports all business cards in CSV or XML format.
6. Import Business Cards
Method (CSV): POST /api/businesscards/import/csv
Method (XML): POST /api/businesscards/import/xml
Description: Imports business card data from a CSV or XML file into the system.
7. Filter Business Cards
Method: GET /api/businesscards
Description: Retrieves a list of business cards filtered by query parameters.
Query Parameters:
name
dateOfBirth
phone
gender
email
Response: Filtered list of business cards based on the provided criteria.
Setup Instructions
Prerequisites
.NET Core SDK (version 7.0 or later)
SQL Server or any compatible database engine

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
1. Navigate to the `/BusinessCardWebAPI` folder.
2. Run `dotnet restore` to install dependencies.
3. Update the database connection string in `appsettings.json`.
4. Run `dotnet run` to start the backend API.

## Database Setup        
## SQL Server (.bak File)
1. Download the database dump file: [database_dump.bak](C:\Users\LENOVO\Documents\scriptDatabaseBusinessCardScript.sql)
2. Open SQL Server Management Studio (SSMS).
3. Right-click on **Databases** in the Object Explorer and choose **Restore Database**.
4. Select **Device**, click on **Add**, and choose the `.bak` file.
5. Click **OK** to restore the database.

## Usage
- Visit `http://localhost:4200` to access the application.
- Use the app to add, view, or manage business cards.



