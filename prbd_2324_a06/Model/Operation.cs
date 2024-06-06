using PRBD_Framework;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Windows.Documents;

namespace prbd_2324_a06.Model;

public class Operation : EntityBase<PridContext>
{
    public Operation(string title, int tricountId, double amount, DateTime operationDate, int initiatorId) {
        Id = GetHighestOperationId();
        Title = title;
        TricountId = tricountId;
        Amount = amount;
        OperationDate = operationDate;
        InitiatorId = initiatorId;
    }

    public Operation() {
    }

    [Key] public int Id { get; set; }
    public string Title { get; set; }
    [ForeignKey(nameof(Tricount))] public int TricountId { get; set; }
    public virtual Tricount Tricount { get; set; }

    [Required] public double Amount { get; set; }

    [Required] public DateTime OperationDate { get; set; }

    [ForeignKey(nameof(Initiator))] public int InitiatorId { get; set; }
    public virtual User Initiator { get; set; }


    public virtual ICollection<Repartition> Repartitions { get; protected set; } = new HashSet<Repartition>();

    public void Delete() {
        Repartitions.Clear();
        Context.Operations.Remove(this);
        Context.SaveChanges();
    }

    public override bool Validate() {
        ClearErrors();

        if (string.IsNullOrWhiteSpace(Title))
            AddError(nameof(Title), "required");
        else if (Title.Length < 3)
            AddError(nameof(Title), "length must be >= 3");
        return !HasErrors;
    }

    public int GetHighestOperationId() {
        return Context.Operations.Max(o => o.Id) + 1;
    }


    public IQueryable<Repartition> GetRepartitionByOperation() {
        var q = from r in Context.Repartitions
            where r.OperationId == Id
            select r;
        return q;
    }
}