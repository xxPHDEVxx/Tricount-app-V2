using PRBD_Framework; // Importation de la bibliothèque PRBD_Framework
using
    System.ComponentModel.DataAnnotations; // Importation de System.ComponentModel.DataAnnotations pour utiliser les annotations de validation

namespace prbd_2324_a06.Model // Déclaration de l'espace de noms prbd_2324_a06.Model
{
    // Définition de l'énumération Role représentant les rôles d'utilisateur
    public enum Role
    {
        User = 0, // Utilisateur standard
        Administrator = 1 // Administrateur
    }

    // Définition de la classe User qui hérite de EntityBase<PridContext>
    public class User : EntityBase<PridContext>
    {
        [Key] // Définition de la clé primaire
        public int UserId { get; set; } // Propriété représentant l'ID de l'utilisateur

        public string Mail { get; set; } // Propriété représentant l'adresse e-mail de l'utilisateur

        public string HashedPassword { get; set; } // Propriété représentant le mot de passe haché de l'utilisateur

        public string FullName { get; set; } // Propriété représentant le nom complet de l'utilisateur

        public Role Role { get; protected internal set; } =
            Role.User; // Propriété représentant le rôle de l'utilisateur, par défaut c'est un utilisateur standard

        public User() { } // Constructeur par défaut

        // Constructeur pour créer un utilisateur avec auto-incrémentation pour l'ID lors de l'inscription
        public User(string mail, string hashed_password, string full_name) {
            UserId = GetHighestUserId() + 1; // Auto-incrémentation de l'ID en appelant GetHighestUserId()
            Mail = mail; // Initialisation de l'adresse e-mail
            HashedPassword = hashed_password; // Initialisation du mot de passe haché
            FullName = full_name; // Initialisation du nom complet
        }

        // Constructeur pour créer un utilisateur avec un ID spécifié
        public User(int userdId, string mail, string hashed_password, string full_name) {
            UserId = userdId; // Initialisation de l'ID avec la valeur spécifiée
            Mail = mail; // Initialisation de l'adresse e-mail
            HashedPassword = hashed_password; // Initialisation du mot de passe haché
            FullName = full_name; // Initialisation du nom complet
        }

        // Méthode pour obtenir le plus grand ID d'utilisateur
        public int GetHighestUserId() {
            return Context.Users.Max(u => u.UserId); // Retourne le plus grand ID d'utilisateur dans le contexte
        }

        // Méthode pour obtenir un utilisateur par son nom
        public static User GetUserByName(string name) {
            return
                Context.Users.FirstOrDefault(u =>
                    u.FullName ==
                    name); // Retourne le premier utilisateur avec le nom spécifié, ou null s'il n'y en a aucun
        }

        // Méthode pour obtenir un utilisateur par son id
        public static User GetUserById(int id) {
            return
                Context.Users.FirstOrDefault(u =>
                    u.UserId ==
                    id); // Retourne le premier utilisateur avec le nom spécifié, ou null s'il n'y en a aucun
        }

        // Collection de souscriptions associées à cet utilisateur
        public virtual ICollection<Subscription> Subscriptions { get; protected set; } = new HashSet<Subscription>();

        // Collection de répartitions associées à cet utilisateur
        public virtual ICollection<Repartition> Repartitions { get; protected set; } = new HashSet<Repartition>();

        // Méthode pour obtenir les Tricounts créés par cet utilisateur
        public IQueryable<Tricount> GetTricounts() {
            var tricounts = from t in Context.Tricounts
                            where t.CreatorId == UserId
                            orderby t.CreatedAt descending
                            select t;
            return tricounts; // Retourne la liste des Tricounts créés par cet utilisateur
        }

        // Méthode pour obtenir les Tricounts auxquels cet utilisateur participe
        public IQueryable<Tricount> GetParticipatedTricounts() {
            var participatedTricounts = from s in Context.Subscriptions
                                        join t in Context.Tricounts on s.TricountId equals t.Id
                                        where s.UserId == UserId
                                        orderby t.CreatedAt descending
                                        select t;
            return participatedTricounts; // Retourne la liste des Tricounts auxquels cet utilisateur participe
        }

        // Méthode pour obtenir tous les Tricounts
        public IQueryable<Tricount> GetAll() {
            var tricounts = from t in Context.Tricounts
                            orderby t.CreatedAt descending
                            select t;
            return tricounts; // Retourne la liste de tous les Tricounts
        }

        public IQueryable<Tricount> GetFiltered(string Filter) {
            var filtered = from t in GetTricounts().Union(GetParticipatedTricounts())
                           join o in Context.Operations on t.Id equals o.TricountId
                           join s in Context.Subscriptions on t.Id equals s.TricountId
                           where t.Title.ToLower().Contains(Filter) || t.Creator.FullName.ToLower().Contains(Filter) || t.Description.ToLower().Contains(Filter)
                           || o.Title.ToLower().Contains(Filter) || s.User.FullName.ToLower().Contains(Filter)
                           orderby t.Title
                           select t;
            return filtered.Distinct();
        }

        // Méthode pour obtenir tous les Tricounts filtrés par un terme de recherche
        public IQueryable<Tricount> GetAllFiltered(string Filter) {
            var filtered = from t in Context.Tricounts
                           join o in Context.Operations on t.Id equals o.TricountId
                           join s in Context.Subscriptions on t.Id equals s.TricountId
                           where t.Title.ToLower().Contains(Filter) || t.Creator.FullName.ToLower().Contains(Filter) || t.Description.ToLower().Contains(Filter)
                            || o.Title.ToLower().Contains(Filter) || s.User.FullName.ToLower().Contains(Filter)
                           select t;
            return filtered.Distinct(); // Retourne la liste des Tricounts filtrés
        }

        // Méthode pour obtenir le nom d'utilisateur à partir de son ID
        public static string GetUserNameById(int userId) {
            var u = Context.Users.SingleOrDefault(u => u.UserId == userId);
            return u.FullName; // Retourne le nom complet de l'utilisateur avec l'ID spécifié
        }

        // Méthode pour obtenir les dépenses de l'utilisateur dans un Tricount spécifié
        public double GetMyExpenses(Tricount tricount) {
            double myExpenses = 0;

            foreach (var operation in tricount.GetOperations()) {
                double operationWeight = 0;
                double userWeight = 0;

                foreach (var repartition in operation.GetRepartitionByOperation()) {
                    operationWeight += repartition.Weight;
                    if (repartition.UserId == UserId) {
                        userWeight = repartition.Weight;
                    }
                }

                if (operationWeight > 0) {
                    myExpenses += (operation.Amount / operationWeight) * userWeight;
                }
            }

            return Math.Round(myExpenses, 2); // Retourne les dépenses de l'utilisateur arrondies à deux décimales
        }

        // Méthode pour obtenir la balance de l'user
        public double GetMyBalance(Tricount tricount) {
            double myPaid = 0;
            double myExpenses = GetMyExpenses(tricount);

            foreach (var operation in tricount.GetOperations()) {
                if (operation.InitiatorId == UserId) {
                    myPaid += operation.Amount;
                }
            }

            // La balance est le total des crédits moins les dépenses
            return Math.Round(myPaid - myExpenses, 2);
        }
        public IQueryable<Operation> GetOperations(Tricount tricount) {
            var q = from o in tricount.GetOperations()
                    join r in Context.Repartitions on o.Id equals r.OperationId
                    where o.InitiatorId == UserId
                    select o;
            return q;
        }
    }
}
    
