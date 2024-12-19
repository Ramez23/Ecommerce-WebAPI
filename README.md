E-Commerce Web API
This is a .NET Web API project designed for an e-commerce platform, allowing users and administrators to interact with the system. The project includes roles-based functionality with distinct permissions for admins and users.

Features
User Roles
Admin:
Can add, update, and delete categories and products.
Manages orders and product inventory.
User:
Can browse categories and products.
Can add products to the cart and place orders.
Core Functionalities
Categories Management: Admin-only feature to add, update, or delete product categories.
Products Management: Admin-only feature to create, update, or delete products.
Cart Management: Allows users to add products to their cart and manage quantities.
Order Management: Users can place orders based on their cart contents.
Payment Processing: Simulates payment workflows with associated DTOs for creating and completing checkout sessions.
Project Structure
The project is organized as follows:

Controllers:

CartProductsController.cs: Handles interactions with the cart products.
CartsController.cs: Manages the cart and associated operations.
CategoryController.cs: Admin functionality for managing product categories.
OrderController.cs: Handles orders and order processing.
PaymentController.cs: Manages payment-related operations.
ProductController.cs: Admin functionality for managing products.
UsersController.cs: Handles user-related operations like authentication and role management.
Data:

ApplicationDbContext.cs: Contains the Entity Framework Core database context for the application.
DTOs (Data Transfer Objects):

Facilitates communication between the API and client, including objects like AddToCartDTO, CategoryDTO, ProductDTO, OrderDetailDTO, etc.
Models:

Includes the database models such as Category, Product, Cart, Order, and User.
Migrations:

Stores database migration files for managing schema changes.
Technologies Used
ASP.NET Core 8.0: Framework for building the Web API.
Entity Framework Core: ORM for database operations.
SQL Server: Relational database for data persistence.
JWT Authentication: For secure role-based access control.



API Documentation
The API includes the following endpoints:

Authentication
POST /api/Users/Login - Authenticate users and generate JWT tokens.
POST /api/Users/Register - Register a new user.
Categories (Admin Only)
GET /api/Category - Retrieve all categories.
POST /api/Category - Add a new category.
PUT /api/Category/{id} - Update an existing category.
DELETE /api/Category/{id} - Delete a category.
Products (Admin Only)
GET /api/Product - Retrieve all products.
POST /api/Product - Add a new product.
PUT /api/Product/{id} - Update an existing product.
DELETE /api/Product/{id} - Delete a product.
Cart
POST /api/Cart/Add - Add a product to the cart.
PUT /api/Cart/Update - Update product quantity in the cart.
DELETE /api/Cart/Delete - Remove a product from the cart.
Orders
POST /api/Order/Create - Place a new order.
GET /api/Order/{userId} - Retrieve user orders.
Payments
POST /api/Payment/CreateCheckoutSession - Create a payment session.
POST /api/Payment/Success - Confirm a successful payment.
