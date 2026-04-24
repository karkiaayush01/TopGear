# TopGear 🔧

A **Vehicle Parts Selling & Inventory Management System** built with ASP.NET Core Web API. TopGear streamlines operations for vehicle service and parts retail centers — handling inventory, sales, customer management, vendor purchasing, and AI-powered part failure predictions.

---

## Features

### 👤 Admin
- Register and manage staff accounts
- Manage vendor details and purchase invoices
- Full inventory control (CRUD for vehicle parts)
- Maintain financial records and accounts payable
- Business performance reports (financial & inventory)
- Review and action customer part requests

### 🧑‍💼 Staff
- Register and manage customers and their vehicle details
- Process part sales and generate emailed invoices
- Manage credit sales and track overdue balances
- Search customers by name, phone, ID, or vehicle number
- Access full customer purchase and vehicle history
- Generate reports: top spenders, regulars, overdue credit

### 🙋 Customer
- Self-register and manage profile and vehicle details
- Book service appointments
- Submit reviews for services and parts
- Request unavailable or out-of-stock parts
- AI-powered part failure predictions and product recommendations
- Automatic 10% loyalty discount on purchases over Rs. 5,000

### ⚙️ Automation
- Email alert to admin when any part stock drops to 10 units or below
- Email reminder to customers with credit balances overdue by more than one month
- Notification log for full audit trail of all sent emails

---

## Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core Web API (.NET 8) |
| Language | C# |
| Database | PostgreSQL |
| ORM | Entity Framework Core |
| Auth | ASP.NET Core Identity + JWT |
| Email | SMTP via IEmailService abstraction |
| AI | LLM API integration (part failure prediction) |

---

## Project Structure

```
TopGear/                        # ASP.NET Web API — controllers, middleware, program entry
TopGear.Application/            # Application layer — services, DTOs, interfaces
TopGear.Domain/                 # Domain layer — entities, enums, domain logic
TopGear.Infrastructure/         # Infrastructure layer — EF Core, Identity, email, migrations
.github/                        # GitHub Actions workflows
.gitignore
```

---

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL](https://www.postgresql.org/download/)

### 1. Clone the repository

```bash
git clone https://github.com/your-org/topgear.git
cd topgear
```

### 2. Create `appsettings.Development.json`

Inside the `TopGear/` project folder, create `appsettings.Development.json` and add your database connection string and JWT settings:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=topgear;Username=your_user;Password=your_password"
  },
  "JwtSettings": {
    "Key": "your-secret-key-here",
    "Issuer": "TopGear",
    "Audience": "TopGearUsers",
    "ExpiryMinutes": 60
  },
  "EmailSettings": {
    "Host": "smtp.your-provider.com",
    "Port": 587,
    "Username": "your@email.com",
    "Password": "your-email-password"
  }
}
```

> ⚠️ `appsettings.Development.json` is listed in `.gitignore` — never commit secrets to the repository.

### 3. Apply database migrations

```bash
cd TopGear
dotnet ef database update
```

If migrations don't exist yet or you've made model changes, add a new migration first:

```bash
dotnet ef migrations add <MigrationName> --project ../TopGear.Infrastructure --startup-project .
dotnet ef database update
```

### 4. Run the API

```bash
dotnet run
```

The API will be available at `https://localhost:5001` by default.

---

## API Overview

| Area | Base Route |
|---|---|
| Auth | `/api/auth` |

---

## License

This project is for academic purposes.