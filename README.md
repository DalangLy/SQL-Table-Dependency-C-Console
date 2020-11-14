# SQL-Table-Dependency-C-Console
the use to get notification from sql server when data change
# Setup
- install SQLTableDependency Nuget
- Create Customer Table in sql server
- enable broker in sql server by slq script or ui up to u
- if u get permission error u have to change your sql server to own that database manually in SQL Server -> Security -> your user login -> Property -> User Mapping or use sql command "EXEC sp_changedbowner 'sa', 'true'" to your database
