using MKW.Core;
using MKW.Core.Storage;
using MKW.GUI.Model;
using System.Collections.ObjectModel;

namespace MKW.GUI
{
    public class TrustedUsersCollectionViewModel : ObservableCollection<TrustedUserItemViewModel>
    {
        private readonly DatabaseModel database;
        private readonly UserId userId;

        public TrustedUsersCollectionViewModel(DatabaseModel database, UserId userId)
        {
            this.database = database;
            this.userId = userId;
            Refresh();
        }

        private void Refresh()
        {
            Clear();

            foreach (UserInfo user in database.Client.EnumerateUsersTrust(userId))
            {
                if (user.Id != userId)
                {
                    Add(new TrustedUserItemViewModel(user));
                }
            }
        }
    }
}
