# ItPos Backend

```
dotnet ef migrations add Initial -c ApplicationDbContext -p ./Notebook.Infrastructure -s ./Notebook.Api -o Persistence/Migrations
```

```
dotnet publish --os linux --arch x64 -c Release -p:PublishProfile=DefaultContainer
docker run -it --rm -p 5000:5000 notebook-api:latest
```

Execute in main solution folder