using PRBD_Framework;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace prbd_2324_a06.Model;

public class Tricount : EntityBase<PridContext>
{
    [Key] public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }

    [Required, ForeignKey(nameof(Creator))]
    public int CreatorId { get; set; }

    public virtual User Creator { get; set; }

    public virtual ICollection<Subscription> Subscriptions { get; protected set; } = new HashSet<Subscription>();

    public virtual ICollection<Template> Templates { get; protected set; } = new HashSet<Template>();

    public Tricount() { }

    public Tricount(string title, string description, DateTime createdAt, User creator) {
        Id = GetHighestTricountId();
        Title = title;
        Description = description;
        CreatedAt = createdAt;
        Creator = creator;
    }

    public string GetCreatorName() {
        return User.GetUserNameById(CreatorId);
    }

    public int GetHighestTricountId() {
        return Context.Tricounts.Max(o => o.Id) + 1;
    }

    public int NumberOfParticipants() {
        var q = (from s in Subscriptions
            where s.UserId != CreatorId
            select s).Count();
        return q;
    }


    public IQueryable<Operation> GetOperations() {
        var q = from o in Context.Operations
            where o.TricountId == Id
            select o;
        return q;
    }


    public double GetTotal() {
        var total = Context.Operations
            .Where(o => o.TricountId == Id)
            .Sum(o => Math.Round(o.Amount, 2));
        return total;
    }

    public IQueryable<User> GetParticipants() {
        var userIds = Subscriptions
            .Where(sub => sub.TricountId == Id)
            .Select(sub => sub.UserId)
            .ToList();

        var participants = Context.Users
            .Where(user => userIds.Contains(user.UserId));

        return participants;
    }

    public IQueryable<Template> GetTemplates() {
        var id = Templates
            .Where(t => t.TricountId == Id)
            .Select(t => t.Id);

        var templates = Context.Templates
            .Where(t => id.Contains(t.Id));
        return templates;
    }

    public Template GetTemplateByTitle(string title) {
        return Templates.FirstOrDefault(t => t.Title == title);
    }

    public override bool Validate() {
        ClearErrors();

        if (string.IsNullOrWhiteSpace(Title))
            AddError(nameof(Title), "required");
        else if (Title.Length < 3)
            AddError(nameof(Title), "length must be >= 3");


        // Validation de la description si elle contient quelque chose
        if (!string.IsNullOrWhiteSpace(Description) && Description.Length < 3) {
            AddError(nameof(Description), "length must be >= 3, ");
        }

        return !HasErrors;
    }

    public void NewSubscriber(int userId) {
        var s = new Subscription { UserId = userId, TricountId = Id };
        Context.Subscriptions.Add(s);
        Subscriptions.Add(s);
        Context.SaveChanges();
    }

    // récupere la derniere opération
    public Operation GetLatestOperation() {
        return Context.Operations
            .Where(o => o.TricountId == Id)
            .OrderByDescending(o => o.OperationDate)
            .FirstOrDefault();
    }

    public void Delete() {
        Subscriptions.Clear();
        Templates.Clear();
        Context.Tricounts.Remove(this);
        Context.SaveChanges();
    }
}