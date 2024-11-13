using Microsoft.EntityFrameworkCore;

public class MyDatabaseContext : DbContext
{
    public MyDatabaseContext(DbContextOptions<MyDatabaseContext> options) : base(options) { }

    public DbSet<Employee> Employees { get; set; }
}
