using PRBD_Framework;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace prbd_2324_a06.Model;

public enum Role
{
    User = 0,
    Administrator = 1
}

public class User : EntityBase<PridContext>
{
    [Key] public int UserId { get; set; }
    public string Mail { get; set; }
    public string HashedPassword { get; set; }
    public string FullName { get; set; }
    public Role Role { get; protected set; } = Role.User;

    public User() { }

    // constructeur avec autoincrémentation pour sign up
    public User(string mail, string hashed_password, string full_name) {
        UserId = GetHighestUserId() + 1;
        Mail = mail;
        HashedPassword = hashed_password;
        FullName = full_name;
    }

    public User(int userdId, string mail, string hashed_password, string full_name) {
        UserId = userdId;
        Mail = mail;
        HashedPassword = hashed_password;
        FullName = full_name;
    }

    // Return the highest UserId
    public int GetHighestUserId() {
        return Context.Users.Max(u => u.UserId);
    }

    // Return User found by name
    public static User GetUserByName(string name) {
        return Context.Users.FirstOrDefault(u => u.FullName == name);
    }

    public virtual ICollection<Subscription> Subscriptions { get; protected set; } = new HashSet<Subscription>();
    public virtual ICollection<Repartition> Repartitions { get; protected set; } = new HashSet<Repartition>();

    public IQueryable<Tricount> GetTricounts() {
        var tricounts = from t in Context.Tricounts
            where t.CreatorId == UserId
            orderby t.CreatedAt descending
            select t;
        return tricounts;
    }

    public IQueryable<Tricount> GetFiltered(string Filter) {
        var filtered = from t in GetTricounts().Union(GetParticipatedTricounts())
            where t.Title.Contains(Filter)
            orderby t.Title
            select t;
        return filtered;
    }

    public IQueryable<Tricount> GetParticipatedTricounts() {
        var participatedTricounts = from s in Context.Subscriptions
            join t in Context.Tricounts on s.TricountId equals t.Id
            where s.UserId == UserId
            orderby t.CreatedAt descending
            select t;
        return participatedTricounts;
    }
    public IQueryable<Tricount> GetAll() {
        var tricounts = from t in Context.Tricounts
                        orderby t.CreatedAt descending
                        select t;
        return tricounts;
    }
    public static string GetUserNameById(int userId) {
        var u = Context.Users.SingleOrDefault(u => u.UserId == userId);
        return u.FullName;
    }

    public double GetMyExpenses(Tricount tricount) {
        double myExpenses = 0;

        foreach (var operation in tricount.GetOperations()) {
            double operationWeight = 0;
            double userWeight = 0;

            foreach (var repartition in operation.GetRepartitionByOperation()) {
                operationWeight += repartition.Weight;
                if (repartition.UserId == UserId) {
                    userWeight = repartition.Weight;
                }
            }

            if (operationWeight > 0) {
                myExpenses += (operation.Amount / operationWeight) * userWeight;
            }
        }

        return Math.Round(myExpenses, 2);
    }


    public double GetMyBalance(Tricount tricount) {
        double myPaid = 0;
        double myExpenses = GetMyExpenses(tricount);

        foreach (var operation in tricount.GetOperations()) {
            if (operation.InitiatorId == UserId) {
                myPaid += operation.Amount;
            }
        }

        // La balance est le total des crédits moins les dépenses
        return Math.Round(myPaid - myExpenses, 2);
    }
}