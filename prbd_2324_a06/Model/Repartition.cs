using PRBD_Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prbd_2324_a06.Model;

public class Repartition : EntityBase<PridContext> {

    [ForeignKey(nameof(Operation))]
    public int OperationId { get; set; }
    public virtual Operation Operation { get; set; }

    [ForeignKey(nameof(User))]
    public int UserId { get; set; }
    public virtual User User { get; set; }
    public int Weight { get; set; }
    public Repartition() { }


}
