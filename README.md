# Entity Framework Best Practices
working trough https://www.youtube.com/watch?v=qkJ9keBmQWo and taking notes along the way.
currently at 1:11:11

## Setup
1. install the EntityFramework Dependencies
```
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
```

1. Create a ClassLibrary DataAccessLib
	- create our Models that live in the Db
	- create our DbContext for whatever Database we start with (sqlite in this case)
	- if we use a real db hook up the connection-string etc...

```csharp
public class PeopleContextSqlite : DbContext{
    public PeopleContextSqlite(DbContextOptions<PeopleContextSqlite> options) : base(options) {}
    public DbSet<Person> Persons { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<Email> Emails { get; set; }
}
```

1. Import our Custom-DBContext in our Frontend/Api Project
	- we can RightClick-Add-ProjectReference
	- or add it manually to the .csproj file
```
  <ItemGroup>
    <ProjectReference Include="..\DataAccessLib\DataAccessLib.csproj" />
  </ItemGroup>
```
1. Then Reference/Use our Custom-DbContext in our Program.cs
```csharp
builder.Services.AddDbContext<PeopleContextSqlite>(options =>{
    // options.UseSqlServer(Configuration.GetConnectionString("DefaultDev")); // "DefaultDev" defined in our appsettings.json
    options.UseSqlite("Data Source=EFlocalDB.db");  // just a local sqlite file that will get created for us for development
});
```

1. Tell EntityFramework to generate the Migrations and put them in the Database

	- In this case we need to run it from our DataAccessLib (so we cd into that)
	- Since our Startup Project is in another folder we have to point to it:
```
dotnet ef --startup-project ../CS_EF_App/ migrations add initialMigration
dotnet ef --startup-project ../CS_EF_App/ database update
```

Alternatively we can from our root path (solution folder that holds both projects) work like this:
```
dotnet ef --project ./DataAccessLib --startup-project ./CS_EF_App/ migrations add initialMigration
dotnet ef --project ./DataAccessLib --startup-project ./CS_EF_App/ database update
```

## Notes
**Migration-Scripts** - the steps EF builds out our DB with.

These are split in 2 functions. Up and Down.
`Up(MigrationBuilder migrationBuilder)` - this creates our tables (and adds ontop of existing ones for followup migrations)
`Down(MigrationBuilder migrationBuilder)` - rolls back our database back down to the state of the previous Migration.
Or drops it if that table didnt exist before. So you might **loose data** when rolling back!


### EF is not efficient by default - nvarchar(max) and indexing
limits: varchar(8000) and nvarchar(4000) of how much symbols a row can hold.

In Windows SqlServer nvarchar(max) goes way beyond that by storing those on the drive as 'files' and just storing a pointer in the db

- problem is speed for searching on those rows or indexing into them etc.
- so EF's defaults generated might not always be optimal/sensible depending on use case and underlying database
- In Sql-Servers case nvarchar(max) also allocates memory for each lookup before reading it out. 
  - for a 256 long nvarchar or varchar allocated heap-memory usually will be zero
  - while for a 1024+ .. max will allocate on the heap.
  - the above is just the database itself. So it's performance might degrade quite a bit.

So to summarize EF by default will **not** create ideal table design.

### The Solution - Data Annotations
We can for example for the zip code go to the more efficient varchar and in general make sensible assumptions about length
```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLib.Models;
public class Address{
    public int Id { get; set; }
    
    [Required, MaxLength(200)]
    public string StreetAddress { get; set; }
    
    [Required, MaxLength(100)]
    public string City { get; set; }
    
    [Required, MaxLength(50)]
    public string State { get; set; }
   
    // We can put them in same section or stack them:
    [Required]
    [MaxLength]
    [Column(TypeName = "varchar(10)")]  // we can basically write raw sql like this
    public string ZipCode { get; set; }
}
```