using PRBD_Framework; // Importation de la bibliothèque PRBD_Framework
using System.ComponentModel.DataAnnotations.Schema; // Importation de System.ComponentModel.DataAnnotations.Schema pour utiliser les attributs de base de données

namespace prbd_2324_a06.Model // Déclaration de l'espace de noms prbd_2324_a06.Model
{
    public class TemplateItem : EntityBase<PridContext> // Définition de la classe TemplateItem qui hérite de EntityBase<PridContext>
    {
        [ForeignKey(nameof(User))] // Clé étrangère vers l'utilisateur associé
        public int UserId { get; set; } // Propriété représentant l'ID de l'utilisateur associé
        public virtual User User { get; set; } // Propriété de navigation vers l'utilisateur associé

        [ForeignKey(nameof(Template))] // Clé étrangère vers le modèle de Tricount associé
        public int TemplateId { get; set; } // Propriété représentant l'ID du modèle de Tricount associé
        public virtual Template Template { get; set; } // Propriété de navigation vers le modèle de Tricount associé

        public int Weight { get; set; } // Propriété représentant le poids de l'élément de modèle
    }
}