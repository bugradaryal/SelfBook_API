# .NET 8 API Project

This project is an E-book API application developed using .NET 8.

## Project Features:
- **JWT Token**: Used for user authentication.
- **FluentValidation**: Used for data validation.
- **Identity**: Used for user management and authentication.
- **Refresh Token**: Used to extend the session duration for users.
- **Fluent API**: Used for database modeling.
- **MailKit**: Used for sending emails.
- **Code First**: The Code First approach is used with Entity Framework Core.
- **N-Tier Architecture**: Layered architecture (Data Access, Business Entity(Logic), API) is used.

## Getting Started

The project is developed using .NET 8. Follow these steps to get it running:

### Steps:
1. Clone or download the project:
   ```bash
   git clone https://github.com/yourusername/projectname.git
   ```

2. Install dependencies:
   ```bash
   dotnet restore
   ```

3. Configure database connection settings in the `SelfBook_API/DataAccess
/DBContext.cs`, `.env` or `appsettings.json` file.

4. Update the database (migration process):
   ```bash
   dotnet ef database update
   ```

5. Start the project:
   ```bash
   dotnet run
   ```
