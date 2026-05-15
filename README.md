# HomeTaste — Food Delivery Platform API

A full-featured food delivery backend built with **ASP.NET Core 8** following Clean Architecture principles. Handles everything from meal browsing and order placement to payment processing, delivery tracking, loyalty rewards, and real-time notifications.

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Framework | ASP.NET Core 8.0 |
| ORM | Entity Framework Core 8 (SQL Server) |
| Auth | ASP.NET Core Identity + JWT Bearer + Refresh Tokens |
| Payments | Stripe.NET |
| Image Storage | Cloudinary |
| Real-time | SignalR |
| Documentation | Swagger / Swashbuckle |
| Micro-queries | Dapper |

## Architecture

```
HomeTaste.sln
├── HomeTaste.API           # Controllers, middleware, SignalR hubs, DI wiring
├── HomeTaste.Application   # Business logic, DTOs, interfaces, validators
├── HomeTaste.Domain        # Entities, enums, base types
├── HomeTaste.Infrastructure # EF Core, Identity, repositories, Cloudinary, Stripe
└── HomeTaste.UnitTests     # Unit tests
```

Clean Architecture — dependencies flow inward: API → Application → Domain. Infrastructure implements Application interfaces.

## Features

| Domain | Capabilities |
|--------|-------------|
| Auth | Register, login, JWT + refresh tokens, role-based access, logout |
| Users | Profile management, avatar upload (Cloudinary), password change |
| Meals | CRUD with image upload, categories, ingredients, customization options, availability, discount pricing, prep time, calories |
| Inventory | Stock tracking, inventory transactions |
| Orders | Place order, status workflow (Pending → Delivered), cancel, coupon redemption, loyalty points |
| Payments | Stripe initiation + confirmation, refunds, transaction history |
| Delivery | Personnel management, assignment tracking, availability toggle |
| Loyalty | Points accrual, tier management, redemption preview, admin adjustments |
| Reviews | Submit, edit, delete reviews; average rating per meal |
| Coupons | Admin CRUD, customer validation |
| Support | Ticket submission and tracking |
| Notifications | Real-time via SignalR, unread count, mark as read |
| Analytics | Dashboard KPIs, daily revenue, top meals, top customers |
| Addresses | User delivery addresses |

## Roles & Policies

| Policy | Roles |
|--------|-------|
| `AdminOnly` | Admin |
| `CustomerOnly` | Customer |
| `DeliveryPersonnelOnly` | DeliveryPersonnel |
| `AdminOrDelivery` | Admin, DeliveryPersonnel |
| `AdminOrCustomer` | Admin, Customer |

Default roles and an admin seed user are created on first startup.

## Getting Started

### Prerequisites

- .NET 8 SDK
- SQL Server (local or remote)
- Cloudinary account
- Stripe account

### Configuration

Copy `appsettings.json` and fill in:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;Database=HomeTaste;..."
  },
  "JwtSettings": {
    "SecretKey": "...",
    "Issuer": "HomeTaste",
    "Audience": "HomeTasteClient",
    "ExpiryMinutes": 60
  },
  "CloudinarySettings": {
    "CloudName": "...",
    "ApiKey": "...",
    "ApiSecret": "..."
  },
  "Stripe": {
    "SecretKey": "sk_test_...",
    "WebhookSecret": "whsec_..."
  }
}
```

### Run

```bash
# Apply migrations
dotnet ef database update --project HomeTaste.Infrastructure --startup-project HomeTaste.API

# Start API
dotnet run --project HomeTaste.API
```

Swagger UI: `https://localhost:7082/swagger`

## API Overview

| Prefix | Resource |
|--------|----------|
| `api/auth` | Authentication (register, login, refresh, logout) |
| `api/userprofile` | Profile, avatar, password |
| `api/meals` | Meal CRUD + image upload |
| `api/mealcategories` | Meal categories |
| `api/ingredients` | Ingredient management |
| `api/units` | Units of measurement |
| `api/mealingredients` | Meal–ingredient mapping |
| `api/mealcustomization` | Meal add-ons / removals / substitutions |
| `api/mealreviews` | Customer reviews + ratings |
| `api/inventory` | Stock levels + transactions |
| `api/order` | Place, track, cancel orders |
| `api/payment` | Stripe payment flow |
| `api/coupon` | Coupon management + validation |
| `api/delivery` | Personnel + assignment management |
| `api/loyalty` | Points, tiers, redemption |
| `api/notification` | Real-time notifications |
| `api/analytics` | Admin dashboard data |
| `api/address` | Delivery addresses |
| `api/support` | Support tickets |

Full request/response schemas available in Swagger.

## Domain Model (key entities)

```
ApplicationUser
  ├── Address[]
  ├── Order[]
  │     ├── OrderItem[]
  │     │     └── OrderItemCustomization[]
  │     └── PaymentTransaction
  ├── LoyaltyAccount
  │     └── LoyaltyTransaction[]
  ├── MealReview[]
  └── SupportTicket[]

Meal
  ├── MealCategory
  ├── MealIngredient[] → Ingredient + Unit
  ├── MealCustomizationOption[]
  └── MealReview[]

DeliveryPersonnel
  └── DeliveryAssignment[] → Order
```

## Frontend

The React client lives at [Nirob-Barman/HomeTasteClient](https://github.com/Nirob-Barman/HomeTasteClient). See its README for setup instructions.
