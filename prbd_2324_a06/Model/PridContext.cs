﻿using Microsoft.EntityFrameworkCore;
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

         var connectionString = ConfigurationManager.ConnectionStrings["SqliteConnectionString"].ConnectionString;
         optionsBuilder.UseSqlite(connectionString);

        /*
         * SQL Server
         */

        //var connectionString = ConfigurationManager.ConnectionStrings["MsSqlConnectionString"].ConnectionString;
        //optionsBuilder.UseSqlServer(connectionString);

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
        .HasMany(user => user.AllOperations)
        .WithOne(op => op.Initiator)
        .OnDelete(DeleteBehavior.Cascade);


        // repartition many to many
        modelBuilder.Entity<User>()
            .HasMany(u => u.Repartitions)
            .WithMany(u => u.Participants)
            .UsingEntity<Repartition>(
                left => left
                .HasOne(r => r.Operation)
                .WithMany()
                .HasForeignKey(nameof(Repartition.OperationId))
                .OnDelete(DeleteBehavior.ClientCascade),
                right => right
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(nameof(Repartition.UserId))
                .OnDelete(DeleteBehavior.ClientCascade),
                joinEntity => {
                    joinEntity.HasKey(r => new {r.OperationId, r.UserId});
                }
                
                );


        modelBuilder.Entity<User>()
            .HasMany(u => u.Templates)
            .WithMany(u => u.Initiators)
            .UsingEntity<TemplateItem>(
                left => left
                    .HasOne(ti => ti.Template)
                    .WithMany()
                    .HasForeignKey(nameof(TemplateItem.TemplateId))
                    .OnDelete(DeleteBehavior.ClientCascade),
                right => right
                    .HasOne(ti => ti.User)
                    .WithMany()
                    .HasForeignKey(nameof(TemplateItem.UserId))
                    .OnDelete(DeleteBehavior.ClientCascade),
                joinEntity => {
                    joinEntity.HasKey(ti => new { ti.UserId, ti.TemplateId });
                });


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




        seedData(modelBuilder);
    }
    private static void seedData(ModelBuilder modelBuilder) {
        var boris = new User(1, "boverhaegen@epfc.eu", "Password1,", "Boris",0);
        var benoit = new User(2, "bepenelle@epfc.eu", "Password1,", "Benoit", 0);
        var xavier = new User(3, "xapigeolet@epfc.eu", "Password1,", "Xavier", 0);
        var marc = new User(4, "mamichel@epfc.eu", "Password1,", "Marc", 0);
        var admin = new User(5, "admin@epfc.eu", "Password1,", "Administrator", 1);
        
        modelBuilder.Entity<User>()
            .HasData(boris, benoit, xavier, marc, admin);



        modelBuilder.Entity<Tricount>()
             .HasData(
                new Tricount { Id = 1, Title="Gers 2023", Description =null, CreatedAt = DateTime.Parse("2023-10-10 18:42:24"), CreatorId=1 },
                new Tricount { Id = 2, Title = "Resto badminton", Description = null, CreatedAt = DateTime.Parse("2023-10-10 19:25:10"), CreatorId = 1 },
                new Tricount { Id = 4, Title = "Vacances", Description = "A la mer du Nord", CreatedAt = DateTime.Parse("2023-10-10 19:31:09"), CreatorId = 1 },
                new Tricount { Id = 5, Title = "Grosse virée", Description = "A Torremolinos", CreatedAt = DateTime.Parse("2023-08-15 10:00:00"), CreatorId = 2 },
                new Tricount { Id = 6, Title = "Torhout Werchter", Description = "Memorabile", CreatedAt = DateTime.Parse("2023-06-02 18:30:12"), CreatorId = 3 }
            ) ;


        modelBuilder.Entity<Subscription>()
            .HasData(
                new Subscription { UserId = 1, TricountId = 1 },
                new Subscription { UserId = 1, TricountId = 2 },
                new Subscription { UserId = 1, TricountId = 4 },
                new Subscription { UserId = 1, TricountId = 6 },
                new Subscription { UserId = 2, TricountId = 2 },
                new Subscription { UserId = 2, TricountId = 4 },
                new Subscription { UserId = 2, TricountId = 5 },
                new Subscription { UserId = 2, TricountId = 6 },
                new Subscription { UserId = 3, TricountId = 4 },
                new Subscription { UserId = 3, TricountId = 5 },
                new Subscription { UserId = 3, TricountId = 6 },
                new Subscription { UserId = 4, TricountId = 4 },
                new Subscription { UserId = 4, TricountId = 5 },
                new Subscription { UserId = 4, TricountId = 6 }
            );

        modelBuilder.Entity<Operation>()
            .HasData(
                new Operation {Id = 1, Title = "Colruyt", TricountId=4, Amount = 100, OperationDate = new DateTime(2023,10,13), InitiatorId=2},
                new Operation {Id = 2, Title = "Plein essence", TricountId=4, Amount = 75, OperationDate = new DateTime(2023,10,13), InitiatorId=1},
                new Operation {Id = 3, Title = "Grosse courses LIDL", TricountId=4, Amount = 212.47, OperationDate = new DateTime(2023,10,13), InitiatorId=3},
                new Operation {Id = 4, Title = "Aperos", TricountId=4, Amount = 31.897456217, OperationDate = new DateTime(2023,10,13), InitiatorId=1},
                new Operation { Id = 5, Title = "Boucherie", TricountId = 4, Amount = 25.5, OperationDate = new DateTime(2023,10,26), InitiatorId = 2 },
                new Operation { Id = 6, Title = "Loterie", TricountId = 4, Amount = 35, OperationDate = new DateTime(2023,10,26), InitiatorId = 1 },
                new Operation { Id = 7, Title = "Sangria", TricountId = 5, Amount = 42, OperationDate = new DateTime(2023,08, 16), InitiatorId = 2 },
                new Operation { Id = 8, Title = "Jet Ski", TricountId = 5, Amount = 250, OperationDate = new DateTime(2023,08,17), InitiatorId = 3 },
                new Operation { Id = 9, Title = "PV parking", TricountId = 5, Amount = 15.5, OperationDate = new DateTime(2023,08,16), InitiatorId = 3 },
                new Operation { Id = 10, Title = "Tickets", TricountId = 6, Amount = 220, OperationDate = new DateTime(2023,06,08), InitiatorId = 1 },
                new Operation { Id = 11, Title = "Decathlon", TricountId = 6, Amount = 199.9, OperationDate = new DateTime(2023,07,01), InitiatorId =2}
            );

        modelBuilder.Entity<Repartition>()
            .HasData(
                new Repartition { OperationId = 1, UserId = 1 , Weight = 1 },
                new Repartition {  OperationId = 1, UserId = 2 , Weight = 1 },
                new Repartition {  OperationId = 2, UserId = 1 , Weight = 1 },
                new Repartition {OperationId = 2, UserId = 2, Weight = 2 },
                new Repartition { OperationId = 3, UserId = 1 , Weight = 2 },
                new Repartition {OperationId = 3, UserId = 2, Weight = 2 },
                new Repartition {OperationId = 3, UserId = 3, Weight = 3 },
                new Repartition { OperationId = 4, UserId = 1 , Weight = 1 },
                new Repartition {  OperationId = 4, UserId = 2 , Weight = 2 },
                new Repartition { OperationId = 4, UserId = 3 , Weight = 3 },
                new Repartition {OperationId = 5, UserId = 1, Weight = 1 },
                new Repartition {OperationId = 5, UserId = 2, Weight = 2 },
                new Repartition {OperationId = 5, UserId = 3, Weight = 3 },
                new Repartition { OperationId = 6, UserId = 1 , Weight = 1 },
                new Repartition {OperationId = 6, UserId = 3, Weight = 3 },
                new Repartition {OperationId = 7, UserId = 2, Weight = 2 },
                new Repartition {OperationId = 7, UserId = 2, Weight = 2 },
                new Repartition {OperationId = 7, UserId = 3, Weight = 3 },
                new Repartition {OperationId = 8, UserId = 3, Weight = 3 },
                new Repartition {OperationId = 8, UserId = 4, Weight = 4 },
                new Repartition {OperationId = 9, UserId = 2, Weight = 2 },
                new Repartition {OperationId = 9, UserId = 4, Weight = 4 },
                new Repartition { OperationId = 10, UserId = 1 , Weight = 1 },
                new Repartition {OperationId = 10, UserId = 3, Weight = 3 },
                new Repartition { OperationId = 11, UserId = 2 , Weight = 2 },
                new Repartition {OperationId = 11, UserId = 4, Weight = 4 }
            );



        modelBuilder.Entity<Template>()
            .HasData(
                new Template {Id=1, Title ="Boris paye double" , TricountId=4},
                new Template {Id=2, Title ="Benoit ne paye rien" , TricountId=4}
            );

        modelBuilder.Entity<TemplateItem>()
            .HasData(
                new TemplateItem {  TemplateId = 1, UserId = 1 , Weight = 2},
                new TemplateItem { TemplateId = 2, UserId = 2 , Weight = 1},
                new TemplateItem { TemplateId = 1, UserId = 3 , Weight = 1},
                new TemplateItem { TemplateId = 2, UserId = 1 , Weight = 1},
                new TemplateItem {TemplateId = 2, UserId = 3, Weight = 1 }
            );
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Tricount> Tricounts => Set<Tricount>();
    public DbSet<Template> Templates => Set<Template>();
    public DbSet<Operation> Operations => Set<Operation>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();
    public DbSet<TemplateItem> TemplateItems => Set<TemplateItem>();
    public DbSet<Repartition> Repartitions => Set<Repartition>();

}