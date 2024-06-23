using PRBD_Framework; // Importation de la bibliothèque PRBD_Framework
using
    System.ComponentModel.DataAnnotations; // Importation de System.ComponentModel.DataAnnotations pour utiliser les annotations de validation
using
    System.ComponentModel.DataAnnotations.Schema; // Importation de System.ComponentModel.DataAnnotations.Schema pour utiliser les attributs de base de données
using System.Globalization; // Importation de System.Globalization pour manipuler la culture
using System.Windows.Documents; // Importation de System.Windows.Documents pour manipuler les documents Windows

namespace prbd_2324_a06.Model // Déclaration de l'espace de noms prbd_2324_a06.Model
{
    public class
        Operation : EntityBase<PridContext> // Définition de la classe Operation qui hérite de EntityBase<PridContext>
    {
        // Constructeur prenant plusieurs paramètres pour initialiser une opération
        public Operation(string title, int tricountId, double amount, DateTime operationDate, int initiatorId) {
            Id = GetHighestOperationId(); // Initialisation de l'ID en appelant GetHighestOperationId()
            Title = title; // Initialisation du titre
            TricountId = tricountId; // Initialisation de l'ID du Tricount associé
            Amount = amount; // Initialisation du montant de l'opération
            OperationDate = operationDate; // Initialisation de la date de l'opération
            InitiatorId = initiatorId; // Initialisation de l'ID de l'initiateur de l'opération
        }

        // Constructeur pour créer une nouvelle opération avec un ID de Tricount spécifié
        public Operation(int tricountId) {
            Id = GetHighestOperationId(); // Initialisation de l'ID en appelant GetHighestOperationId()
            TricountId = tricountId; // Initialisation de l'ID du Tricount associé
        }

        // Constructeur par défaut
        public Operation() {
        }

        [Key] // Définition de la clé primaire
        public int Id { get; set; } // Propriété représentant l'ID de l'opération

        public string Title { get; set; } // Propriété représentant le titre de l'opération

        [ForeignKey(nameof(Tricount))] // Clé étrangère vers Tricount
        public int TricountId { get; set; } // Propriété représentant l'ID du Tricount associé

        public virtual Tricount Tricount { get; set; } // Propriété de navigation vers le Tricount associé

        [Required] // Annotation indiquant que la propriété est requise
        public double Amount { get; set; } // Propriété représentant le montant de l'opération

        [Required] // Annotation indiquant que la propriété est requise
        public DateTime OperationDate { get; set; } // Propriété représentant la date de l'opération

        [ForeignKey(nameof(Initiator))] // Clé étrangère vers l'initiateur de l'opération
        public int InitiatorId { get; set; } // Propriété représentant l'ID de l'initiateur de l'opération

        public virtual User Initiator { get; set; } // Propriété de navigation vers l'initiateur de l'opération

        // Collection de répartitions associées à cette opération
        public virtual ICollection<Repartition> Repartitions { get; protected set; } = new HashSet<Repartition>();

        // Méthode pour obtenir le plus grand ID d'opération
        public int GetHighestOperationId() {
            return Context.Operations.Max(o => o.Id) + 1; // Retourne le plus grand ID d'opération dans le contexte + 1
        }

        public static Operation GetOperationById(int id) {
            return Context.Operations.FirstOrDefault(o => o.Id == id);
        }

        // Méthode pour obtenir les répartitions associées à cette opération
        public IQueryable<Repartition> GetRepartitionByOperation() {
            var q = from r in Context.Repartitions // Requête LINQ pour récupérer les répartitions
                where r.OperationId == Id // Filtre pour les répartitions associées à cette opération
                select r;
            return q; // Retourne la requête
        }

        // Méthode pour supprimer l'opération
        public void Delete() {
            Repartitions.Clear(); // Supprime toutes les répartitions associées à cette opération
            Context.Operations.Remove(this); // Supprime cette opération du contexte
            Context.SaveChanges(); // Enregistre les changements dans la base de données
        }
    }
}