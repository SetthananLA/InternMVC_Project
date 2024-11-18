using Microsoft.EntityFrameworkCore;
using InternProjectMVC.Models;

namespace InternProjectMVC.Models{
    public class MyDatabaseContext : DbContext
    {
        public MyDatabaseContext(DbContextOptions<MyDatabaseContext> options) : base(options) { }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<LogHistory> LogHistories { get; set; }
        public DbSet<Program> Programs { get; set; }
        public DbSet<Action> Actions { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Certified> Certifieds { get; set; }
        public DbSet<PendingAuth> PendingAuths { get; set; }

    }
}
