using PRBD_Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prbd_2324_a06.Model;

public class TemplateItem : EntityBase<PridContext> {
    [Key]
    public int Id { get; set; }
    [ForeignKey(nameof(User))]
    public int UserId { get; set; }
    public virtual User User { get; set; }

    [ForeignKey(nameof(Template))]
    public int TemplateId { get; set; }
    public virtual Template Template { get; set; }
    public int Weight { get; set; }

    public TemplateItem() { }   

}

