using DataAccessLib.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLib.DataAccess
{
    public class PeopleContextSqlite : DbContext
    {
        public PeopleContextSqlite(DbContextOptions<PeopleContextSqlite> options) : base(options) {}

        public List<Person> Persons { get; set; }
        public List<Address> Addresses { get; set; }
        public List<Email> Emails { get; set; }
    }
}
