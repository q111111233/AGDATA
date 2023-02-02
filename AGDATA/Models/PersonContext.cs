using Microsoft.EntityFrameworkCore;

namespace AGDATA.Models
{
    public class PersonContext : DbContext
    {
        public PersonContext(DbContextOptions<PersonContext> options) : base(options)
        { 
        }

        DbSet<Person> Persons { get; set; } = null!;
    }
}
