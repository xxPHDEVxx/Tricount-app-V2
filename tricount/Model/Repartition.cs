using PRBD_Framework; // Importation de la bibliothèque PRBD_Framework
using System.ComponentModel.DataAnnotations.Schema; // Importation de System.ComponentModel.DataAnnotations.Schema pour utiliser les attributs de base de données

namespace prbd_2324_a06.Model // Déclaration de l'espace de noms tricount.Model
{
    public class Repartition : EntityBase<PridContext> // Définition de la classe Repartition qui hérite de EntityBase<PridContext>
    {
        public Repartition() {
        }

        // Constructeur pour initialiser une répartition avec les ID d'opération, d'utilisateur et un poids spécifiés
        public Repartition(int operationId, int userId, int weight) {
            OperationId = operationId; // Initialisation de l'ID de l'opération associée
            UserId = userId; // Initialisation de l'ID de l'utilisateur associé
            Weight = weight; // Initialisation du poids de la répartition
        }

        [ForeignKey(nameof(Operation))] // Clé étrangère vers l'opération associée
        public int OperationId { get; set; } // Propriété représentant l'ID de l'opération associée
        public virtual Operation Operation { get; set; } // Propriété de navigation vers l'opération associée

        [ForeignKey(nameof(User))] // Clé étrangère vers l'utilisateur associé
        public int UserId { get; set; } // Propriété représentant l'ID de l'utilisateur associé
        public virtual User User { get; set; } // Propriété de navigation vers l'utilisateur associé

        public int Weight { get; set; } // Propriété représentant le poids de la répartition

        // Méthode pour obtenir le poids total pour un utilisateur et une opération spécifiés
        public int GetWeightForUserAndOperation(int userId, int operationId) {
            var q = Context.Repartitions // Requête LINQ pour récupérer les répartitions
                .Where(r => r.OperationId == OperationId) // Filtre pour les répartitions associées à l'opération spécifiée
                .Where(r => r.UserId == UserId) // Filtre pour les répartitions associées à l'utilisateur spécifié
                .Sum(r => r.Weight); // Somme des poids des répartitions correspondantes
            return q; // Retourne le poids total
        }
    }
}
