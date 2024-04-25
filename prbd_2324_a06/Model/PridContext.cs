using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PRBD_Framework;
using System.Configuration;

namespace prbd_2324_a06.Model;

public class PridContext : DbContextBase
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        base.OnConfiguring(optionsBuilder);

        /*
         * SQLite
         */

         //var connectionString = ConfigurationManager.ConnectionStrings["SqliteConnectionString"].ConnectionString;
         //optionsBuilder.UseSqlite(connectionString);

        /*
         * SQL Server
         */

        var connectionString = ConfigurationManager.ConnectionStrings["MsSqlConnectionString"].ConnectionString;
        optionsBuilder.UseSqlServer(connectionString);

        ConfigureOptions(optionsBuilder);
    }

    private static void ConfigureOptions(DbContextOptionsBuilder optionsBuilder) {
        optionsBuilder.UseLazyLoadingProxies()
            .LogTo(Console.WriteLine, LogLevel.Information) // permet de visualiser les requêtes SQL générées par LINQ
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors() // attention : ralentit les requêtes
            ;

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);

        //user participe à plusieurs tricounts
        modelBuilder.Entity<User>()
            .HasMany(user => user.Tricounts)
            .WithOne(tricount => tricount.Creator)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<User>()
            .HasMany(user => user.TemplateItems)
            .WithOne(templateItem => templateItem.User)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>()
            .HasMany(user => user.Operations)
            .WithOne(operation => operation.Initiator)
            .OnDelete(DeleteBehavior.NoAction);



        modelBuilder.Entity<User>()
            .HasMany(u => u.Subscriptions)
            .WithMany(u => u.Subscribers)
            .UsingEntity<Subscription>(
            left => left.HasOne(s => s.Tricount).WithMany().HasForeignKey(nameof(Subscription.TricountId))
            .OnDelete(DeleteBehavior.ClientCascade),
            right => right.HasOne(s => s.User).WithMany().HasForeignKey(nameof(Subscription.UserId))
            .OnDelete(DeleteBehavior.ClientCascade),
            joinEntity => {
                joinEntity.HasKey(s => new { s.UserId, s.TricountId });

            });

        modelBuilder.Entity<Tricount>()
            .HasMany(t => t.Templates)
            .WithOne(template  => template.Tricount)
            .OnDelete(DeleteBehavior.ClientCascade);

        modelBuilder.Entity<Operation>()
            .HasMany(o => o.Repartitions)
            .WithOne(r => r.Operation) 
            .OnDelete(DeleteBehavior.ClientCascade);

        modelBuilder.Entity<TemplateItem>() 
            .HasOne(templateItem => templateItem.Template )
            .WithMany(template => template.Items)
            .OnDelete(DeleteBehavior.ClientCascade);
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Tricount> Tricounts => Set<Tricount>();
    public DbSet<Template> Templates => Set<Template>();
    public DbSet<Operation> Operations => Set<Operation>();
    public DbSet<Tricount> Subscriptions => Set<Tricount>();
    public DbSet<TemplateItem> TemplateItems => Set<TemplateItem>();
    public DbSet<Repartition> Repartitions => Set<Repartition>();
}