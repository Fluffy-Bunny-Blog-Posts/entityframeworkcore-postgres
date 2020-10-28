# entityframeworkcore-postgres

## Install and run Postgress
[deploy-postgresql-using-docker-compose](https://www.osradar.com/deploy-postgresql-using-docker-compose/)   

```
docker volume create --name data-postgresql --driver local
docker-compose -f docker-compose.yml up
```
### docker-compose
```
version: '3.8'

services:
  db:
    image: postgres
    restart: always
    environment:
      POSTGRES_PASSWORD: "angelo123"
    ports:
     - "5432:5432"
    volumes:
     - data-postgresql:/var/lib/postgresql/data
    networks:
     - postgresql
 
  pgadmin4:
    image: dpage/pgadmin4
    restart: always
    environment:
      PGADMIN_DEFAULT_EMAIL: "angelo@osradar.com"
      PGADMIN_DEFAULT_PASSWORD: "angelo123"
    ports:
     - "8034:80"
    depends_on:
     - db
    networks:
     - postgresql

networks:
  postgresql:
       driver: bridge

volumes:
  data-postgresql:
    external: true
```
I had to add the following;
```
  ports:
     - "5432:5432"
```
So that I could connect to the database directly in development.

## Entity Framework
### Code First
[asp-net-core-entity-framework-core-with-postgresql-code-first](https://medium.com/faun/asp-net-core-entity-framework-core-with-postgresql-code-first-d99b909796d7) 
```
add-migration Initial -Context TenantAwareDbContext
```
The above builds the migration code, which only gets run at runtime in the task of provisioning a new Tenant.  When a new Tenant gets created a new database and all its tables get spun up, and that is only done when needed.


### Database First
```
dotnet tool install --global dotnet-ef
```
[dotnet-ef-not-found-in-net-core-3](https://stackoverflow.com/questions/57066856/dotnet-ef-not-found-in-net-core-3)   
[www.npgsql.org efcore](https://www.npgsql.org/efcore/index.html)  

[dated by still ok.  getting-started-with-entity-framework-core-postgresql](https://medium.com/@RobertKhou/getting-started-with-entity-framework-core-postgresql-c6fa09681624)  
For database first imports the following is all I needed for a console app.
```
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>netcoreapp3.1</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="3.1.9">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="3.1.4" />
    <PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL.Design" Version="1.1.0" />
  </ItemGroup>

</Project>

```

