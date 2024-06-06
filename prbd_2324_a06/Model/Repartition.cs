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
}