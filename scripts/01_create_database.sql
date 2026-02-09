-- 01_create_database.sql
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'TeamTasksSample')
BEGIN
    CREATE DATABASE TeamTasksSample;
END
GO

USE TeamTasksSample;
GO

IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'app')
BEGIN
    EXEC('CREATE SCHEMA app');
END
GO