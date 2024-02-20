using System.Windows;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace PRBD_Framework; 

public class DbContextBase : DbContext {
    public DbContextBase() { }

    public DbContextBase(DbContextOptions options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        AppDomain.CurrentDomain.SetData("DataDirectory",
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
        base.OnConfiguring(optionsBuilder);
    }
        
    public override int SaveChanges() {
        int count = -1;
        bool hasTransaction = Database.CurrentTransaction != null;
        try {
            if (ExecuteValidation()) {
                // s'il n'y a pas de transaction en cours, en créer une
                if (!hasTransaction)
                    Database.BeginTransaction();
                count = base.SaveChanges();
                if (!hasTransaction)
                    Database.CommitTransaction();
                return count;
            }
            Console.WriteLine("SaveChanges() not successful due to business rules errors");
        } catch (DbUpdateConcurrencyException ex) {
            Console.WriteLine("SaveChanges() not successful due to optimistic locking violation");
            // see: https://docs.microsoft.com/en-us/ef/core/saving/concurrency
            foreach (var entry in ex.Entries) {
                entry.Reload();
            }
            ApplicationRoot.NotifyColleagues(ApplicationBaseMessages.MSG_REFRESH_DATA);
        }

        if (!hasTransaction)
            Database.RollbackTransaction();
        return count;
    }

    private bool ExecuteValidation() {
        bool hasErrors = false;
        foreach (var entity in from entry in ChangeTracker.Entries()
                     .Where(e => e.State is EntityState.Added or EntityState.Modified).ToList() 
                 where entry.Entity is ValidatableObjectBase select entry.Entity as ValidatableObjectBase) {
            entity.Validate();
            if (!entity.HasErrors) continue;
            MessageBox.Show(Application.Current.MainWindow!,
                $"Business rules errors in entity of type {entity.GetType().Name}:\n" +
                JsonConvert.SerializeObject(entity.Errors, Formatting.Indented), "Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
            hasErrors = true;
        }
        return !hasErrors;
    }

    // public int SaveChangesWithIdentityInsert<T>() {
    //     int ret;
    //     // see:  https://entityframeworkcore.com/knowledge-base/58847327/set-identity-insert-not-working-when-using-transactionscope-in-entity-framework-core-3
    //     Database.OpenConnection();
    //     try {
    //         SetIdentityInsert<T>(true);
    //         ret = base.SaveChanges();
    //         SetIdentityInsert<T>(false);
    //     } finally {
    //         Database.CloseConnection();
    //     }
    //     return ret;
    // }
    //
    // private void SetIdentityInsert<T>(bool enable) {
    //     if (!Database.IsSqlServer() || !HasIdentity<T>()) return;
    //     IEntityType entityType = Model.FindEntityType(typeof(T));
    //     string value = enable ? "ON" : "OFF";
    //     string query = $"SET IDENTITY_INSERT {entityType.GetSchema()}.{entityType.GetTableName()} {value}";
    //     Database.ExecuteSqlRaw(query);
    // }
    //
    // private bool HasIdentity<T>() {
    //     var efEntity = Model.FindEntityType(typeof(T));
    //     var efProperties = efEntity.GetProperties();
    //     return efProperties.Any(p => {
    //         return p.IsPrimaryKey() && p.ValueGenerated == ValueGenerated.OnAdd;
    //     });
    // }
}