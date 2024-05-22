using PRBD_Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace prbd_2324_a06.Model;

public class Tricount : EntityBase<PridContext> {
    [Key]
    public int Id { get; set; }
    public string Title {  get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt {  get; set; }
    [Required, ForeignKey(nameof(Creator))]
    public int CreatorId {  get; set; }
    public virtual User Creator { get; set; }
    public virtual ICollection<User> Subscribers { get; protected set; } = new HashSet<User>();
    public virtual ICollection<Template> Templates { get; protected set; } = new HashSet<Template>();

    public virtual ICollection<User> Participants { get; protected set; } = new HashSet<User>();
    
    public Tricount() { }
    public Tricount(string title,string description, DateTime createdAt, User creator) {
        Title = title; 
        Description = description;
        CreatedAt = createdAt;
        Creator = creator;

    }
}

