/*
 *  Copyright © 2016, Russell Libby 
 */

using System.Collections.ObjectModel;
using System.Linq;
using Template10.Mvvm;

namespace AzureStorage.Models
{
    /// <summary>
    /// Model for handling recent items.
    /// </summary>
    public class RecentModel : BindableBase
    {
        #region Private fields

        private static ObservableCollection<AccountResourceModel> _resources = new ObservableCollection<AccountResourceModel>();
        private static RecentModel _instance = new RecentModel();

        #endregion

        #region Private methods

        /// <summary>
        /// Locates the instance that matches the equality test of the passed item.
        /// </summary>
        /// <param name="item">The item to locate.</param>
        /// <returns>The index of the item if found, (-1) if not found.</returns>
        private int GetIndexOf(AccountResourceModel item)
        {
            if (item == null) return (-1);

            for (var i = 0; i < _resources.Count; i++)
            {
                if (AccountResourceModel.Equals(_resources[i], item)) return i;
            }

            return (-1);
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Removes the specified resource from the recent list.
        /// </summary>
        /// <param name="item">The account resource item to remove.</param>
        public void Remove(AccountResourceModel item)
        {
            for (var i = 0; i < _resources.Count; i++)
            {
                if (AccountResourceModel.Equals(_resources[i], item))
                {
                    _resources.RemoveAt(i);
                    break;
                }
            }
        }

        /// <summary>
        /// Removes the specified resource container from the recent list.
        /// </summary>
        /// <param name="account">The account that the resource exists in.</param>
        public void Remove(AccountModel account)
        {
            if (account == null) return;

            var select = _resources.Where(ar => ar.Account.Equals(account)).ToList();
            
            foreach (var item in select)
            {
                _resources.Remove(item);
            }
        }

        /// <summary>
        /// Removes the specified resource container from the recent list.
        /// </summary>
        /// <param name="account">The account that the resource exists in.</param>
        /// <param name="item">The resource container item to remove.</param>
        public void Remove(AccountModel account, ResourceContainerModel item)
        {
            if ((account == null) || (item == null)) return;

            var recentItem = new AccountResourceModel(account, item.Name, item.ResourceType);

            for (var i = 0; i < _resources.Count; i++)
            {
                if (AccountResourceModel.Equals(_resources[i], recentItem))
                {
                    _resources.RemoveAt(i);
                    break;
                }
            }
        }

        /// <summary>
        /// Adds a new resource to the recent list.
        /// </summary>
        /// <param name="item">The account resource item to add.</param>
        public void Add(AccountResourceModel item)
        {
            if (item == null) return;

            var index = GetIndexOf(item);

            if (index < 0)
            {
                _resources.Insert(0, item);

                while (_resources.Count > 10) _resources.RemoveAt(_resources.Count - 1);

                return;
            }

            _resources.Move(index, 0);
        }

        #endregion

        #region Public properties

        /// <summary>
        /// The static instance.
        /// </summary>
        public static RecentModel Instance
        {
            get { return _instance; }
        }

        /// <summary>
        /// The collection of resources.
        /// </summary>
        public ObservableCollection<AccountResourceModel> Resources
        {
            get { return _resources; }
        }

        #endregion
    }
}
