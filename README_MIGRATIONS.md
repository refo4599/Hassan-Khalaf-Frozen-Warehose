EF Core Migrations with Onion Architecture

Prerequisites:
- .NET SDK installed
- SQL Server running and accessible via the connection string in `Frozen-Warehouse.API/appsettings.json`

1) Ensure Infrastructure project has EF Core design and tools packages (done).
2) Ensure `DesignTimeDbContextFactory` is present in `Frozen-Warehouse.Infrastructure` pointing to API appsettings if available (done).
3) From solution root run the migrations command pointing to Infrastructure project and specifying StartupProject as API project.

PowerShell (Package Manager Console) commands:

PM> Add-Migration Init -Project .\Frozen-Warehouse.Infrastructure -StartupProject .\Frozen-Warehouse.API
PM> Update-Database -Project .\Frozen-Warehouse.Infrastructure -StartupProject .\Frozen-Warehouse.API

dotnet CLI commands:

> dotnet ef migrations add Init --project "./Frozen-Warehouse.Infrastructure" --startup-project "./Frozen-Warehouse.API"
> dotnet ef database update --project "./Frozen-Warehouse.Infrastructure" --startup-project "./Frozen-Warehouse.API"

If you get errors about the connection string, ensure `Frozen-Warehouse.API/appsettings.json` contains the `ConnectionStrings:DefaultConnection` entry. The `DesignTimeDbContextFactory` will also accept environment variable `ConnectionStrings__DefaultConnection`.
