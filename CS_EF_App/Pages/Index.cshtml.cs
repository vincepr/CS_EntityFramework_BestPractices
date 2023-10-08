using System.Text.Json;
using DataAccessLib.DataAccess;
using DataAccessLib.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CS_EF_App.Pages;

public class IndexModel : PageModel {
    private readonly ILogger<IndexModel> _logger;
    private readonly PeopleContextSqlite _dbCtx;

    public IndexModel(ILogger<IndexModel> logger, PeopleContextSqlite dbCtx) {
        _logger = logger;
        _dbCtx = dbCtx;
    }
    public void OnGet() {
        LoadSampleData();
        // GetAllTheData();
        WhereClause();
    }
    private void LoadSampleData() {
        // load and insert our json file if there is no entries in the database
        if (_dbCtx.Persons.Count() == 0) {
            string file = System.IO.File.ReadAllText("generated.json");
            var jsonPersons = JsonSerializer.Deserialize<List<Person>>(file);
            if (jsonPersons is null) return;
            _dbCtx.AddRange(jsonPersons);
            _dbCtx.SaveChanges();
        }
    }
    private void DeleteAllPersons() {
        var people = _dbCtx.Persons
            .ToList();
        foreach (Person p in people) {
            _dbCtx.Persons.Remove(p);
        }
        _dbCtx.SaveChanges(); 
    }
    private void GetAllTheData() {
        var people = _dbCtx.Persons
        .Include(a => a.Addresses)
        .Include(e => e.EmailAddresses)
        .ToList();
        // a inefficient way to get all data: Rider even gives a warning about average number of records: 530 for just 100 objects
        // 		SELECT "p"."Id", "p"."Age", "p"."FirstName", "p"."LastName", "a"."Id", "a"."City", "a"."PersonId", "a"."State", "a"."StreetAddress", "a"."ZipCode", "e"."Id", "e"."EmailAddress", "e"."PersonId"
        // 		FROM "Persons" AS "p"
        // 		LEFT JOIN "Addresses" AS "a" ON "p"."Id" = "a"."PersonId"
        //		LEFT JOIN "Emails" AS "e" ON "p"."Id" = "e"."PersonId"
        //		ORDER BY "p"."Id", "a"."Id"
        // 
        // So the data for just ONE Person might look like this:
        //
        // Id   Age FName   LName   Id  City    PersonId    State   Street  Zip Id  Email
        // 1    33  James   Bond    1   Town    1           Alabama SomeStr A1V 234 brookmail@gmx.de     
        // 1    33  James   Bond    1   Town    1           Alabama SomeStr A1V 22  james@bond.mit
        // 1    33  James   Bond    1   Town    1           Alabama SomeStr A1V 113 spy@seecret.eu
        // 1    33  James   Bond    1   Town    1           Alabama SomeStr A1V 234 another@mail.ml
        // 1    33  James   Bond    213 Town    1           Oregon  LakeAV  123 234 brookmail@gmx.de     
        // 1    33  James   Bond    213 Town    1           Oregon  LakeAV  123 22  james@bond.mit
        // 1    33  James   Bond    213 Town    1           Oregon  LakeAV  123 113 spy@seecret.eu
        // 1    33  James   Bond    213 Town    1           Oregon  LakeAV  123 234 another@mail.ml
        // 1    33  James   Bond    111 Town    1           Guam    street  IB1 234 brookmail@gmx.de     
        // 1    33  James   Bond    111 Town    1           Guam    street  IB1 22  james@bond.mit
        // 1    33  James   Bond    111 Town    1           Guam    street  IB1 113 spy@seecret.eu
        // 1    33  James   Bond    111 Town    1           Guam    street  IB1 234 another@mail.ml
        //
        // it would be way less data to first get all persons, then all addresses then all emails and put them together
        // but the default behavior of Entity Framework does it in this inefficient way
        //
        // An this would get multiplicatively worse for more and more relations -> more and more left joins + extra data
    }

    private void WhereClause() {
        var peopleGOOD = _dbCtx.Persons
            .Where(p => p.Age >= 18 && p.Age < 30)
            .ToList();
        var peopleBAD = _dbCtx.Persons
            .ToList()
            .Where(p => ApprovedAge(p.Age));
        
        Console.WriteLine( peopleGOOD.Count() ); // just fetched the 44 targeted people
        Console.WriteLine( peopleBAD.Count() );  // this fetched all 100 potentially 1000s of people then filtered them down to 44
    }
    
    private bool ApprovedAge(int age) {
        return (age >= 18 && age < 30);
    }
}