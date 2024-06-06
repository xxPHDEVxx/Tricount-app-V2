using PRBD_Framework;
using System.ComponentModel.DataAnnotations.Schema;


namespace prbd_2324_a06.Model;

public class Repartition : EntityBase<PridContext>
{
    public Repartition() {
    }

    public Repartition(int operationId, int userId, int weight) {
        OperationId = operationId;
        UserId = userId;
        Weight = weight;
    }

    [ForeignKey(nameof(Operation))] public int OperationId { get; set; }
    public virtual Operation Operation { get; set; }

    [ForeignKey(nameof(User))] public int UserId { get; set; }
    public virtual User User { get; set; }
    public int Weight { get; set; }

    public int GetWeightForUserAndOperation(int userId, int operationId) {
        var q = Context.Repartitions
            .Where(r => r.OperationId == OperationId)
            .Where(r => r.UserId == UserId)
            .Sum(r => r.Weight);
        return q;
    }
}