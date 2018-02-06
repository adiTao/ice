using System.Collections.ObjectModel;

namespace Demo
{
    public static class UserUtils
    {
        public static UserList OnlineUser = new UserList();
    }

    public class User
    {
        public User(string name, UserCallBackPrx cp)
        {
            _name = name;
            _Cp = cp;
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public UserCallBackPrx Cp
        {
            get
            {
                return _Cp;
            }
            set
            {
                _Cp = value;
            }
        }

        public override int GetHashCode()
        {
            return -1; // Not needed
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (this.GetType() != obj.GetType())
            {
                return false;
            }
            User user = (User)obj;
            return user.Name == Name;
        }

        private string _name;
        private UserCallBackPrx _Cp;
    }

    //
    // Extends ObservableCollection to store the list of chat users.
    //
    // We need to use ObservableCollection so the view is updated when the list contents change.
    //
    // Also note that updates to the list need to be done from the UI thread
    // for ObservableCollection to work correctly.
    //
    public class UserList : ObservableCollection<User>
    {
        public UserList()
        {
        }
    }
}
