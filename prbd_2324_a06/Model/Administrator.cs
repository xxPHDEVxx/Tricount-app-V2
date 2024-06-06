using PRBD_Framework;

namespace prbd_2324_a06.Model
{
    public class Administrator : User {
        protected Administrator() {
            Role = Role.Administrator;
        }

        public Administrator(int userId, string mail, string hashed_password, string full_name)
            : base(mail, hashed_password, full_name) {
            Role = Role.Administrator;
        }
    }
}