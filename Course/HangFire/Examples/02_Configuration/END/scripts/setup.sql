USE master;
GO

IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = '$(MSSQL_DB)')
BEGIN
    CREATE DATABASE $(MSSQL_DB);
END
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='MyTable' and xtype='U')
BEGIN
    CREATE TABLE MyTable (
        Id INT PRIMARY KEY,
        Name VARCHAR(50)
    );
END
GO

IF NOT EXISTS (SELECT * FROM sys.sql_logins WHERE name = '$(MSSQL_USER)')
BEGIN
    CREATE LOGIN $(MSSQL_USER) WITH PASSWORD = '$(MSSQL_PASSWORD)';
END
GO

USE $(MSSQL_DB);
GO

IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = '$(MSSQL_USER)')
BEGIN
    CREATE USER $(MSSQL_USER) FOR LOGIN $(MSSQL_USER);
END
GO

IF NOT EXISTS (SELECT * FROM sys.database_role_members WHERE member_principal_id = USER_ID('$(MSSQL_USER)') AND role_principal_id = DATABASE_PRINCIPAL_ID('db_owner'))
BEGIN
    ALTER ROLE db_owner ADD MEMBER $(MSSQL_USER);
END
GO
