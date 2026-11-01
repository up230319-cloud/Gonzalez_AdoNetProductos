-- Ejecutar en SQL Server antes de iniciar la API.
USE master;
GO

IF DB_ID(N'ProductosDB') IS NULL
BEGIN
    CREATE DATABASE ProductosDB;
END;
GO

USE ProductosDB;
GO

IF OBJECT_ID(N'dbo.Productos', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Productos
    (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Nombre NVARCHAR(100) NOT NULL CHECK (LEN(LTRIM(RTRIM(Nombre))) > 0),
        Descripcion NVARCHAR(255) NOT NULL,
        Precio DECIMAL(10,2) NOT NULL CHECK (Precio > 0),
        Existencia INT NOT NULL CHECK (Existencia >= 0)
    );
END;
GO

-- La tabla puede estar vacía. Agrega tu primer producto desde Swagger.
SELECT Id, Nombre, Descripcion, Precio, Existencia FROM dbo.Productos;
GO

