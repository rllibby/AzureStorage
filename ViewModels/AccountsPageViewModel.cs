/*
 *  Copyright © 2016, Russell Libby 
 */

using AzureStorage.Models;
using Template10.Mvvm;

namespace AzureStorage.ViewModels
{
    /// <summary>
    /// View model for Accounts page.
    /// </summary>
    class AccountsPageViewModel : ViewModelBase
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public AccountsPageViewModel()
        {
        }

        #endregion

        #region Public properties

        /// <summary>
        /// The collection of accounts.
        /// </summary>
        public AccountsModel Accounts
        {
            get { return AccountsModel.Instance; }
        }

        #endregion
    }
}
