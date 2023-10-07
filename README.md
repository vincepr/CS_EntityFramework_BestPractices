# Entity Framework Best Practices
working trough https://www.youtube.com/watch?v=qkJ9keBmQWo and taking notes along the way.


## Setup
1. install the EntityFramework Dependencies
```
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
```

2. Create a ClassLibrary DataAccessLib
	- create our Models that live in the Db
	- create our DbContext for whatever Database we start with (sqlite in this case)
	- if we use a real db hook up the connectionstring etc...

```csharp
public class PeopleContextSqlite : DbContext{
    public PeopleContextSqlite(DbContextOptions<PeopleContextSqlite> options) : base(options) {}
    public List<Person> Persons { get; set; }
    public List<Address> Addresses { get; set; }
    public List<Email> Emails { get; set; }
}
```

3. Import our Custom-DBContext in our Frontend/Api Project
	- we can RightClick-Add-ProjectReference
	- or add it manually to the .csproj file
```
  <ItemGroup>
    <ProjectReference Include="..\DataAccessLib\DataAccessLib.csproj" />
  </ItemGroup>
```
4. Then Reference/Use our Custom-DbContext in our Program.cs
```csharp
builder.Services.AddDbContext<PeopleContextSqlite>(options =>{
    options.UseSqlite("Date Source=EFlocalDB.db");  // just a local sqlite file that will get created for us for development
});
```

3. Tell EntityFramework to generate the Migrations and put them in the Database

	- In this case we need to run it from our DataAccessLib (so we cd into that)
	- Since our Startup Project is in another folder we have to point to it:
```
dotnet ef --startup-project ../CS_EF_App/ migrations add initialMigration
dotnet ef database update
```
