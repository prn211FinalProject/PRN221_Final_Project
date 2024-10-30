-- Tạo cơ sở dữ liệu LibraryManagement
CREATE DATABASE LMS_PRN221;
--DROP DATABASE LibraryManagement;

-- Sử dụng cơ sở dữ liệu LibraryManagement
USE LMS_PRN221;
Go

-- Bảng Role (vai trò của user)
CREATE TABLE Role (
    Id INT PRIMARY KEY IDENTITY,
    Name NVARCHAR(255),
    Status BIT
);

-- Bảng User (người dùng)
CREATE TABLE [User] (
    Uid INT PRIMARY KEY IDENTITY,
    FullName NVARCHAR(255),
    Email NVARCHAR(255) UNIQUE,
    Phone NVARCHAR(20),
    Status BIT,
    RoleId INT FOREIGN KEY REFERENCES Role(Id)
);

-- Bảng Category (thể loại sách)
CREATE TABLE Category (
    Id INT PRIMARY KEY IDENTITY,
    CName NVARCHAR(255),
    Status BIT
);

-- Bảng Publisher (nhà xuất bản)
CREATE TABLE Publisher (
    Id INT PRIMARY KEY IDENTITY,
    PName NVARCHAR(255),
    Address NVARCHAR(255),
    Phone NVARCHAR(20),
    Email NVARCHAR(255),
    Status BIT
);

-- Bảng Book (sách)
CREATE TABLE Book (
    Id INT PRIMARY KEY IDENTITY,
    CId INT FOREIGN KEY REFERENCES Category(Id),
    BName NVARCHAR(255),
    Author NVARCHAR(255),
    Quantity INT,
    Image NVARCHAR(255),
    Price DECIMAL(10, 2),
    Status BIT,
    PublisherId INT FOREIGN KEY REFERENCES Publisher(Id),
    PublishDate DATETIME
);

-- Bảng Feedback (đánh giá sách)
CREATE TABLE Feedback (
    Id INT PRIMARY KEY IDENTITY,
    Bid INT FOREIGN KEY REFERENCES Book(Id),
    Uid INT FOREIGN KEY REFERENCES [User](Uid),
    Content NVARCHAR(MAX),
    CreatedDate DATETIME,
    Status BIT
);

-- Bảng Wishlist (danh sách yêu thích của người dùng)
CREATE TABLE Wishlist (
    Id INT PRIMARY KEY IDENTITY,
    Uid INT FOREIGN KEY REFERENCES [User](Uid)
);

-- Bảng WishlistItem (mục trong danh sách yêu thích)
CREATE TABLE WishlistItem (
    WId INT FOREIGN KEY REFERENCES Wishlist(Id),
    BId INT FOREIGN KEY REFERENCES Book(Id),
    PRIMARY KEY (WId, BId)
);

-- Bảng Borrow Information (thông tin mượn sách)
CREATE TABLE BorrowInformation (
    Oid INT PRIMARY KEY IDENTITY,
    Uid INT FOREIGN KEY REFERENCES [User](Uid),
    BorrowDate DATETIME,
    DueDate DATETIME,
    TotalAmount DECIMAL(10, 2),
    Note NVARCHAR(MAX),
    Status BIT
);

-- Bảng Borrow Detail (chi tiết mượn sách)
CREATE TABLE BorrowDetail (
    Oid INT FOREIGN KEY REFERENCES BorrowInformation(Oid),
    Bid INT FOREIGN KEY REFERENCES Book(Id),
    PRIMARY KEY (Oid, Bid)
);

-- Bảng Blog (blog về sách hoặc hệ thống)
CREATE TABLE Blog (
    Bid INT PRIMARY KEY IDENTITY,
    Title NVARCHAR(255),
    Content NVARCHAR(MAX),
    CreatedDate DATETIME,
    Status BIT,
	Uid INT FOREIGN KEY REFERENCES [User](Uid)
);