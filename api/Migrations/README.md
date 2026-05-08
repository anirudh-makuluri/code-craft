# EF Core migrations

This repo was upgraded to a normalized schema (Users, Crafts, CraftLikes, CraftViews).

Run these commands on a machine with .NET 8 SDK installed:

```bash
cd api
dotnet tool install --global dotnet-ef

dotnet ef migrations add InitialNormalizedSchema

dotnet ef database update
```

If your SQL connection is environment-driven, set `CONNECTION_STRING` before running `database update`.
