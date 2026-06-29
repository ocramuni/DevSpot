# Installation

1. Install .NET 8 SDK and ASP.NET Core Runtime
2. Install Entity Framework Core tools
```shell
dotnet tool install --version 8.0.28 -g dotnet-ef
```
3. Create SQLite database
```shell
dotnet ef datbase update
```
4. Build and run application
```shell
dotnet run
```