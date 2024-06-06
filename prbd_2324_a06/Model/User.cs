using PRBD_Framework;
using System.ComponentModel.DataAnnotations;

namespace prbd_2324_a06.Model;

public enum Role {
    User = 0,
    Administrator = 1
}

public class User : EntityBase<PridContext>
{
    [Key]
    public int UserId { get; set; }
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
    
    public User(int userdId,string mail, string hashed_password, string full_name) {
        UserId = userdId;
        Mail = mail;
        HashedPassword = hashed_password;
        FullName = full_name;
    }
    
    // Return the highest UserId
    public int GetHighestUserId()
    {
        return Context.Users.Max(u => u.UserId);
    }


    public virtual ICollection<Subscription> Subscriptions { get; protected set; } = new HashSet<Subscription>();
    public virtual ICollection<Repartition> Repartitions { get; protected set; } = new HashSet<Repartition>();
}