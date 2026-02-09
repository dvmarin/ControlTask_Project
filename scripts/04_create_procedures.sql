-- 04_create_procedures.sql
USE TeamTasksSample;
GO

-- Eliminar procedimiento si existe
IF OBJECT_ID('app.sp_InsertTask', 'P') IS NOT NULL
    DROP PROCEDURE app.sp_InsertTask;
GO

-- Crear procedimiento para insertar tareas
CREATE PROCEDURE app.sp_InsertTask
    @ProjectId INT,
    @Title NVARCHAR(200),
    @Description NVARCHAR(MAX) = NULL,
    @AssigneeId INT,
    @Status NVARCHAR(50) = 'ToDo',
    @Priority NVARCHAR(50) = 'Medium',
    @EstimatedComplexity INT = NULL,
    @DueDate DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @ErrorMessage NVARCHAR(500);
    
    -- Validar ProjectId existe
    IF NOT EXISTS (SELECT 1 FROM app.Projects WHERE ProjectId = @ProjectId)
    BEGIN
        SET @ErrorMessage = 'Error: ProjectId ' + CAST(@ProjectId AS NVARCHAR(10)) + ' no existe.';
        THROW 50001, @ErrorMessage, 1;
        RETURN;
    END
    
    -- Validar AssigneeId existe y está activo
    IF NOT EXISTS (SELECT 1 FROM app.Developers WHERE DeveloperId = @AssigneeId AND IsActive = 1)
    BEGIN
        SET @ErrorMessage = 'Error: AssigneeId ' + CAST(@AssigneeId AS NVARCHAR(10)) + ' no existe o no está activo.';
        THROW 50002, @ErrorMessage, 1;
        RETURN;
    END
    
    -- Validar Status válido
    IF @Status NOT IN ('ToDo', 'InProgress', 'Blocked', 'Completed')
    BEGIN
        SET @ErrorMessage = 'Error: Status debe ser ToDo, InProgress, Blocked o Completed.';
        THROW 50003, @ErrorMessage, 1;
        RETURN;
    END
    
    -- Validar Priority válido
    IF @Priority NOT IN ('Low', 'Medium', 'High')
    BEGIN
        SET @ErrorMessage = 'Error: Priority debe ser Low, Medium o High.';
        THROW 50004, @ErrorMessage, 1;
        RETURN;
    END
    
    -- Validar EstimatedComplexity
    IF @EstimatedComplexity IS NOT NULL AND (@EstimatedComplexity < 1 OR @EstimatedComplexity > 5)
    BEGIN
        SET @ErrorMessage = 'Error: EstimatedComplexity debe estar entre 1 y 5.';
        THROW 50005, @ErrorMessage, 1;
        RETURN;
    END
    
    -- Insertar la tarea
    INSERT INTO app.Tasks (
        ProjectId, Title, Description, AssigneeId, 
        Status, Priority, EstimatedComplexity, DueDate
    ) VALUES (
        @ProjectId, @Title, @Description, @AssigneeId,
        @Status, @Priority, @EstimatedComplexity, @DueDate
    );
    
    -- Devolver la tarea insertada
    SELECT 
        TaskId, ProjectId, Title, Description, AssigneeId,
        Status, Priority, EstimatedComplexity, DueDate,
        CreatedAt
    FROM app.Tasks 
    WHERE TaskId = SCOPE_IDENTITY();
END
GO

SELECT 'Procedimiento sp_InsertTask creado exitosamente.' AS Mensaje;
GO