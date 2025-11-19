# Invoice Management System

A complete invoice management application built with ASP.NET Core, PostgreSQL, and a modern web UI.

## Features

- ✅ Create, read, and delete invoices
- ✅ Add items to invoices with automatic total calculation
- ✅ PostgreSQL database integration
- ✅ RESTful API with Swagger documentation
- ✅ Responsive web UI
- ✅ Real-time invoice management

## Prerequisites

- .NET 6.0 or higher
- PostgreSQL 12 or higher
- Node.js (optional, for development)

## Database Setup

### 1. Create PostgreSQL User and Database

```sql
-- Connect to PostgreSQL as superuser
psql -U postgres

-- Create user
CREATE USER cryptoversex WITH PASSWORD 'XVerseCrypto@GT&CG';

-- Create database
CREATE DATABASE invoice_db OWNER cryptoversex;

-- Grant privileges
GRANT ALL PRIVILEGES ON DATABASE invoice_db TO cryptoversex;
```

### 2. Initialize Database Schema

```bash
# Run the SQL initialization script
psql -U cryptoversex -d invoice_db -f init.sql
```

## Installation & Setup

### 1. Clone the Repository

```bash
git clone https://github.com/LinUxTo5re/DisplayAnInvoice.git
cd DisplayAnInvoice
```

### 2. Restore NuGet Packages

```bash
dotnet restore
```

### 3. Update Database Connection

Edit `appsettings.json` and verify the connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=invoice_db;Username=cryptoversex;Password=XVerseCrypto@GT&CG;Pooling=true;Minimum Pool Size=10;Maximum Pool Size=200;Timeout=15;Command Timeout=15;Keepalive=30;Connection Idle Lifetime=30;Connection Pruning Interval=5"
  }
}
```

### 4. Run Database Migrations

```bash
dotnet ef database update
```

### 5. Build the Project

```bash
dotnet build
```

### 6. Run the Application

```bash
dotnet run
```

The application will start on `https://localhost:5001` and `http://localhost:5000`

## API Documentation

### Swagger UI

Once the application is running, access the Swagger documentation at:

```
http://localhost:5000/swagger
```

### API Endpoints

#### Get All Invoices
```
GET /api/invoice
```

#### Get Invoice by ID
```
GET /api/invoice/{id}
```

#### Create Invoice
```
POST /api/invoice
Content-Type: application/json

{
  "customerName": "John Doe"
}
```

#### Add Item to Invoice
```
POST /api/invoice/{invoiceId}/items
Content-Type: application/json

{
  "name": "Widget A",
  "price": 19.99,
  "quantity": 2
}
```

#### Delete Invoice
```
DELETE /api/invoice/{id}
```

## Web UI

Access the web interface at:

```
http://localhost:5000
```

### Features

- View all invoices in a card-based layout
- Create new invoices
- Add items to existing invoices
- Delete invoices
- Real-time total calculation
- Responsive design for mobile and desktop

## Project Structure

```
.
├── Controllers/
│   └── InvoiceController.cs       # API endpoints
├── Models/
│   ├── Invoice.cs                 # Invoice model
│   └── InvoiceItem.cs             # Invoice item model
├── Data/
│   └── InvoiceDbContext.cs        # Entity Framework context
├── wwwroot/
│   ├── index.html                 # Web UI
│   ├── styles.css                 # Styling
│   └── script.js                  # Client-side logic
├── Program.cs                     # Application startup
├── appsettings.json               # Configuration
├── InvoiceApi.csproj              # Project file
└── init.sql                       # Database initialization
```

## Technologies Used

- **Backend**: ASP.NET Core 6.0
- **Database**: PostgreSQL
- **ORM**: Entity Framework Core
- **API Documentation**: Swagger/OpenAPI
- **Frontend**: HTML5, CSS3, JavaScript (Vanilla)

## Error Handling

The application includes comprehensive error handling:

- Null reference checks
- Validation of input data
- Proper HTTP status codes
- User-friendly error messages
- Database transaction management

## Development

### Running in Development Mode

```bash
dotnet run --configuration Development
```

### Building for Production

```bash
dotnet publish -c Release -o ./publish
```

## Troubleshooting

### Database Connection Issues

1. Verify PostgreSQL is running
2. Check connection string in `appsettings.json`
3. Ensure user `cryptoversex` exists and has proper permissions
4. Test connection: `psql -U cryptoversex -d invoice_db -h localhost`

### Port Already in Use

If port 5000/5001 is already in use, modify in `Program.cs`:

```csharp
app.Run("http://localhost:5002");
```

### Migration Issues

```bash
# Remove last migration
dotnet ef migrations remove

# Create new migration
dotnet ef migrations add InitialCreate

# Update database
dotnet ef database update
```

## License

This project is provided as-is for educational and commercial use.

## Support

For issues or questions, please create an issue in the GitHub repository.

## Author

Invoice Management System - 2024
