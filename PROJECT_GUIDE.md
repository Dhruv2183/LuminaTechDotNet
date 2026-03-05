# LuminaTech — Complete Project Guide & Interview Prep

> A full-stack web application built with **ASP.NET Core 8**, **Entity Framework Core**, **SQLite**, **JWT Authentication**, **Google OAuth 2.0**, and **Cloudinary** for media management.

---

## Table of Contents

1. [Why .NET?](#1-why-net)
2. [Installing .NET](#2-installing-net)
3. [Essential .NET CLI Commands](#3-essential-net-cli-commands)
4. [Project Architecture](#4-project-architecture)
5. [File-by-File Breakdown](#5-file-by-file-breakdown)
6. [How the Application Works (Request Flow)](#6-how-the-application-works-request-flow)
7. [Technologies Deep Dive](#7-technologies-deep-dive)
8. [Database & Entity Framework Core](#8-database--entity-framework-core)
9. [Authentication System](#9-authentication-system)
10. [Security Design](#10-security-design)
11. [Frontend Architecture](#11-frontend-architecture)
12. [Cloudinary Integration](#12-cloudinary-integration)
13. [API Endpoints Reference](#13-api-endpoints-reference)
14. [Interview Questions & Answers](#14-interview-questions--answers)

---

## 1. Why .NET?

### What is .NET?
.NET is a **free, open-source, cross-platform** framework created by Microsoft for building modern applications — web APIs, desktop apps, mobile apps, cloud services, games, IoT, and more.

### Why Choose .NET Over Others?

| Feature | .NET (ASP.NET Core) | Node.js (Express) | Python (Django) |
|---|---|---|---|
| **Performance** | ⚡ Extremely fast (compiled, JIT) | Moderate (single-threaded) | Slower (interpreted) |
| **Type Safety** | ✅ Strong (C# is statically typed) | ❌ Weak (JavaScript) | ❌ Dynamic typing |
| **Scalability** | Enterprise-grade, built-in DI | Good but needs more config | Good for mid-scale |
| **Security** | Built-in (Identity, JWT, OAuth) | Needs packages | Django has good defaults |
| **ORM** | Entity Framework Core (excellent) | Sequelize / Prisma | Django ORM |
| **Ecosystem** | NuGet (350K+ packages) | npm (2M+ packages) | PyPI (400K+) |
| **IDE Support** | Visual Studio / VS Code / Rider | VS Code | VS Code / PyCharm |
| **Enterprise Adoption** | Microsoft, Stack Overflow, GoDaddy | Netflix, PayPal | Instagram, Spotify |

### Key Advantages:
1. **Cross-platform** — Runs on Windows, macOS, Linux
2. **High performance** — One of the fastest web frameworks (TechEmpower benchmarks)
3. **Built-in Dependency Injection** — No need for external DI containers
4. **Middleware pipeline** — Clean, composable request processing
5. **Entity Framework Core** — Powerful ORM with migrations
6. **Swagger/OpenAPI** — Auto-generated API documentation
7. **Kestrel web server** — High-performance, production-ready
8. **Long-term support (LTS)** — .NET 8 is LTS until November 2026

---

## 2. Installing .NET

### On macOS:
```bash
# Using Homebrew (recommended)
brew install dotnet-sdk

# Or download from Microsoft
# https://dotnet.microsoft.com/download

# Verify installation
dotnet --version
# Output: 8.0.xxx
```

### On Windows:
```bash
# Using winget
winget install Microsoft.DotNet.SDK.8

# Or download the installer from
# https://dotnet.microsoft.com/download
```

### On Linux (Ubuntu):
```bash
sudo apt-get update
sudo apt-get install -y dotnet-sdk-8.0
```

### Verify Everything:
```bash
dotnet --version       # SDK version
dotnet --list-sdks     # All installed SDKs
dotnet --list-runtimes # All installed runtimes
dotnet --info          # Full environment info
```

---

## 3. Essential .NET CLI Commands

### Project Management
```bash
# Create a new solution
dotnet new sln -n MyProject

# Create a new Web API project
dotnet new webapi -n MyProject.API

# Create a class library (for Data/Services layers)
dotnet new classlib -n MyProject.Data

# Add project to solution
dotnet sln add MyProject.API/MyProject.API.csproj

# Add reference between projects
dotnet add MyProject.API reference MyProject.Data
```

### NuGet Packages
```bash
# Add a package
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package Serilog.AspNetCore
dotnet add package CloudinaryDotNet

# Remove a package
dotnet remove package PackageName

# List installed packages
dotnet list package

# Restore all packages
dotnet restore
```

### Building & Running
```bash
# Build the project
dotnet build

# Run in development mode (hot reload)
dotnet run

# Run with hot reload (watches for file changes)
dotnet watch run

# Run on specific port
dotnet run --urls "http://localhost:5284"

# Build for production
dotnet publish -c Release -o ./publish

# Run tests
dotnet test

# Clean build artifacts
dotnet clean
```

### Entity Framework Commands
```bash
# Install EF tools globally
dotnet tool install --global dotnet-ef

# Add a migration
dotnet ef migrations add InitialCreate

# Apply migrations
dotnet ef database update

# Remove last migration
dotnet ef migrations remove

# Generate SQL script
dotnet ef migrations script
```

---

## 4. Project Architecture

### Solution Structure (Clean Architecture)
```
LuminaTech/
├── LuminaTech.sln                    ← Solution file (groups all projects)
│
├── LuminaTech.API/                   ← PRESENTATION LAYER (Web API)
│   ├── Controllers/                  ← API endpoints
│   │   ├── AuthController.cs         ← Login, OAuth, demo accounts
│   │   ├── AdminController.cs        ← User management (admin only)
│   │   ├── ProductController.cs      ← CRUD for products
│   │   ├── CartController.cs         ← Shopping cart operations
│   │   ├── WishlistController.cs     ← Wishlist operations
│   │   ├── OrderController.cs        ← Checkout & order history
│   │   ├── UserController.cs         ← User profile operations
│   │   ├── ContactController.cs      ← Contact form submissions
│   │   └── MediaController.cs        ← Image upload (Cloudinary)
│   ├── wwwroot/                      ← Static frontend files
│   │   ├── css/style.css             ← Complete design system
│   │   ├── js/app.js                 ← Main application logic
│   │   ├── js/hero-animation.js      ← Scroll-driven hero animation
│   │   ├── index.html                ← Landing page
│   │   ├── products.html             ← Product catalog
│   │   ├── product-detail.html       ← Single product view
│   │   ├── login.html                ← Authentication page
│   │   ├── dashboard.html            ← User dashboard
│   │   ├── admin.html                ← Admin panel
│   │   ├── cart.html                 ← Shopping cart
│   │   ├── wishlist.html             ← User wishlist
│   │   ├── checkout.html             ← Payment & checkout
│   │   └── contact.html              ← Contact form
│   ├── Program.cs                    ← App entry point & middleware config
│   ├── appsettings.json              ← Configuration (DB, JWT, OAuth, Cloudinary)
│   └── LuminaTech.API.csproj         ← Project file & dependencies
│
├── LuminaTech.Data/                  ← DATA LAYER
│   ├── Entities/                     ← Database models (POCOs)
│   │   ├── User.cs                   ← User entity
│   │   ├── Product.cs                ← Product entity
│   │   ├── CartItem.cs               ← Cart item entity
│   │   ├── Wishlist.cs               ← Wishlist entity
│   │   ├── Order.cs                  ← Order & OrderItem entities
│   │   └── ContactSubmission.cs      ← Contact form entity
│   ├── LuminaTechDbContext.cs        ← EF Core DbContext + seed data
│   └── LuminaTech.Data.csproj        ← Project file
│
├── LuminaTech.Services/              ← BUSINESS LOGIC LAYER
│   ├── AuthService.cs                ← JWT token generation
│   ├── CloudinaryService.cs          ← Image upload/delete service
│   └── LuminaTech.Services.csproj    ← Project file
│
└── LuminaTech.Tests/                 ← TEST LAYER
    └── LuminaTech.Tests.csproj       ← Test project file
```

### Why This Architecture?

**Separation of Concerns** — Each layer has a single responsibility:

```
┌─────────────────────────────────────────────────┐
│           PRESENTATION (LuminaTech.API)         │
│   Controllers ← Receive HTTP requests           │
│   wwwroot     ← Serve frontend files             │
├─────────────────────────────────────────────────┤
│           BUSINESS LOGIC (LuminaTech.Services)  │
│   AuthService       ← JWT token logic            │
│   CloudinaryService ← Image upload logic          │
├─────────────────────────────────────────────────┤
│           DATA ACCESS (LuminaTech.Data)          │
│   DbContext ← Database operations                 │
│   Entities  ← Database table definitions          │
└─────────────────────────────────────────────────┘
```

Dependencies flow **downward only** — API depends on Services, Services depends on Data. Data depends on nothing.

---

## 5. File-by-File Breakdown

### `Program.cs` — The Brain of the Application

This is where **everything is configured**. Think of it as the master setup file.

```csharp
// 1. CREATE THE BUILDER
var builder = WebApplication.CreateBuilder(args);

// 2. REGISTER SERVICES (Dependency Injection)
builder.Services.AddDbContext<LuminaTechDbContext>(...);   // Database
builder.Services.AddScoped<IAuthService, AuthService>();   // Auth logic
builder.Services.AddAuthentication(...)                     // JWT + Google OAuth
builder.Services.AddControllers();                          // API controllers
builder.Services.AddSwaggerGen();                           // API docs

// 3. BUILD THE APP
var app = builder.Build();

// 4. CONFIGURE MIDDLEWARE PIPELINE (order matters!)
app.UseStaticFiles();      // Serve HTML/CSS/JS
app.UseAuthentication();   // Check who the user is
app.UseAuthorization();    // Check what they can do
app.MapControllers();      // Route to controllers

// 5. RUN
app.Run();
```

**Key Concept: Middleware runs in ORDER.** Each request passes through the pipeline like a conveyor belt:
```
Request → StaticFiles → Auth → Authorization → Controller → Response
```

### `appsettings.json` — Configuration

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=luminatech.db"    // SQLite database file
  },
  "Jwt": {
    "Key": "secret-key-here",         // HMAC key for signing tokens
    "Issuer": "LuminaTech",           // Who issued the token
    "Audience": "LuminaTech",         // Who the token is for
    "ExpiryMinutes": "60"             // Token lifetime
  },
  "Authentication": {
    "Google": {
      "ClientId": "...",              // From Google Cloud Console
      "ClientSecret": "..."           // From Google Cloud Console
    }
  },
  "Cloudinary": {
    "CloudName": "dhgwfh6x0",        // Your Cloudinary account
    "ApiKey": "952651983569773",
    "ApiSecret": "..."
  }
}
```

### `LuminaTechDbContext.cs` — The Database Blueprint

```csharp
public class LuminaTechDbContext : DbContext
{
    // These define your database TABLES
    public DbSet<User> Users => Set<User>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Wishlist> Wishlists => Set<Wishlist>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Define relationships
        modelBuilder.Entity<Wishlist>()
            .HasKey(w => new { w.UserId, w.ProductId });  // Composite key

        // Seed initial data (products, demo users, admin)
        modelBuilder.Entity<Product>().HasData(...);
        modelBuilder.Entity<User>().HasData(...);
    }
}
```

### Entity Classes — Database Tables as C# Classes

```csharp
// Each property = a column in the database
public class Product
{
    public int Id { get; set; }             // Primary key (auto-increment)
    public string Name { get; set; }        // Product name
    public string Description { get; set; }
    public decimal Price { get; set; }      // decimal(10,2) for money
    public string Category { get; set; }
    public string ImageUrl { get; set; }    // Cloudinary URL
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation property (EF Core knows this is a relationship)
    public ICollection<Wishlist> Wishlists { get; set; }
}
```

### Controllers — API Endpoints

```csharp
[ApiController]                          // Enables automatic model validation
[Route("api/[controller]")]             // Route = /api/product
[Authorize]                              // All endpoints require authentication
public class ProductController : ControllerBase
{
    private readonly LuminaTechDbContext _context;  // Injected by DI

    // GET /api/product
    [HttpGet]
    [AllowAnonymous]                     // Override: no auth needed
    public async Task<IActionResult> GetAll()
    {
        var products = await _context.Products
            .Where(p => p.IsActive)      // LINQ query
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
        return Ok(products);             // Returns 200 + JSON
    }

    // POST /api/product
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return Ok(product);
    }
}
```

---

## 6. How the Application Works (Request Flow)

### Example: User visits the Products page

```
1. Browser requests GET /products.html
   ↓
2. Kestrel (web server) receives the request
   ↓
3. Middleware pipeline processes it:
   UseStaticFiles() → finds products.html in wwwroot/ → serves it
   ↓
4. Browser loads products.html, which loads:
   - /css/style.css (styling)
   - /js/app.js (JavaScript logic)
   ↓
5. app.js calls: fetch('/api/product')
   ↓
6. Middleware pipeline processes the API request:
   UseStaticFiles() → not a static file, passes through
   UseAuthentication() → no JWT cookie, user is anonymous
   UseAuthorization() → [AllowAnonymous] allows it
   MapControllers() → routes to ProductController.GetAll()
   ↓
7. Controller queries SQLite via EF Core:
   SELECT * FROM Products WHERE IsActive = 1
   ↓
8. Returns JSON response to browser
   ↓
9. app.js renders product cards with the data
```

### Example: User adds item to cart

```
1. User clicks "Add to Cart" button
   ↓
2. app.js calls: fetch('/api/cart', { method: 'POST', body: {productId: 1} })
   ↓
3. Middleware pipeline:
   UseAuthentication() → reads JWT from cookie → validates signature
   UseAuthorization() → [Authorize] → user is authenticated ✓
   ↓
4. CartController.AddToCart() runs:
   - Reads userId from JWT claims
   - Checks if product already in cart
   - If yes: increment quantity
   - If no: add new CartItem
   - SaveChanges() → INSERT INTO CartItems
   ↓
5. Returns { message: "Added to cart" }
   ↓
6. app.js shows success toast notification
```

---

## 7. Technologies Deep Dive

### ASP.NET Core 8
- **What**: Microsoft's web framework for building APIs and web apps
- **Why**: High performance, built-in DI, middleware pipeline, cross-platform
- **In this project**: Handles all HTTP requests, routing, authentication

### Entity Framework Core (EF Core)
- **What**: Object-Relational Mapper (ORM) — maps C# classes to database tables
- **Why**: Write C# code instead of raw SQL, automatic migrations, LINQ queries
- **In this project**: All database operations (CRUD for users, products, orders)

```csharp
// Instead of writing SQL:
// SELECT * FROM Products WHERE Category = 'Hardware' ORDER BY Price DESC

// You write C#:
var products = await _context.Products
    .Where(p => p.Category == "Hardware")
    .OrderByDescending(p => p.Price)
    .ToListAsync();
```

### SQLite
- **What**: Lightweight, file-based relational database
- **Why**: No server setup needed, single file (`luminatech.db`), perfect for dev/small apps
- **In this project**: Stores all application data
- **Production alternative**: PostgreSQL, SQL Server, MySQL

### JWT (JSON Web Tokens)
- **What**: Stateless authentication tokens
- **Why**: No server-side session storage needed, works across multiple servers
- **How it works**:
```
1. User logs in → server creates JWT with user info
2. JWT stored in HTTP-only cookie
3. Every request sends JWT → server validates signature
4. Server extracts userId, role from token claims
```

**JWT Structure**:
```
eyJhbGciOiJIUzI1NiJ9.       ← HEADER (algorithm)
eyJ1c2VySWQiOiIxIn0.         ← PAYLOAD (claims: userId, role, email)
SflKxwRJSMeKKF2QT4fwp        ← SIGNATURE (HMAC-SHA256)
```

### Google OAuth 2.0
- **What**: "Sign in with Google" — delegated authentication
- **Why**: Users don't need to create/remember passwords
- **Flow**:
```
1. User clicks "Sign in with Google"
2. Redirected to Google's login page
3. User grants permission
4. Google redirects back with authorization code
5. Server exchanges code for user info (email, name, picture)
6. Server creates/finds user in DB → generates JWT
```

### Cloudinary
- **What**: Cloud-based media management platform
- **Why**: CDN delivery (fast global loading), automatic optimization, image transformations
- **In this project**: Stores product images and hero video
- **URL format**: `https://res.cloudinary.com/{cloud_name}/image/upload/v{version}/{folder}/{filename}`

### GSAP (GreenSock Animation Platform)
- **What**: JavaScript animation library
- **Why**: Smooth, performant animations with simple API
- **In this project**: Product card entrance animations, scroll-triggered reveals

### Serilog
- **What**: Structured logging library for .NET
- **Why**: Better than built-in logging, supports structured data, multiple output "sinks"
- **In this project**: Request logging, error tracking

---

## 8. Database & Entity Framework Core

### Entity Relationship Diagram

```
┌──────────┐     ┌──────────────┐     ┌───────────┐
│   User   │────<│   Wishlist   │>────│  Product  │
│──────────│     │──────────────│     │───────────│
│ Id (PK)  │     │ UserId (FK)  │     │ Id (PK)   │
│ Email    │     │ ProductId(FK)│     │ Name      │
│ FullName │     │ AddedAt      │     │ Price     │
│ Role     │     └──────────────┘     │ Category  │
│ GoogleId │                          │ ImageUrl  │
└──────────┘     ┌──────────────┐     └───────────┘
     │           │  CartItem    │          │
     │────────<──│──────────────│──>───────│
     │           │ Id (PK)      │          │
     │           │ UserId (FK)  │          │
     │           │ ProductId(FK)│          │
     │           │ Quantity     │          │
     │           └──────────────┘          │
     │                                     │
     │           ┌──────────────┐          │
     │────────<──│    Order     │          │
                 │──────────────│          │
                 │ Id (PK)      │          │
                 │ UserId (FK)  │          │
                 │ OrderNumber  │          │
                 │ TotalAmount  │          │
                 │ Status       │          │
                 └──────┬───────┘          │
                        │                  │
                 ┌──────────────┐          │
                 │  OrderItem   │──>───────┘
                 │──────────────│
                 │ Id (PK)      │
                 │ OrderId (FK) │
                 │ ProductId(FK)│
                 │ Quantity     │
                 │ UnitPrice    │
                 └──────────────┘
```

### Key EF Core Concepts

**DbSet** = A table in the database
```csharp
public DbSet<Product> Products => Set<Product>();
// This creates a "Products" table with columns matching Product class properties
```

**LINQ Queries** = Type-safe SQL
```csharp
// Find user by email
var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

// Get products with filters
var products = await _context.Products
    .Where(p => p.IsActive && p.Category == "Hardware")
    .OrderBy(p => p.Price)
    .Take(10)
    .ToListAsync();

// Join: Get cart items with product details
var cart = await _context.CartItems
    .Where(c => c.UserId == userId)
    .Include(c => c.Product)           // JOIN with Products table
    .ToListAsync();
```

**Tracking vs. No-Tracking**
```csharp
// Tracking (default) — EF watches for changes
var product = await _context.Products.FindAsync(1);
product.Price = 999.99m;
await _context.SaveChangesAsync();  // UPDATE query generated automatically

// No-tracking — faster for read-only queries
var products = await _context.Products
    .AsNoTracking()
    .ToListAsync();
```

---

## 9. Authentication System

### Three Auth Methods in This Project

| Method | Used For | How It Works |
|---|---|---|
| **Google OAuth** | Real users | Redirects to Google → gets user info → creates JWT |
| **Demo Login** | Testing (sandboxed) | Predefined accounts with `isDemo:true` claim |
| **Super Admin** | Full admin access | Email/password login (`superadmin@luminatech.com`) |

### JWT Token Creation (AuthService.cs)

```csharp
public string GenerateJwtToken(User user, bool isDemo = false)
{
    var claims = new List<Claim>
    {
        new("userId", user.Id.ToString()),
        new(ClaimTypes.Email, user.Email),
        new(ClaimTypes.Role, user.Role),
    };

    if (isDemo)
        claims.Add(new Claim("isDemo", "true"));  // Sandbox flag

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
        issuer: _issuer,
        audience: _audience,
        claims: claims,
        expires: DateTime.UtcNow.AddMinutes(60),
        signingCredentials: creds
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
}
```

### How JWT Validation Works (Program.cs)

```csharp
// On every request with [Authorize]:
// 1. Read JWT from cookie named "jwt"
// 2. Verify HMAC-SHA256 signature with secret key
// 3. Check issuer, audience, expiry
// 4. Extract claims (userId, role, email, isDemo)
// 5. Populate HttpContext.User with claims
```

---

## 10. Security Design

### Demo Account Sandboxing

```csharp
// In controllers — check if user is demo:
private bool IsDemoUser() =>
    HttpContext.User.FindFirst("isDemo")?.Value == "true";

// Before any write operation:
if (IsDemoUser())
    return StatusCode(403, new { error = "Demo users cannot modify data" });
```

### Account Hierarchy

| Account | Email | Role | Can Read | Can Write |
|---|---|---|---|---|
| Google Users | user@gmail.com | User | ✅ | ✅ Own data |
| Demo User | demo@luminatech.com | User | ✅ | ❌ Sandboxed |
| Demo Admin | admin@luminatech.com | Admin | ✅ | ❌ Sandboxed |
| Super Admin | superadmin@luminatech.com | Admin | ✅ | ✅ Everything |

### Security Features
- **HTTP-only cookies** — JWT can't be accessed by JavaScript (prevents XSS)
- **HMAC-SHA256 signing** — Tokens can't be tampered with
- **Role-based authorization** — `[Authorize(Roles = "Admin")]`
- **Demo sandboxing** — Demo accounts return 403 on write operations
- **Input validation** — `[Required]`, `[MaxLength]` on entities

---

## 11. Frontend Architecture

### No Framework — Vanilla HTML/CSS/JS

The frontend uses **static files served by ASP.NET Core** — no React, Angular, or Vue.

### Design System (style.css)

```css
/* CSS Custom Properties — single source of truth for design tokens */
:root {
    --bg-primary: #0A0A0F;           /* Dark background */
    --color-primary: #4F8EF7;        /* Blue accent */
    --color-accent: #7C5CFC;         /* Purple accent */
    --font-display: 'Syne';          /* Headings font */
    --font-body: 'Inter';            /* Body text font */
    --glass-bg: rgba(255,255,255,0.04);  /* Glassmorphism */
    --radius-lg: 20px;               /* Border radius */
}
```

### app.js — Single Page Application Behavior

```javascript
// Core pattern: fetch data from API, render with template literals
async function loadProducts() {
    const products = await API.get('/api/product');
    container.innerHTML = products.map(p => `
        <div class="product-card">
            <img src="${p.imageUrl}" alt="${p.name}">
            <h3>${p.name}</h3>
            <span>$${p.price}</span>
        </div>
    `).join('');
}
```

### Hero Scroll Animation (hero-animation.js)

```javascript
// Directly driven by scroll position — no delay
window.addEventListener('scroll', () => {
    const scrollProgress = window.scrollY / heroHeight;
    // Map scroll position to animation phase (0-1)
    // Phase 0: "Innovation Redefined"
    // Phase 1: "Powered by AI"
    // Phase 2: "Built for Tomorrow"
    updateOverlays(scrollProgress);
});
```

---

## 12. Cloudinary Integration

### Upload Flow (CloudinaryService.cs)

```csharp
public async Task<(string url, string publicId)> UploadImageAsync(IFormFile file)
{
    using var stream = file.OpenReadStream();
    var uploadParams = new ImageUploadParams
    {
        File = new FileDescription(file.FileName, stream),
        Folder = "luminatech"
    };
    var result = await _cloudinary.UploadAsync(uploadParams);
    return (result.SecureUrl.ToString(), result.PublicId);
}
```

### Cloudinary URLs in This Project

| Asset | URL Pattern |
|---|---|
| Product images | `https://res.cloudinary.com/dhgwfh6x0/image/upload/v.../luminatech/product1.png` |
| Hero video | `https://res.cloudinary.com/dhgwfh6x0/video/upload/v.../luminatech/hero.mp4` |
| User uploads | `https://res.cloudinary.com/dhgwfh6x0/image/upload/v.../luminatech/{id}` |

### Why Cloudinary?
- **CDN** — Assets served from nearest edge location (fast globally)
- **Auto-optimization** — Compress without quality loss
- **Transformations** — Resize, crop, format convert via URL params
- **Free tier** — 25 credits/month, enough for development

---

## 13. API Endpoints Reference

### Auth (`/api/auth`)
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | `/api/auth/status` | No | Check if user is logged in |
| POST | `/api/auth/demo-login` | No | Login with demo account |
| POST | `/api/auth/logout` | No | Clear JWT cookie |
| GET | `/api/auth/google-login` | No | Start Google OAuth flow |

### Products (`/api/product`)
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | `/api/product` | No | List all active products |
| GET | `/api/product/{id}` | No | Get product details |
| POST | `/api/product` | Admin | Create product |
| PUT | `/api/product/{id}` | Admin | Update product |
| DELETE | `/api/product/{id}` | Admin | Delete product |

### Cart (`/api/cart`)
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | `/api/cart` | Yes | Get cart items + total |
| POST | `/api/cart` | Yes | Add item to cart |
| PUT | `/api/cart/{id}` | Yes | Update quantity |
| DELETE | `/api/cart/{id}` | Yes | Remove from cart |
| GET | `/api/cart/count` | Yes | Get cart item count |

### Wishlist (`/api/wishlist`)
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | `/api/wishlist` | Yes | Get wishlist items |
| POST | `/api/wishlist` | Yes | Add to wishlist |
| DELETE | `/api/wishlist/{id}` | Yes | Remove from wishlist |
| POST | `/api/wishlist/move-to-cart/{id}` | Yes | Move to cart |

### Orders (`/api/order`)
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | `/api/order` | Yes | Get order history |
| POST | `/api/order/checkout` | Yes | Place order (dummy payment) |

### Admin (`/api/admin`)
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | `/api/admin/users` | Admin | List all users |
| PUT | `/api/admin/users/{id}/role` | Admin | Change user role |
| PUT | `/api/admin/users/{id}/toggle-active` | Admin | Activate/deactivate user |

---

## 14. Interview Questions & Answers

### .NET / C# Fundamentals

**Q: What is the difference between .NET Framework and .NET Core / .NET 8?**
> .NET Framework is Windows-only and the older version. .NET Core (now just ".NET" from v5+) is **cross-platform** (Windows, macOS, Linux), **open-source**, and significantly faster. .NET 8 is the latest LTS release with improved performance, native AOT compilation, and minimal APIs.

**Q: What is Dependency Injection and why is it important?**
> DI is a design pattern where a class receives its dependencies from outside rather than creating them. In ASP.NET Core, DI is **built-in**. Example:
> ```csharp
> // Instead of: var service = new AuthService();
> // We register: builder.Services.AddScoped<IAuthService, AuthService>();
> // And inject:  public CartController(LuminaTechDbContext context) { }
> ```
> Benefits: **Testability** (mock dependencies), **loose coupling**, **single responsibility**.

**Q: What are the DI lifetimes in ASP.NET Core?**
> - **Transient** (`AddTransient`) — New instance every time it's requested
> - **Scoped** (`AddScoped`) — One instance per HTTP request (used for DbContext)
> - **Singleton** (`AddSingleton`) — One instance for the entire app lifetime

**Q: What is middleware in ASP.NET Core?**
> Middleware is software that handles requests and responses in a **pipeline**. Each middleware can process the request, pass it to the next middleware, or short-circuit. Order matters:
> ```
> Request → Logging → StaticFiles → Auth → Authorization → Controller → Response
> ```

**Q: What is the difference between `async Task` and `Task`?**
> `async Task` allows non-blocking I/O operations. When you `await` a database query, the thread is freed to handle other requests instead of blocking. This is crucial for scalability — a sync server handling 100 concurrent DB queries blocks 100 threads, while async needs far fewer.

### Entity Framework Core

**Q: What is the difference between `FirstOrDefault` and `SingleOrDefault`?**
> - `FirstOrDefault` — Returns the first match or null. Use when multiple matches are OK.
> - `SingleOrDefault` — Returns the only match or null. **Throws exception** if multiple matches exist.

**Q: What is the N+1 query problem and how do you solve it?**
> N+1 occurs when EF lazy-loads related data in a loop. Solution: use **eager loading** with `.Include()`:
> ```csharp
> // Bad (N+1): loads cart items, then 1 query per item for Product
> var items = await _context.CartItems.ToListAsync();
>
> // Good (1 query with JOIN):
> var items = await _context.CartItems.Include(c => c.Product).ToListAsync();
> ```

**Q: What is the difference between `EnsureCreated()` and `Migrate()`?**
> - `EnsureCreated()` — Creates the database if it doesn't exist. Does NOT apply migrations. Ignores schema changes.
> - `Migrate()` — Applies all pending migrations. Use this for production apps.

### Authentication & Security

**Q: How does JWT authentication work?**
> 1. User logs in → server creates a token with claims (userId, role) and signs it with HMAC-SHA256
> 2. Token sent to client (stored in HTTP-only cookie)
> 3. Every API request includes the token
> 4. Server validates the signature and extracts claims
> 5. No database lookup needed — the token IS the proof of identity

**Q: What is the difference between Authentication and Authorization?**
> - **Authentication** = "Who are you?" (JWT validation, Google OAuth)
> - **Authorization** = "What can you do?" (`[Authorize(Roles = "Admin")]`)

**Q: How do you prevent XSS attacks?**
> - Store JWT in **HTTP-only cookies** (JavaScript can't access them)
> - Validate and sanitize all user input
> - Use Content Security Policy (CSP) headers

**Q: What is OAuth 2.0?**
> OAuth is a protocol that allows users to grant third-party apps access to their information without sharing passwords. In Google OAuth:
> 1. Your app redirects to Google
> 2. User authenticates with Google
> 3. Google redirects back with an authorization code
> 4. Your server exchanges the code for user info
> 5. Your server creates a local account/session

### Architecture & Design

**Q: What is Clean Architecture?**
> Organizing code into layers with clear dependencies:
> - **Presentation** (API/Controllers) → depends on →
> - **Business Logic** (Services) → depends on →
> - **Data Access** (DbContext/Entities)
> 
> Inner layers don't know about outer layers. This makes testing, maintenance, and swapping components easy.

**Q: What is the Repository Pattern?**
> An abstraction layer between business logic and data access. In this project, controllers use DbContext directly (acceptable for smaller apps), but in larger apps you'd add:
> ```csharp
> public interface IProductRepository
> {
>     Task<List<Product>> GetAllAsync();
>     Task<Product?> GetByIdAsync(int id);
>     Task CreateAsync(Product product);
> }
> ```

**Q: What is CORS and why do we need it?**
> Cross-Origin Resource Sharing — browsers block requests from one domain to another by default. CORS headers tell the browser which origins are allowed. In dev, we use `AllowAll`; in production, you whitelist specific domains.

### Cloudinary / CDN

**Q: Why use a CDN for images instead of serving from the server?**
> - **Speed**: CDN serves from nearest edge location (e.g., Mumbai instead of US East)
> - **Reduced server load**: Your server handles only API requests, not file serving
> - **Auto-optimization**: Cloudinary compresses images, converts formats (WebP)
> - **Scalability**: CDN handles millions of requests without affecting your server

### Frontend

**Q: Why vanilla JS instead of React/Angular?**
> For a project of this size, vanilla JS is:
> - **Lighter** (no 40KB+ framework bundle)
> - **Faster to load** (no hydration step)
> - **Simpler to deploy** (just static files)
> - **Easier to understand** (no virtual DOM, no state management)
>
> For larger apps with complex state management, React/Angular would be better.

**Q: What is the CSS `backdrop-filter` property?**
> It applies visual effects (like blur) to the **area behind** an element. Used for glassmorphism effects:
> ```css
> .glass-card {
>     background: rgba(255, 255, 255, 0.04);
>     backdrop-filter: blur(16px);  /* Blurs content behind the card */
> }
> ```

---

## Quick Start Guide

```bash
# 1. Clone the project
cd /Users/apple/Desktop/DotNet/LuminaTech

# 2. Restore packages
dotnet restore

# 3. Build
dotnet build

# 4. Run the server
cd LuminaTech.API
dotnet run

# 5. Open in browser
# http://localhost:5284

# 6. Demo Accounts:
#    User:  demo@luminatech.com / Demo@2024!
#    Admin: admin@luminatech.com / Admin@2024!
#    Real Admin: superadmin@luminatech.com / LuminaAdmin@2024!
```

---

*This guide covers the full LuminaTech project. For deeper dives into any topic, refer to the source code files mentioned throughout.*
