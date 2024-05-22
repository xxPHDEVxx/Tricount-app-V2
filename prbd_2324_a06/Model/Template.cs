using PRBD_Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prbd_2324_a06.Model;

public class Template : EntityBase<PridContext> {
    [Key]
    public int Id { get; set; }
    public string Title {  get; set; }
    [ForeignKey(nameof(Tricount))]
    public int TricountId { get; set; }
    public virtual Tricount Tricount { get; set; }
    
    public virtual ICollection<User> Initiators { get; set; } = new HashSet<User>();
}

