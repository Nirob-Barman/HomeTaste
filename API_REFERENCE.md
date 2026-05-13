# HomeTaste API — User Manual

HomeTaste is an ASP.NET Core 8 Web API platform for a homemade food delivery service. It supports three actor types — **Customer**, **Admin**, and **DeliveryPersonnel** — and provides meal management, ordering, payments, delivery tracking, loyalty rewards, real-time notifications, and admin analytics.

---

## Table of Contents

1. [Getting Started](#1-getting-started)
2. [Roles & Default Credentials](#2-roles--default-credentials)
3. [Authentication](#3-authentication)
4. [Response Format](#4-response-format)
5. [Enums Reference](#5-enums-reference)
6. [Meals & Catalog](#6-meals--catalog)
7. [Addresses](#7-addresses)
8. [Orders](#8-orders)
9. [Payments](#9-payments)
10. [Delivery](#10-delivery)
11. [Coupons](#11-coupons)
12. [Loyalty Program](#12-loyalty-program)
13. [Reviews](#13-reviews)
14. [Support Tickets](#14-support-tickets)
15. [Notifications](#15-notifications)
16. [User Profile](#16-user-profile)
17. [Admin Features](#17-admin-features)
18. [Real-Time (SignalR)](#18-real-time-signalr)
19. [Error Reference](#19-error-reference)

---

## 1. Getting Started

### Prerequisites
- .NET 8 SDK
- SQL Server (default: `WINDOWS\SQLEXPRESS`)
- `dotnet-ef` tool: `dotnet tool install --global dotnet-ef`

### Build & Run

```bash
# 1. Restore and build
dotnet build

# 2. Apply all migrations
dotnet ef database update --project HomeTaste.Infrastructure --startup-project HomeTaste.API

# 3. Run the API
dotnet run --project HomeTaste.API
```

### Swagger UI

Navigate to `https://localhost:{port}/swagger` in a browser. The API auto-seeds roles and a default admin account on first startup.

### Authorize in Swagger

1. Call `POST /api/auth/login` with the admin or a registered user's credentials.
2. Copy the `accessToken` from the response.
3. Click the **Authorize** button (top right of Swagger UI).
4. Paste the token — **do not** prefix it with `Bearer`. Swagger adds the prefix automatically.
5. Click **Authorize** and close the dialog. All protected endpoints will now include the token.

---

## 2. Roles & Default Credentials

| Role | What they can do |
|------|-----------------|
| `Admin` | Full access — users, meals, inventory, orders, analytics, support |
| `Customer` | Register, browse meals, place orders, pay, review, loyalty rewards |
| `DeliveryPersonnel` | View assigned deliveries, update delivery status, update GPS location |

### Seeded Admin Account

| Field | Value |
|-------|-------|
| Email | `admin@HomeTaste.com` |
| Password | `Admin@123` |

> Change this password after first login in production.

### Registering Users

Pass the desired role in the `role` field when registering. Valid values: `Customer`, `DeliveryPersonnel`. (`Admin` accounts should be promoted via the admin user management API, not self-registered.)

---

## 3. Authentication

All auth endpoints are public (no token required).

### Register

```
POST /api/auth/register
```

```json
{
  "firstName": "Jane",
  "lastName": "Doe",
  "dateOfBirth": "1995-06-15",
  "email": "jane@example.com",
  "password": "Secure@123",
  "role": "Customer"
}
```

### Login

```
POST /api/auth/login
```

```json
{
  "email": "jane@example.com",
  "password": "Secure@123"
}
```

**Response `data`:**

```json
{
  "accessToken": "eyJhbGci...",
  "refreshToken": "dGhpcyBp...",
  "expiresIn": 3600
}
```

The `accessToken` expires in **60 minutes**. The `refreshToken` expires in **24 hours**.

### Refresh Token

```
POST /api/auth/refresh-token
```

```json
{
  "refreshToken": "dGhpcyBp..."
}
```

Returns a new `accessToken` and `refreshToken` pair.

### Get Current User

```
GET /api/auth/me
```
*Requires: any authenticated user*

### Logout

```
POST /api/auth/logout
```
*Requires: any authenticated user* — invalidates the current refresh token.

---

## 4. Response Format

Every endpoint returns a consistent envelope:

```json
{
  "success": true,
  "message": "Operation completed successfully.",
  "data": { },
  "errors": []
}
```

| Field | Description |
|-------|-------------|
| `success` | `true` on success, `false` on any error |
| `message` | Human-readable summary |
| `data` | The response payload (null on errors) |
| `errors` | Array of error strings (empty on success) |

### Paginated Responses

List endpoints that support pagination return:

```json
{
  "success": true,
  "data": {
    "data": [ ],
    "metaData": {
      "pageNumber": 1,
      "pageSize": 10,
      "totalCount": 42,
      "totalPages": 5,
      "currentPageCount": 10,
      "hasPreviousPage": false,
      "hasNextPage": true
    }
  }
}
```

Pass `?pageNumber=1&pageSize=10` as query parameters. Most list endpoints also accept `?searchTerm=`.

---

## 5. Enums Reference

### OrderStatus
| Value | Integer |
|-------|---------|
| Pending | 1 |
| Confirmed | 2 |
| Preparing | 3 |
| ReadyForPickup | 4 |
| OutForDelivery | 5 |
| Delivered | 6 |
| Cancelled | 7 |
| Refunded | 8 |

### PaymentStatus
| Value | Integer |
|-------|---------|
| Pending | 1 |
| Success | 2 |
| Failed | 3 |
| Refunded | 4 |

### DeliveryStatus
| Value | Integer |
|-------|---------|
| Assigned | 1 |
| PickedUp | 2 |
| Delivered | 3 |
| Failed | 4 |

### LoyaltyTier
| Value | Integer | Min Points Earned |
|-------|---------|-------------------|
| Bronze | 1 | 0 |
| Silver | 2 | 1,000 |
| Gold | 3 | 5,000 |
| Platinum | 4 | 10,000 |

### LoyaltyTransactionType
| Value | Integer |
|-------|---------|
| Earned | 1 |
| Redeemed | 2 |
| Expired | 3 |
| Adjusted | 4 |

### DiscountType
| Value | Integer |
|-------|---------|
| Percentage | 1 |
| Flat | 2 |

### CustomizationOptionType
| Value | Integer |
|-------|---------|
| AddOn | 1 |
| Removal | 2 |
| Substitution | 3 |

### NotificationType
| Value | Integer |
|-------|---------|
| OrderStatus | 1 |
| Payment | 2 |
| Delivery | 3 |
| Promotion | 4 |
| System | 5 |

### TicketPriority
| Value | Integer |
|-------|---------|
| Low | 1 |
| Medium | 2 |
| High | 3 |
| Urgent | 4 |

### TicketStatus
| Value | Integer |
|-------|---------|
| Open | 1 |
| Resolved | 2 |
| InProgress | 3 |
| Closed | 4 |

---

## 6. Meals & Catalog

Meal catalog endpoints are **public** — no token required for reads.

### Meal Categories

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/mealcategories` | Public | List categories (paginated, searchable) |
| GET | `/api/mealcategories/{id}` | Public | Get category by ID |
| POST | `/api/mealcategories` | Public | Create category |
| PUT | `/api/mealcategories/{id}` | Public | Update category |
| DELETE | `/api/mealcategories/{id}` | Public | Delete category |
| POST | `/api/mealcategories/bulk-insert` | Public | Seed predefined Bengali food categories |

### Meals

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/meals` | Public | List meals (paginated, searchable, with category name) |
| GET | `/api/meals/{id}` | Public | Get meal by ID |
| POST | `/api/meals` | Public | Create meal |
| PUT | `/api/meals/{id}` | Public | Update meal |
| DELETE | `/api/meals/{id}` | Public | Delete meal |
| POST | `/api/meals/bulk-insert` | Public | Seed ~60 predefined Bengali meals |

**Create/Update Meal request:**
```json
{
  "name": "Shorshe Ilish",
  "description": "Hilsa fish in mustard sauce",
  "price": 500.00,
  "categoryId": "3fa85f64-...",
  "imageUrl": "https://example.com/image.jpg"
}
```

### Meal Customization Options

Customers can add customizations when placing an order.

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/mealcustomization/meal/{mealId}` | Public | List options for a meal |
| GET | `/api/mealcustomization/{id}` | Public | Get option by ID |
| POST | `/api/mealcustomization` | Admin | Create option |
| PUT | `/api/mealcustomization/{id}` | Admin | Update option |
| DELETE | `/api/mealcustomization/{id}` | Admin | Delete option |
| PATCH | `/api/mealcustomization/{id}/toggle-availability` | Admin | Enable/disable option |

### Ingredients & Units

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET/POST/PUT/DELETE | `/api/ingredients` | Ingredient CRUD |
| POST | `/api/ingredients/bulk-insert` | Seed predefined ingredients |
| GET/POST/PUT/DELETE | `/api/mealingredients` | Meal↔Ingredient associations |
| GET/POST/PUT | `/api/units` | Unit of measurement CRUD (kg, g, l…) |
| POST | `/api/units/bulk-insert` | Seed predefined units |

---

## 7. Addresses

*Requires: any authenticated user*

```
GET    /api/address                     — list my addresses
GET    /api/address/{id}                — get by ID
POST   /api/address                     — create address
PUT    /api/address/{id}                — update address
DELETE /api/address/{id}                — delete address
PATCH  /api/address/{id}/set-default    — set as default delivery address
```

**Create/Update request:**
```json
{
  "label": "Home",
  "addressLine1": "123 Main Street",
  "addressLine2": "Apt 4B",
  "city": "Dhaka",
  "state": "Dhaka Division",
  "postalCode": "1205",
  "country": "Bangladesh",
  "latitude": 23.7985,
  "longitude": 90.3640,
  "isDefault": true
}
```

---

## 8. Orders

### Order Lifecycle

```
Pending ──► Confirmed ──► Preparing ──► ReadyForPickup ──► OutForDelivery ──► Delivered
   │              │
   └──────────────┴──► Cancelled
                                                                        Delivered ──► Refunded
```

Status transitions are enforced — attempting an invalid transition returns `400 Bad Request`.

### Endpoints

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/order/my` | Any auth | My orders (paginated) |
| GET | `/api/order/{id}` | Any auth | Order by ID (customer sees own orders only) |
| GET | `/api/order` | Admin | All orders, optional `?status=` filter |
| POST | `/api/order` | Any auth | Place new order |
| PATCH | `/api/order/{id}/status` | Admin | Advance order through workflow |
| PATCH | `/api/order/{id}/cancel` | Any auth | Cancel order (Pending or Confirmed only) |

### Place Order

```
POST /api/order
```

```json
{
  "addressId": "3fa85f64-...",
  "items": [
    {
      "mealId": "3fa85f64-...",
      "quantity": 2,
      "specialInstructions": "Less spicy",
      "customizationOptionIds": ["3fa85f64-..."]
    }
  ],
  "couponCode": "SAVE20",
  "pointsToRedeem": 500,
  "notes": "Please ring the doorbell"
}
```

**Pricing calculation:**
1. SubTotal = sum of (meal price + customizations) × quantity
2. Loyalty discount = `pointsToRedeem ÷ 100` (capped at SubTotal)
3. Coupon discount applied to amount after loyalty discount
4. Tax = 10% of taxable amount
5. Total = taxable amount + tax

### Update Order Status (Admin)

```
PATCH /api/order/{id}/status
```
```json
{
  "status": 2,
  "cancellationReason": ""
}
```

### Cancel Order

```
PATCH /api/order/{id}/cancel
```
```json
{
  "reason": "Changed my mind"
}
```

---

## 9. Payments

### Payment Flow

```
Order placed (Pending)
     │
     ▼
POST /api/payment/initiate   →  PaymentTransaction created (Pending)
     │
     ▼
PATCH /api/payment/{id}/confirm  →  Transaction becomes Success
                                     Order advances to Confirmed
                                     Loyalty points earned automatically
     │
     ▼  (optional, admin only)
PATCH /api/payment/{id}/refund   →  Transaction becomes Refunded
                                     Order becomes Refunded
```

### Endpoints

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | `/api/payment/initiate` | Any auth | Initiate payment for an order |
| PATCH | `/api/payment/{id}/confirm` | Any auth | Confirm a pending payment |
| PATCH | `/api/payment/{id}/refund` | Admin | Refund a successful payment |
| GET | `/api/payment/order/{orderId}` | Any auth | Get payment for an order |
| GET | `/api/payment/{id}` | Any auth | Get payment by ID |
| GET | `/api/payment` | Admin | All payments, optional `?status=` filter |

### Initiate Payment

```
POST /api/payment/initiate
```
```json
{
  "orderId": "3fa85f64-...",
  "gateway": "cash",
  "notes": "Pay on delivery"
}
```

Common `gateway` values: `cash`, `card`, `bkash`, `nagad`, `stripe`.

### Confirm Payment

```
PATCH /api/payment/{id}/confirm
```
```json
{
  "transactionRef": "TXN-2024-00123",
  "notes": "Confirmed via bKash"
}
```

---

## 10. Delivery

### Delivery Flow

```
Admin creates DeliveryPersonnel profile
         │
         ▼
Admin assigns personnel to order  →  DeliveryAssignment (Assigned)
         │
         ▼
DeliveryPersonnel picks up order  →  Status: PickedUp
         │
         ▼
DeliveryPersonnel delivers order  →  Status: Delivered  (or Failed)
```

### Endpoints

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/delivery/personnel` | Admin | List all delivery personnel (paginated) |
| GET | `/api/delivery/personnel/{id}` | Admin | Get personnel by ID |
| POST | `/api/delivery/personnel` | Admin | Create personnel profile |
| PUT | `/api/delivery/personnel/{id}` | Admin | Update personnel |
| DELETE | `/api/delivery/personnel/{id}` | Admin | Delete personnel |
| PATCH | `/api/delivery/personnel/{id}/toggle-availability` | Admin | Toggle available/unavailable |
| PATCH | `/api/delivery/personnel/{id}/location` | DeliveryPersonnel | Update GPS coordinates |
| POST | `/api/delivery/assign` | Admin | Assign personnel to order |
| PATCH | `/api/delivery/assignments/{id}/status` | Admin or DeliveryPersonnel | Update delivery status |
| GET | `/api/delivery/order/{orderId}` | Any auth | Get assignment for an order |
| GET | `/api/delivery/my-deliveries` | DeliveryPersonnel | My assigned deliveries |

### Create Personnel Profile (Admin)

```
POST /api/delivery/personnel
```
```json
{
  "userId": "identity-user-id",
  "fullName": "Karim Khan",
  "phone": "+8801700000000",
  "vehicleType": "Motorcycle",
  "vehicleNumber": "DHAKA-1234"
}
```

### Assign Delivery (Admin)

```
POST /api/delivery/assign
```
```json
{
  "orderId": "3fa85f64-...",
  "deliveryPersonnelId": "3fa85f64-..."
}
```

### Update Delivery Status

```
PATCH /api/delivery/assignments/{id}/status
```
```json
{
  "status": 2,
  "notes": "Package picked up from kitchen"
}
```

### Update GPS Location (DeliveryPersonnel)

```
PATCH /api/delivery/personnel/{id}/location
```
```json
{
  "latitude": 23.7985,
  "longitude": 90.3640
}
```

---

## 11. Coupons

### Endpoints

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/coupon` | Admin | List coupons (paginated, searchable) |
| GET | `/api/coupon/{id}` | Admin | Get coupon by ID |
| POST | `/api/coupon` | Admin | Create coupon |
| PUT | `/api/coupon/{id}` | Admin | Update coupon |
| DELETE | `/api/coupon/{id}` | Admin | Delete coupon |
| PATCH | `/api/coupon/{id}/toggle` | Admin | Toggle active/inactive |
| POST | `/api/coupon/validate` | Public | Validate coupon against order amount |

### Create Coupon (Admin)

```
POST /api/coupon
```
```json
{
  "code": "SAVE20",
  "description": "20% off on orders above ৳500",
  "discountType": 1,
  "discountValue": 20.0,
  "minOrderAmount": 500.00,
  "maxDiscountAmount": 200.00,
  "usageLimit": 100,
  "expiresAt": "2026-12-31T23:59:59Z",
  "isActive": true,
  "isFirstOrderOnly": false
}
```

- `discountType` 1 = Percentage, 2 = Flat amount
- `maxDiscountAmount` caps the discount for percentage-type coupons

### Validate Coupon (Public)

```
POST /api/coupon/validate
```
```json
{
  "code": "SAVE20",
  "orderAmount": 800.00
}
```

Response includes `isValid`, `discountAmount`, and the full coupon object if valid.

### Applying a Coupon

Pass `"couponCode": "SAVE20"` in the `POST /api/order` request body.

---

## 12. Loyalty Program

### How It Works

| Event | Effect |
|-------|--------|
| Payment confirmed | Earn 1 point per ৳1 spent (automatically) |
| Redeem at order | 100 points = ৳1 discount |
| Admin adjustment | Admin can manually add or deduct points |

### Tier Progression

| Tier | Total Points Earned |
|------|---------------------|
| Bronze | 0 |
| Silver | 1,000 |
| Gold | 5,000 |
| Platinum | 10,000 |

Tier is based on **total points ever earned**, not current balance.

### Endpoints

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/loyalty/my-account` | Any auth | My points, tier, and next-tier progress |
| GET | `/api/loyalty/my-transactions` | Any auth | My point transaction history (paginated) |
| GET | `/api/loyalty/preview-redemption?points=500` | Any auth | Preview discount before placing order |
| GET | `/api/loyalty/account/{userId}` | Admin | View any user's loyalty account |
| POST | `/api/loyalty/adjust` | Admin | Manually adjust points |

### Preview Redemption

```
GET /api/loyalty/preview-redemption?points=500
```

Returns `pointsToRedeem`, `discountAmount`, and `remainingPoints`.

### Redeem Points at Order Placement

Pass `"pointsToRedeem": 500` in the `POST /api/order` request body. The system validates you have enough points and applies the discount automatically.

### Admin Point Adjustment

```
POST /api/loyalty/adjust
```
```json
{
  "userId": "identity-user-id",
  "points": 200,
  "description": "Compensation for late delivery"
}
```

Use negative `points` to deduct.

---

## 13. Reviews

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/mealreview/{id}` | Public | Get review by ID |
| GET | `/api/mealreview/meal/{mealId}` | Public | All reviews for a meal |
| GET | `/api/mealreview/{mealId}/average-rating` | Public | Average rating for a meal |
| POST | `/api/mealreview` | Customer | Submit review |
| PATCH | `/api/mealreview/{id}` | Any auth | Update own review (admin can update any) |
| DELETE | `/api/mealreview/{id}` | Any auth | Delete own review (admin can delete any) |
| GET | `/api/mealreview/my-reviews` | Any auth | My submitted reviews |

### Submit Review

```
POST /api/mealreview
```
```json
{
  "mealId": "3fa85f64-...",
  "userId": "3fa85f64-...",
  "rating": 5,
  "feedback": "Absolutely delicious! Authentic Bengali taste."
}
```

`rating` must be between 1 and 5.

---

## 14. Support Tickets

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | `/api/supportticket` | Public | Create a support ticket |
| GET | `/api/supportticket/{ticketId}` | Public | Get ticket by ID |
| GET | `/api/supportticket/user/{userId}` | Public | Get tickets for a user |
| GET | `/api/supportticket` | Admin | List all tickets |
| PATCH | `/api/supportticket/{ticketId}` | Public | Update ticket status |

### Create Ticket

```
POST /api/supportticket
```
```json
{
  "subject": "Missing item in order",
  "description": "My order #ABC123 was missing the Rasgulla dessert.",
  "priority": 2,
  "mobileNo": "+8801700000000",
  "departmentId": "3fa85f64-...",
  "categoryTypeId": "3fa85f64-..."
}
```

`priority`: 1=Low, 2=Medium, 3=High, 4=Urgent

---

## 15. Notifications

### REST Endpoints

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/notification` | Any auth | My notifications (paginated) |
| GET | `/api/notification/unread-count` | Any auth | Count of unread notifications |
| PATCH | `/api/notification/{id}/read` | Any auth | Mark one notification as read |
| PATCH | `/api/notification/read-all` | Any auth | Mark all notifications as read |
| DELETE | `/api/notification/{id}` | Any auth | Delete a notification |

### Automatic Notifications

The system automatically creates notifications for:
- Order placed
- Order status updated
- Payment confirmed

---

## 16. User Profile

*Requires: any authenticated user*

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/userprofile` | Get my profile (name, email, phone, avatar, roles) |
| PUT | `/api/userprofile` | Update name, date of birth, phone number |
| POST | `/api/userprofile/change-password` | Change password |
| POST | `/api/userprofile/avatar` | Upload/replace avatar image |
| GET | `/api/userprofile/order-history` | My order history (paginated) |

### Update Profile

```
PUT /api/userprofile
```
```json
{
  "firstName": "Jane",
  "lastName": "Smith",
  "dateOfBirth": "1995-06-15",
  "phoneNumber": "+8801700000000"
}
```

### Change Password

```
POST /api/userprofile/change-password
```
```json
{
  "currentPassword": "OldPass@123",
  "newPassword": "NewPass@456"
}
```

Password rules: min 8 characters, at least one uppercase, one lowercase, one digit, one special character.

### Upload Avatar

```
POST /api/userprofile/avatar
Content-Type: multipart/form-data
```

Send the image file as form-data with key `file`. Returns the new `profileImageUrl`. The previous avatar is deleted from cloud storage automatically.

---

## 17. Admin Features

### Analytics Dashboard

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/analytics/dashboard` | Full dashboard: order KPIs, revenue, status breakdown, top meals, top customers, loyalty/inventory summary |
| GET | `/api/analytics/daily-revenue?days=30` | Daily revenue and order count for the last N days |
| GET | `/api/analytics/top-meals?top=10` | Top N meals by quantity ordered |
| GET | `/api/analytics/top-customers?top=10` | Top N customers by total spend |

### User Management

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/admin/users` | All users (paginated, searchable) |
| GET | `/api/admin/users/{userId}` | User by ID |
| POST | `/api/admin/users/{userId}/ban` | Ban (lock) user account |
| POST | `/api/admin/users/{userId}/unban` | Unban (unlock) user account |
| POST | `/api/admin/users/assign-role` | Assign role to user |
| POST | `/api/admin/users/remove-role` | Remove role from user |

**Assign Role:**
```json
{ "userId": "identity-user-id", "role": "DeliveryPersonnel" }
```

### Inventory Management

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/inventory` | Inventory items (paginated, searchable) |
| POST | `/api/inventory` | Add item |
| PATCH | `/api/inventory/{id}` | Update stock count or price |
| DELETE | `/api/inventory/{id}` | Remove item (logs a Deletion transaction) |
| POST | `/api/inventory/bulk-insert` | Seed ~40 predefined kitchen inventory items |

**Add Inventory Item:**
```json
{
  "name": "Mustard Oil",
  "stockCount": 100,
  "price": 2.49
}
```

Updating `stockCount` automatically creates an `InventoryTransaction` record (Restock or OrderUse) for audit trail.

---

## 18. Real-Time (SignalR)

### Connect

```
wss://localhost:{port}/hubs/notifications?access_token=<jwt>
```

Pass the JWT access token as a query parameter. The hub uses it for authentication.

### Receiving Notifications

Listen on the `ReceiveNotification` event:

```javascript
// JavaScript example
const connection = new signalR.HubConnectionBuilder()
  .withUrl("/hubs/notifications", {
    accessTokenFactory: () => localStorage.getItem("accessToken")
  })
  .build();

connection.on("ReceiveNotification", (notification) => {
  console.log(notification.title, notification.message);
});

await connection.start();
```

### Notification Payload

```json
{
  "id": "3fa85f64-...",
  "userId": "...",
  "title": "Order Placed",
  "message": "Your order #ABC12345 has been placed.",
  "type": 1,
  "typeLabel": "OrderStatus",
  "isRead": false,
  "referenceId": "3fa85f64-...",
  "referenceType": "Order",
  "createdAt": "2026-05-13T10:30:00Z"
}
```

---

## 19. Error Reference

| HTTP Status | Meaning |
|-------------|---------|
| 200 | Success |
| 201 | Created |
| 204 | No Content |
| 400 | Bad Request — validation failed or invalid operation |
| 401 | Unauthorized — missing or invalid token |
| 403 | Forbidden — authenticated but lacks required role |
| 404 | Not Found |
| 409 | Conflict — duplicate resource (e.g., duplicate coupon code) |
| 422 | Validation Error — request body failed validation rules |
| 429 | Too Many Requests |
| 500 | Internal Server Error |

On error, `success` is `false` and `errors` contains descriptive messages:

```json
{
  "success": false,
  "message": "Validation failed",
  "data": null,
  "errors": [
    "At least one item is required.",
    "Points to redeem cannot be negative."
  ]
}
```
