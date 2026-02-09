-- 03_insert_data.sql
USE TeamTasksSample;

DELETE FROM app.Tasks;
DELETE FROM app.Projects;
DELETE FROM app.Developers;

-- Resetear identidades
DBCC CHECKIDENT ('app.Tasks', RESEED, 0);
DBCC CHECKIDENT ('app.Projects', RESEED, 0);
DBCC CHECKIDENT ('app.Developers', RESEED, 0);

-- Insertar proyectos
INSERT INTO app.Projects (Name, ClientName, StartDate, EndDate, Status) VALUES
('Sistema de Inventarios', 'BTW', '2026-01-01', '2026-12-31', 'InProgress'),
('Portal Cliente', 'BTW', '2026-03-01', '2026-10-30', 'Planned'),
('App Movil', 'BTW', '2026-03-01', '2026-08-31', 'Planned');

-- Insertar desarrolladores
INSERT INTO app.Developers (FirstName, LastName, Email, IsActive) VALUES
('Diana', 'Marin', 'dia.marin@example.com', 1),
('Martin', 'Martinez', 'mar.martinez@example.com', 1),
('Amanda', 'Triana', 'ama.triana@example.com', 1),
('Paula', 'Meneses', 'pau.meneses@example.com', 1),
('Wendy', 'Villareal', 'wen.villareal@example.com', 1);

-- Insertar tareas
INSERT INTO app.Tasks (ProjectId, Title, Description, AssigneeId, Status, Priority, EstimatedComplexity, DueDate, CompletionDate) VALUES
(1, 'Diseño de base de datos', 'Diseñar el esquema relacional para el sistema de gestión, incluyendo tablas, relaciones y restricciones.', 1, 'Completed', 'High', 4, '2026-02-01', '2026-01-28'),
(1, 'Desarrollo API REST', 'Implementar endpoints CRUD para gestión de proyectos con autenticación JWT.', 2, 'InProgress', 'High', 5, '2026-06-15', NULL),
(1, 'Pruebas unitarias módulo tareas', 'Escribir pruebas unitarias para el servicio de gestión de tareas con cobertura >80%.', 3, 'ToDo', 'Medium', 3, '2026-07-01', NULL),
(1, 'Documentación arquitectura', 'Crear documentación técnica describiendo la arquitectura en capas y patrones utilizados.', 4, 'Blocked', 'Low', 2, '2026-05-20', NULL),
(2, 'Maquetación UI/UX', 'Diseñar interfaces de usuario para el portal cliente usando Figma, siguiendo guías de estilo.', 5, 'Completed', 'Medium', 3, '2026-03-15', '2026-03-25'),
(2, 'Sistema de autenticación', 'Implementar login, registro y recuperación de contraseña con validaciones de seguridad.', 1, 'InProgress', 'High', 4, '2026-01-29', NULL),
(2, 'Panel de administración', 'Desarrollar dashboard administrativo con métricas y gestión de usuarios.', 2, 'ToDo', 'Medium', 3, '2026-08-15', NULL),
(3, 'Prototipo navegable', 'Crear prototipo funcional en React Native para validación con el cliente.', 3, 'Completed', 'Low', 2, '2026-01-31', '2026-01-25'),
(3, 'Despliegue producción', 'Configurar servidor, dominio SSL y desplegar aplicación móvil en stores.', 4, 'Completed', 'Medium', 3, '2026-04-10', '2026-04-05'),
(3, 'Recolección feedback inicial', 'Realizar entrevistas con usuarios beta para recoger sugerencias de mejora.', 5, 'Completed', 'Low', 1, '2026-04-20', '2026-04-18'),
(1, 'Auditoría de seguridad', 'Revisar código y configuraciones en busca de vulnerabilidades OWASP Top 10.', 1, 'ToDo', 'High', 5, '2026-08-31', NULL),
(2, 'Optimización rendimiento frontend', 'Implementar lazy loading, code splitting y optimizar bundles.', 2, 'InProgress', 'Medium', 3, '2026-07-10', NULL),
(3, 'Soporte post-lanzamiento', 'Atender incidencias reportadas por usuarios en producción.', 3, 'Blocked', 'Low', 2, '2026-09-01', NULL),
(1, 'Integración CI/CD', 'Configurar pipeline en Azure DevOps con build, test y despliegue automático.', 4, 'ToDo', 'High', 4, '2026-09-15', NULL),
(2, 'Pruebas end-to-end', 'Implementar pruebas E2E con Cypress para flujos críticos del portal.', 5, 'ToDo', 'Medium', 3, '2026-08-01', NULL),
(3, 'Actualización manual de usuario', 'Revisar y actualizar documentación según cambios en la última versión.', 1, 'Completed', 'Low', 2, '2026-05-05', '2026-05-01'),
(1, 'Módulo de reportes PDF', 'Generar reportes ejecutivos en PDF con gráficos y estadísticas.', 2, 'InProgress', 'High', 5, '2026-09-30', NULL),
(2, 'Internacionalización i18n', 'Traducir aplicación a inglés, francés y alemán con soporte RTL.', 3, 'ToDo', 'Medium', 3, '2026-10-01', NULL),
(3, 'Sistema de backup automático', 'Configurar backups diarios de base de datos con retención de 30 días.', 4, 'Completed', 'Low', 1, '2026-03-01', '2026-02-25'),
(1, 'Monitoreo y alertas', 'Configurar New Relic para monitoreo y alertas de errores en producción.', 5, 'ToDo', 'Medium', 3, '2026-10-15', NULL);