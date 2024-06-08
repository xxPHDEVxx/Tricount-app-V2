using PRBD_Framework; // Importation de la bibliothèque PRBD_Framework
using
    System.ComponentModel.DataAnnotations; // Importation de System.ComponentModel.DataAnnotations pour utiliser les annotations de validation
using
    System.ComponentModel.DataAnnotations.Schema; // Importation de System.ComponentModel.DataAnnotations.Schema pour utiliser les attributs de base de données

namespace prbd_2324_a06.Model // Déclaration de l'espace de noms prbd_2324_a06.Model
{
    public class
        Tricount : EntityBase<PridContext> // Définition de la classe Tricount qui hérite de EntityBase<PridContext>
    {
        [Key] // Définition de la clé primaire
        public int Id { get; set; } // Propriété représentant l'ID du Tricount

        public string Title { get; set; } // Propriété représentant le titre du Tricount

        public string Description { get; set; } // Propriété représentant la description du Tricount

        public DateTime CreatedAt { get; set; } // Propriété représentant la date de création du Tricount

        [Required,
         ForeignKey(nameof(Creator))] // Annotation indiquant que le champ est requis et définition de la clé étrangère vers l'utilisateur créateur
        public int CreatorId { get; set; } // Propriété représentant l'ID de l'utilisateur créateur du Tricount

        public virtual User Creator { get; set; } // Propriété de navigation vers l'utilisateur créateur du Tricount

        public virtual ICollection<Subscription> Subscriptions { get; protected set; } =
            new HashSet<Subscription>(); // Collection de souscriptions associées à ce Tricount

        public virtual ICollection<Template> Templates { get; protected set; } =
            new HashSet<Template>(); // Collection de modèles associés à ce Tricount

        public Tricount() { } // Constructeur par défaut

        // Constructeur pour initialiser un Tricount avec un titre, une description, une date de création et un créateur spécifiés
        public Tricount(string title, string description, DateTime createdAt, User creator) {
            Id = GetHighestTricountId(); // Initialisation de l'ID en appelant GetHighestTricountId()
            Title = title; // Initialisation du titre
            Description = description; // Initialisation de la description
            CreatedAt = createdAt; // Initialisation de la date de création
            Creator = creator; // Initialisation de l'utilisateur créateur
        }

        // Méthode pour récupérer le nom du créateur du Tricount
        public string GetCreatorName() {
            return
                User.GetUserNameById(
                    CreatorId); // Appel de la méthode GetUserNameById() de la classe User pour récupérer le nom du créateur
        }

        // Méthode pour obtenir le plus grand ID de Tricount
        public int GetHighestTricountId() {
            return Context.Tricounts.Max(o => o.Id) + 1; // Retourne le plus grand ID de Tricount dans le contexte + 1
        }

        // Méthode pour obtenir le nombre de participants (hors créateur) associés à ce Tricount
        public int NumberOfParticipants() {
            var q = (from s in Subscriptions // Requête LINQ pour compter les souscriptions
                where s.UserId != CreatorId // Filtre pour exclure le créateur
                select s).Count();
            return q; // Retourne le nombre de participants
        }

        // Méthode pour obtenir les opérations associées à ce Tricount
        public IQueryable<Operation> GetOperations() {
            var q = from o in Context.Operations // Requête LINQ pour récupérer les opérations
                where o.TricountId == Id // Filtre pour les opérations associées à ce Tricount
                select o;
            return q; // Retourne la requête
        }

        // Méthode pour obtenir le total des montants des opérations associées à ce Tricount
        public double GetTotal() {
            var total = Context.Operations // Requête LINQ pour calculer le total des montants des opérations
                .Where(o => o.TricountId == Id) // Filtre pour les opérations associées à ce Tricount
                .Sum(o => Math.Round(o.Amount, 2)); // Somme des montants arrondis à deux décimales
            return total; // Retourne le total
        }

        // Méthode pour obtenir les participants associés à ce Tricount
        public IQueryable<User> GetParticipants() {
            var userIds =
                Subscriptions // Sélectionne les ID d'utilisateur à partir des souscriptions associées à ce Tricount
                    .Where(sub => sub.TricountId == Id)
                    .Select(sub => sub.UserId)
                    .ToList();

            var participants = Context.Users // Sélectionne les utilisateurs à partir des ID récupérés
                .Where(user => userIds.Contains(user.UserId))
                .OrderBy(user => user.FullName); // Trie les participants par ordre alphabétique

            return participants; // Retourne les participants
        }

        // Méthode pour obtenir les modèles associés à ce Tricount
        public IQueryable<Template> GetTemplates() {
            var id = Templates // Sélectionne les ID de modèle à partir des modèles associés à ce Tricount
                .Where(t => t.TricountId == Id)
                .Select(t => t.Id);

            var templates = Context.Templates // Sélectionne les modèles à partir des ID récupérés
                .Where(t => id.Contains(t.Id));
            return templates; // Retourne les modèles
        }

        // Méthode pour obtenir un modèle par titre
        public Template GetTemplateByTitle(string title) {
            return
                Templates.FirstOrDefault(t =>
                    t.Title == title); // Retourne le premier modèle avec le titre spécifié, ou null s'il n'y en a aucun
        }

        // Méthode pour valider le Tricount
        public override bool Validate() {
            ClearErrors(); // Efface les erreurs de validation précédentes

            if (string.IsNullOrWhiteSpace(Title)) // Vérifie si le titre est vide ou null
                AddError(nameof(Title), "required"); // Ajoute une erreur si le titre est requis
            else if (Title.Length < 3) // Vérifie si le titre a une longueur inférieure à 3 caractères
                AddError(nameof(Title), "length must be >= 3");

            // Validation de la description si elle contient quelque chose
            if (!string.IsNullOrWhiteSpace(Description) && Description.Length < 3) {
                AddError(nameof(Description), "length must be >= 3, ");
            }

            return !HasErrors; // Retourne vrai si aucune erreur n'est présente, sinon retourne faux
        }

        // récupere la derniere opération
        public Operation GetLatestOperation() {
            return Context.Operations
                .Where(o => o.TricountId == Id)
                .OrderByDescending(o => o.OperationDate)
                .FirstOrDefault();
        }
        
        // Delete in Cascade from Database
        public void Delete() {
            Subscriptions.Clear();
            foreach (var template in Templates) {
                template.delete();
            }
            Context.Tricounts.Remove(this);
            Context.SaveChanges();
        }
    }
}