-- 02_create_tables.sql
USE TeamTasksSample;
GO

-- Tabla Developers
IF NOT EXISTS (SELECT * FROM sys.tables t 
               JOIN sys.schemas s ON t.schema_id = s.schema_id 
               WHERE t.name = 'Developers' AND s.name = 'app')
BEGIN
    CREATE TABLE app.Developers (
        DeveloperId INT PRIMARY KEY IDENTITY(1,1),
        FirstName NVARCHAR(100) NOT NULL,
        LastName NVARCHAR(100) NOT NULL,
        Email NVARCHAR(255) NOT NULL,
        IsActive BIT DEFAULT 1,
        CreatedAt DATETIME DEFAULT GETDATE()
    );
END
GO

-- Tabla Projects
IF NOT EXISTS (SELECT * FROM sys.tables t 
               JOIN sys.schemas s ON t.schema_id = s.schema_id 
               WHERE t.name = 'Projects' AND s.name = 'app')
BEGIN
    CREATE TABLE app.Projects (
        ProjectId INT PRIMARY KEY IDENTITY(1,1),
        Name NVARCHAR(200) NOT NULL,
        ClientName NVARCHAR(200) NOT NULL,
        StartDate DATE,
        EndDate DATE,
        Status NVARCHAR(50) CHECK (Status IN ('Planned', 'InProgress', 'Completed')) DEFAULT 'Planned',
        CreatedAt DATETIME DEFAULT GETDATE(),
        UpdatedAt DATETIME DEFAULT GETDATE()
    );
END
GO

-- Tabla Tasks
IF NOT EXISTS (SELECT * FROM sys.tables t 
               JOIN sys.schemas s ON t.schema_id = s.schema_id 
               WHERE t.name = 'Tasks' AND s.name = 'app')
BEGIN
    CREATE TABLE app.Tasks (
        TaskId INT PRIMARY KEY IDENTITY(1,1),
        ProjectId INT NOT NULL,
        Title NVARCHAR(200) NOT NULL,
        Description NVARCHAR(MAX),
        AssigneeId INT NOT NULL,
        Status NVARCHAR(50) CHECK (Status IN ('ToDo', 'InProgress', 'Blocked', 'Completed')) DEFAULT 'ToDo',
        Priority NVARCHAR(50) CHECK (Priority IN ('Low', 'Medium', 'High')) DEFAULT 'Medium',
        EstimatedComplexity INT CHECK (EstimatedComplexity BETWEEN 1 AND 5),
        DueDate DATE,
        CompletionDate DATE,
        CreatedAt DATETIME DEFAULT GETDATE(),
        UpdatedAt DATETIME DEFAULT GETDATE(),
        CONSTRAINT FK_Tasks_Projects FOREIGN KEY (ProjectId) REFERENCES app.Projects(ProjectId),
        CONSTRAINT FK_Tasks_Developers FOREIGN KEY (AssigneeId) REFERENCES app.Developers(DeveloperId)
    );
END
GO