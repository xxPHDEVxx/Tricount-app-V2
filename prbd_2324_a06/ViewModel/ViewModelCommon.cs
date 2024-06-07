using prbd_2324_a06.Model;
using PRBD_Framework;

namespace prbd_2324_a06.ViewModel
{
    public class ViewModelCommon : ViewModelBase<User, PridContext>
    {
        public static bool IsAdmin => App.IsLoggedIn && App.CurrentUser is Administrator;

        public static bool IsNotAdmin => !IsAdmin;
    }
}
