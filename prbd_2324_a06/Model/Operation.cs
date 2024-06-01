using PRBD_Framework;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace prbd_2324_a06.Model;

public class Operation : EntityBase<PridContext> {
    [Key]
    public int Id {  get; set; }
    public string Title { get; set; }
    [ForeignKey(nameof(Tricount))]
    public int TricountId { get; set; }
    public virtual Tricount Tricount { get; set; }

    [Required]
    public double Amount { get; set; }

    [Required]
    public DateTime OperationDate { get; set; }

    [ForeignKey(nameof(Initiator))]
    public int InitiatorId { get; set; }
    public virtual User Initiator { get; set; }


    public virtual ICollection<Repartition> Repartitions { get; protected set; } = new HashSet<Repartition>();
}

