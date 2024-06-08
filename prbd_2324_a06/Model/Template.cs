using PRBD_Framework; // Importation de la bibliothèque PRBD_Framework
using System.ComponentModel.DataAnnotations; // Importation de System.ComponentModel.DataAnnotations pour utiliser les annotations de validation
using System.ComponentModel.DataAnnotations.Schema; // Importation de System.ComponentModel.DataAnnotations.Schema pour utiliser les attributs de base de données

namespace prbd_2324_a06.Model // Déclaration de l'espace de noms prbd_2324_a06.Model
{
    public class Template : EntityBase<PridContext> // Définition de la classe Template qui hérite de EntityBase<PridContext>
    {
        [Key] // Définition de la clé primaire
        public int Id { get; set; } // Propriété représentant l'ID du modèle de Tricount

        public string Title { get; set; } // Propriété représentant le titre du modèle de Tricount

        [ForeignKey(nameof(Tricount))] // Clé étrangère vers le Tricount associé
        public int TricountId { get; set; } // Propriété représentant l'ID du Tricount associé
        public virtual Tricount Tricount { get; set; } // Propriété de navigation vers le Tricount associé

        public virtual ICollection<TemplateItem> TemplateItems { get; set; } = new HashSet<TemplateItem>(); // Collection d'éléments de modèle associés à ce modèle

        // Méthode pour récupérer un dictionnaire (paire de clé-valeur) avec l'utilisateur et son poids pour chaque répartition
        public Dictionary<string, int> GetUsersAndWeightsByTricountId() {
            return TemplateItems // Sélectionne les éléments de modèle
                .Where(ti => ti.TemplateId == Id) // Filtre pour les éléments de modèle associés à ce modèle
                .ToDictionary<TemplateItem, string, int>( // Convertit la liste d'éléments de modèle en dictionnaire
                    templateItem => templateItem.User.FullName, // Clé : Nom complet de l'utilisateur
                    templateItem => templateItem.Weight); // Valeur : Poids de l'élément de modèle
        }

        public void delete() {
            TemplateItems.Clear();
            Context.Templates.Remove(this);
        }
    }
}
