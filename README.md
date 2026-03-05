<p align="center">
  <img src="https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" />
  <img src="https://img.shields.io/badge/C%23-12-239120?style=for-the-badge&logo=csharp&logoColor=white" />
  <img src="https://img.shields.io/badge/SQLite-003B57?style=for-the-badge&logo=sqlite&logoColor=white" />
  <img src="https://img.shields.io/badge/Cloudinary-3448C5?style=for-the-badge&logo=cloudinary&logoColor=white" />
  <img src="https://img.shields.io/badge/JWT-000000?style=for-the-badge&logo=jsonwebtokens&logoColor=white" />
  <img src="https://img.shields.io/badge/Google_OAuth-4285F4?style=for-the-badge&logo=google&logoColor=white" />
</p>

# ◈ LuminaTech

> A full-stack **e-commerce web application** built with ASP.NET Core 8, featuring Google OAuth, JWT authentication, role-based access control, Cloudinary media management, shopping cart, wishlist, and a dummy payment system.

---

## ✨ Features

### 🔐 Authentication & Security
- **Google OAuth 2.0** — Sign in with Google for real users
- **JWT Token Auth** — Stateless authentication with HTTP-only cookies
- **Demo Accounts** — Fully sandboxed accounts for testing (can't modify real data)
- **Super Admin** — Dedicated admin account with full control
- **Role-based Access** — User / Admin roles with middleware enforcement

### 🛍️ E-Commerce
- **Product Catalog** — Browse, search, and filter products by category
- **Product Detail** — Full product pages with specs and related products
- **Shopping Cart** — Add, remove, update quantities, order summary with tax
- **Wishlist** — Save products, move to cart with one click
- **Checkout & Payment** — Dummy payment system with order confirmation
- **Order History** — View past orders with full details

### 👨‍💼 Admin Panel
- **User Management** — View all users, promote/demote roles, activate/deactivate
- **Product CRUD** — Create, update, delete products with Cloudinary image upload
- **Dashboard** — Admin overview with user and product stats

### 🎨 UI/UX
- **Dark Luxury Theme** — Premium dark design with glassmorphism effects
- **Scroll-driven Hero Animation** — Elegant text transitions driven by scroll position
- **Responsive Design** — Works on desktop, tablet, and mobile
- **GSAP Animations** — Smooth entrance animations and micro-interactions
- **Google Fonts** — Syne (display) + Inter (body) typography

---

## 🏗️ Architecture

```
LuminaTech/
├── LuminaTech.API/           ← Web API + Static Frontend
│   ├── Controllers/          ← 9 API controllers
│   ├── wwwroot/              ← HTML, CSS, JS (no framework)
│   ├── Program.cs            ← App config & middleware pipeline
│   └── appsettings.json      ← Configuration
├── LuminaTech.Data/          ← Data Layer
│   ├── Entities/             ← 6 database models
│   └── LuminaTechDbContext.cs ← EF Core DbContext + seed data
├── LuminaTech.Services/      ← Business Logic
│   ├── AuthService.cs        ← JWT token generation
│   └── CloudinaryService.cs  ← Image upload/delete
└── LuminaTech.Tests/         ← Unit Tests
```

### Tech Stack

| Layer | Technology |
|---|---|
| **Backend** | ASP.NET Core 8, C# 12 |
| **Database** | SQLite + Entity Framework Core |
| **Auth** | JWT + Google OAuth 2.0 |
| **Media** | Cloudinary (CDN-delivered images & video) |
| **Frontend** | Vanilla HTML/CSS/JS, GSAP animations |
| **Logging** | Serilog (structured logging) |
| **API Docs** | Swagger / OpenAPI |

---

## 🚀 Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Git](https://git-scm.com/)

### Installation

```bash
# Clone the repository
git clone https://github.com/Dhruv2183/LuminaTechDotNet.git
cd LuminaTechDotNet

# Restore packages
dotnet restore

# Build the solution
dotnet build
```

### Configuration

Create/update `LuminaTech.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=luminatech.db"
  },
  "Jwt": {
    "Key": "your-256-bit-secret-key-here-minimum-32-chars",
    "Issuer": "LuminaTech",
    "Audience": "LuminaTech",
    "ExpiryMinutes": "60"
  },
  "Authentication": {
    "Google": {
      "ClientId": "YOUR_GOOGLE_CLIENT_ID",
      "ClientSecret": "YOUR_GOOGLE_CLIENT_SECRET"
    }
  },
  "Cloudinary": {
    "CloudName": "YOUR_CLOUD_NAME",
    "ApiKey": "YOUR_API_KEY",
    "ApiSecret": "YOUR_API_SECRET"
  }
}
```

### Run

```bash
cd LuminaTech.API
dotnet run
```

Open **http://localhost:5284** in your browser.

---

## 🔑 Demo Accounts

| Role | Email | Password | Access |
|---|---|---|---|
| User | `demo@luminatech.com` | `Demo@2024!` | Browse, cart, wishlist (sandboxed) |
| Admin | `admin@luminatech.com` | `Admin@2024!` | Admin panel view (sandboxed) |
| Super Admin | `superadmin@luminatech.com` | `LuminaAdmin@2024!` | Full access |

> **Note:** Demo accounts are sandboxed — they can view everything but cannot modify real data.

---

## 📡 API Endpoints

| Route | Methods | Auth | Description |
|---|---|---|---|
| `/api/auth/*` | GET, POST | No | Authentication (login, OAuth, status) |
| `/api/product` | GET, POST, PUT, DEL | Mixed | Product CRUD |
| `/api/cart` | GET, POST, PUT, DEL | Yes | Shopping cart operations |
| `/api/wishlist` | GET, POST, DEL | Yes | Wishlist + move-to-cart |
| `/api/order` | GET, POST | Yes | Checkout & order history |
| `/api/admin/users` | GET, PUT | Admin | User management |
| `/api/media/upload` | POST | Admin | Cloudinary image upload |
| `/api/contact` | POST | No | Contact form submission |

Full API docs available at `/swagger` when running in development mode.

---

## 🔒 Security

- **JWT in HTTP-only cookies** — Prevents XSS token theft
- **HMAC-SHA256 token signing** — Tamper-proof tokens
- **Demo account sandboxing** — Demo users get `isDemo:true` claim, write ops return 403
- **Role-based authorization** — `[Authorize(Roles = "Admin")]` on sensitive endpoints
- **Google OAuth 2.0** — No password storage for OAuth users

---

## 📦 Deployment

```bash
# Build for production
dotnet publish -c Release -o ./publish

# Run production build
cd publish
dotnet LuminaTech.API.dll
```

For production, consider:
- Switch SQLite → PostgreSQL or SQL Server
- Set `ASPNETCORE_ENVIRONMENT=Production`
- Use HTTPS with proper certificates
- Store secrets in environment variables or Azure Key Vault

---

## 📄 Documentation

See [PROJECT_GUIDE.md](./PROJECT_GUIDE.md) for a comprehensive guide covering:
- .NET fundamentals & CLI commands
- File-by-file code breakdown
- Request flow diagrams
- Database schema & EF Core patterns
- Authentication deep dive
- Interview Q&A (20+ questions)

---

## 🛠️ Built With

- [ASP.NET Core 8](https://dotnet.microsoft.com/) — Web framework
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/) — ORM
- [SQLite](https://www.sqlite.org/) — Database
- [Cloudinary](https://cloudinary.com/) — Media management & CDN
- [Google OAuth 2.0](https://developers.google.com/identity) — Authentication
- [GSAP](https://greensock.com/gsap/) — Animations
- [Serilog](https://serilog.net/) — Structured logging
- [Swagger](https://swagger.io/) — API documentation

---

<p align="center">
  Made with ❤️ by <a href="https://github.com/Dhruv2183">Dhruv</a>
</p>
