using PRBD_Framework; // Importation de la bibliothèque PRBD_Framework
using System.ComponentModel.DataAnnotations.Schema; // Importation de System.ComponentModel.DataAnnotations.Schema pour utiliser les attributs de base de données

namespace prbd_2324_a06.Model // Déclaration de l'espace de noms tricount.Model
{
    public class Subscription : EntityBase<PridContext> // Définition de la classe Subscription qui hérite de EntityBase<PridContext>
    {
        // Constructeur pour initialiser une souscription avec les ID d'utilisateur et de Tricount spécifiés
        public Subscription(int userId, int tricountId) {
            UserId = userId; // Initialisation de l'ID de l'utilisateur associé à la souscription
            TricountId = tricountId; // Initialisation de l'ID du Tricount associé à la souscription
        }
        
        // Constructeur par défaut
        public Subscription(){}

        [ForeignKey(nameof(User))] // Clé étrangère vers l'utilisateur associé
        public int UserId { get; set; } // Propriété représentant l'ID de l'utilisateur associé
        public virtual User User { get; set; } // Propriété de navigation vers l'utilisateur associé

        [ForeignKey(nameof(Tricount))] // Clé étrangère vers le Tricount associé
        public int TricountId { get; set; } // Propriété représentant l'ID du Tricount associé
        public virtual Tricount Tricount { get; set; } // Propriété de navigation vers le Tricount associé
    }
}
