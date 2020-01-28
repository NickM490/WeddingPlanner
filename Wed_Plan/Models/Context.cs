using Microsoft.EntityFrameworkCore;

namespace Wedding_Planner.Models
{
    public class MyContext : DbContext
    {
        // base() calls the parent class' constructor passing the "options" parameter along
        public MyContext(DbContextOptions options) : base(options) { }

        public DbSet<User> Users {get;set;}
        public DbSet<Wedding> Weddings {get;set;}

        public DbSet<RSVP> RSVPs { get; set; }
    }
}
