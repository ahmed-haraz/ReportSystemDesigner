-- Sample database setup for ReportSystem

-- SQLite compatible
CREATE TABLE IF NOT EXISTS Sales (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    ProductName TEXT NOT NULL,
    Quantity INTEGER NOT NULL DEFAULT 0,
    UnitPrice REAL NOT NULL DEFAULT 0.0,
    Total REAL NOT NULL DEFAULT 0.0,
    SaleDate TEXT NOT NULL,
    Salesperson TEXT NOT NULL,
    Category TEXT
);

CREATE TABLE IF NOT EXISTS InvoiceItems (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    InvoiceId INTEGER NOT NULL,
    Description TEXT NOT NULL,
    Quantity INTEGER NOT NULL DEFAULT 0,
    UnitPrice REAL NOT NULL DEFAULT 0.0,
    LineTotal REAL NOT NULL DEFAULT 0.0
);

CREATE TABLE IF NOT EXISTS Products (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    Category TEXT,
    Price REAL NOT NULL DEFAULT 0.0,
    StockQuantity INTEGER NOT NULL DEFAULT 0
);

CREATE TABLE IF NOT EXISTS Customers (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    Email TEXT,
    Phone TEXT,
    Address TEXT
);

-- Sample data
INSERT INTO Sales (ProductName, Quantity, UnitPrice, Total, SaleDate, Salesperson, Category) VALUES
('Laptop Pro X1', 2, 1299.99, 2599.98, '2026-01-15', 'John Smith', 'Electronics'),
('Wireless Mouse', 5, 29.99, 149.95, '2026-01-15', 'John Smith', 'Accessories'),
('USB-C Hub', 3, 79.99, 239.97, '2026-01-16', 'Jane Doe', 'Accessories'),
('Monitor 27" 4K', 1, 499.99, 499.99, '2026-01-17', 'John Smith', 'Electronics'),
('Mechanical Keyboard', 2, 149.99, 299.98, '2026-01-18', 'Jane Doe', 'Accessories');

INSERT INTO Products (Name, Category, Price, StockQuantity) VALUES
('Laptop Pro X1', 'Electronics', 1299.99, 50),
('Wireless Mouse', 'Accessories', 29.99, 200),
('USB-C Hub', 'Accessories', 79.99, 100),
('Monitor 27" 4K', 'Electronics', 499.99, 30),
('Mechanical Keyboard', 'Accessories', 149.99, 75);

INSERT INTO Customers (Name, Email, Phone, Address) VALUES
('Acme Corporation', 'contact@acme.com', '555-0100', '123 Business Ave'),
('TechStart Inc', 'info@techstart.io', '555-0200', '456 Innovation Dr'),
('Global Solutions', 'hello@globalsol.com', '555-0300', '789 Enterprise St');
