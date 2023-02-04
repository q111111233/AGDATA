using Microsoft.EntityFrameworkCore;

namespace AGDATA.Models
{
    public class PersonContext : DbContext
    {
        public PersonContext(DbContextOptions<PersonContext> options) : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase(databaseName: "PersonDb");
        }

        public virtual DbSet<Person> Persons { get; set; } = null!;
    }
}
