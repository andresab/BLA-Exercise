IF DB_ID(N'BookCatalogDb') IS NULL
BEGIN
    CREATE DATABASE BookCatalogDb;
END
GO

USE BookCatalogDb;
GO

IF OBJECT_ID(N'dbo.Users', N'U') IS NULL
BEGIN
    CREATE TABLE Users (
        Id           UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        Username     NVARCHAR(100)    NOT NULL UNIQUE,
        Email        NVARCHAR(200)    NOT NULL UNIQUE,
        PasswordHash NVARCHAR(MAX)    NOT NULL,
        CreatedAt    DATETIME2        NOT NULL DEFAULT GETUTCDATE()
    );
END
GO

IF OBJECT_ID(N'dbo.Books', N'U') IS NULL
BEGIN
    CREATE TABLE Books (
        Id            UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        Title         NVARCHAR(200)   NOT NULL,
        Author        NVARCHAR(150)   NOT NULL,
        ISBN          NVARCHAR(20)    NOT NULL UNIQUE,
        PublishedYear INT             NOT NULL,
        CreatedAt     DATETIME2       NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt     DATETIME2       NOT NULL DEFAULT GETUTCDATE()
    );
END
GO
