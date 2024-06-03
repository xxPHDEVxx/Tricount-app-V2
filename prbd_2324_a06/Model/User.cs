using PRBD_Framework;
using System.ComponentModel.DataAnnotations;

namespace prbd_2324_a06.Model;

public enum Role {
    Member = 0,
    Administrator = 1
}

public class User : EntityBase<PridContext>
{
    [Key]
    public int UserId { get; set; }
    public string Mail { get; set; }
    public string HashedPassword { get; set; }
    public string FullName { get; set; }
    public int Role { get; protected set; }


    public User() { }
    
    // constructeur avec autoincrémentation pour sign up
    public User(string mail, string hashed_password, string full_name, int role) {
        UserId = GetHighestUserId() + 1;
        Mail = mail;
        HashedPassword = hashed_password;
        FullName = full_name;
        Role = role;
    }
    
    public User(int userdId,string mail, string hashed_password, string full_name, int role) {
        UserId = userdId;
        Mail = mail;
        HashedPassword = hashed_password;
        FullName = full_name;
        Role = role;
    }
    
    // Return the highest UserId
    public int GetHighestUserId()
    {
        return Context.Users.Max(u => u.UserId);
    }


    public virtual ICollection<Subscription> Subscriptions { get; protected set; } = new HashSet<Subscription>();
    public virtual ICollection<Repartition> Repartitions { get; protected set; } = new HashSet<Repartition>();

    public IQueryable<Tricount> GetTricounts() {
        var tricounts = from t in Context.Tricounts
                        where t.CreatorId == UserId
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
                                    select t;
        return participatedTricounts;
    }
}