-- 05_queries.sql
USE TeamTasksSample;
GO

-- 1. Resumen de carga por desarrollador
SELECT 
    CONCAT(d.FirstName, ' ', d.LastName) AS DeveloperName,
    COUNT(t.TaskId) AS OpenTasksCount,
    AVG(CAST(t.EstimatedComplexity AS DECIMAL(5,2))) AS AverageEstimatedComplexity
FROM app.Developers d
LEFT JOIN app.Tasks t ON d.DeveloperId = t.AssigneeId 
    AND t.Status <> 'Completed'
WHERE d.IsActive = 1
GROUP BY d.DeveloperId, d.FirstName, d.LastName
ORDER BY DeveloperName;
GO

-- 2. Resumen de estado por proyecto
SELECT 
    p.Name AS ProjectName,
    COUNT(t.TaskId) AS TotalTasks,
    SUM(CASE WHEN t.Status <> 'Completed' THEN 1 ELSE 0 END) AS OpenTasks,
    SUM(CASE WHEN t.Status = 'Completed' THEN 1 ELSE 0 END) AS CompletedTasks
FROM app.Projects p
LEFT JOIN app.Tasks t ON p.ProjectId = t.ProjectId
GROUP BY p.ProjectId, p.Name
ORDER BY p.Name;
GO

-- 3. Tareas próximas a vencer (próximos 7 días)
SELECT 
    t.Title,
    p.Name AS ProjectName,
    CONCAT(d.FirstName, ' ', d.LastName) AS AssignedTo,
    t.Status,
    t.Priority,
    t.DueDate,
    DATEDIFF(DAY, GETDATE(), t.DueDate) AS DaysUntilDue
FROM app.Tasks t
JOIN app.Projects p ON t.ProjectId = p.ProjectId
JOIN app.Developers d ON t.AssigneeId = d.DeveloperId
WHERE t.Status <> 'Completed'
    AND t.DueDate IS NOT NULL
    AND t.DueDate BETWEEN GETDATE() AND DATEADD(DAY, 7, GETDATE())
ORDER BY t.DueDate;
GO

-- 4. Developer Delay Risk Prediction
WITH DeveloperCompletedTasks AS (
    SELECT 
        t.AssigneeId,
        AVG(CASE 
            WHEN t.CompletionDate > t.DueDate 
            THEN DATEDIFF(DAY, t.DueDate, t.CompletionDate)
            ELSE 0 
        END) AS AvgDelayDays
    FROM app.Tasks t
    WHERE t.Status = 'Completed'
        AND t.CompletionDate IS NOT NULL
        AND t.DueDate IS NOT NULL
    GROUP BY t.AssigneeId
),
DeveloperOpenTasks AS (
    SELECT 
        t.AssigneeId,
        COUNT(*) AS OpenTasksCount,
        MIN(t.DueDate) AS NearestDueDate,
        MAX(t.DueDate) AS LatestDueDate
    FROM app.Tasks t
    WHERE t.Status <> 'Completed'
        AND t.DueDate IS NOT NULL
    GROUP BY t.AssigneeId
)
SELECT 
    CONCAT(d.FirstName, ' ', d.LastName) AS DeveloperName,
    ISNULL(dot.OpenTasksCount, 0) AS OpenTasksCount,
    ISNULL(dct.AvgDelayDays, 0) AS AvgDelayDays,
    dot.NearestDueDate,
    dot.LatestDueDate,
    CASE 
        WHEN dot.LatestDueDate IS NOT NULL
        THEN DATEADD(DAY, ISNULL(dct.AvgDelayDays, 0), dot.LatestDueDate)
        ELSE NULL 
    END AS PredictedCompletionDate,
    CASE 
        WHEN (ISNULL(dct.AvgDelayDays, 0) > 3) 
            OR (DATEADD(DAY, ISNULL(dct.AvgDelayDays, 0), ISNULL(dot.LatestDueDate, GETDATE())) > ISNULL(dot.LatestDueDate, GETDATE()))
        THEN 1 
        ELSE 0 
    END AS HighRiskFlag
FROM app.Developers d
LEFT JOIN DeveloperCompletedTasks dct ON d.DeveloperId = dct.AssigneeId
LEFT JOIN DeveloperOpenTasks dot ON d.DeveloperId = dot.AssigneeId
WHERE d.IsActive = 1
ORDER BY HighRiskFlag DESC, OpenTasksCount DESC;
GO

SELECT 'Consultas ejecutadas exitosamente.' AS Mensaje;
GO