# Invoice Management System

A complete invoice management application built with ASP.NET Core, PostgreSQL, and a modern web UI.

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
    "DefaultConnection": "Host=localhost;Port=5432;Database=invoice_db;Username=xxx;Password=xxx;Pooling=true;Minimum Pool Size=10;Maximum Pool Size=200;Timeout=15;Command Timeout=15;Keepalive=30;Connection Idle Lifetime=30;Connection Pruning Interval=5"
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
dotnet run InvoiceApi.csproj
```
if issue, then use
```base
export ASPNETCORE_ENVIRONMENT=Development && dotnet run InvoiceApi.csproj
```
## API Documentation

### Swagger UI

Once the application is running, access the Swagger documentation at:

```
http://localhost:5000/swagger/index.html
```


## Web UI

Access the web interface at:

```
http://localhost:5000/index.html
```
