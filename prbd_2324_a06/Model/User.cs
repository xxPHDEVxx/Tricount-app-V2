using PRBD_Framework;

namespace prbd_2324_a06.Model;

public enum Role {
    Member = 0,
    Administrator = 1
}

public class User : EntityBase<PridContext>
{
    public int UserId { get; set; }
    public string Mail { get; set; }
    public string HashedPassword { get; set; }
    public string FullName { get; set; }
    public int Role { get; protected set; } 

    public User() { }
    public User(int userId, string mail, string hashed_password, string full_name, int role) {
        UserId = userId;
        Mail = mail;
        HashedPassword = hashed_password;
        FullName = full_name;
        Role = role;
    }

    public virtual ICollection<Tricount> Tricounts { get; protected set; } = new HashSet<Tricount>();
    public virtual ICollection<Tricount> Subscriptions { get; protected set; } = new HashSet<Tricount>();
    public virtual ICollection<Operation> Repartitions { get; protected set; } = new HashSet<Operation>();
    public virtual ICollection<Template> Templates { get; protected set; }  = new HashSet<Template>();  
    
    public virtual ICollection<Operation> AllOperations { get; protected set; } = new HashSet<Operation>();
}