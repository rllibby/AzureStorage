/*
 *  Copyright © 2016, Russell Libby 
 */

using AzureStorage.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Template10.Mvvm;
using Windows.UI.Xaml;

namespace AzureStorage.ViewModels
{
    /// <summary>
    /// The view model to handle account management.
    /// </summary>
    public class AccountsViewModel : ViewModelBase
    {
        #region Private fields

        private ObservableCollection<AccountModel> _accounts = new ObservableCollection<AccountModel>();
        private Services.SettingsServices.SettingsService _settings;
        private RelayCommand _deleteCommand;
        private RelayCommand _editCommand;
        private int _selected = (-1);

        #endregion

        #region Private methods

        /// <summary>
        /// Loads the list of accounts into the observable collection.
        /// </summary>
        /// <param name="accounts">The list of accounts to load.</param>
        private void LoadAccounts(IList<AccountModel> accounts)
        {
            if ((accounts == null) || (accounts.Count == 0)) return;

            foreach (var account in accounts)
            {
                account.DeleteCommand = _deleteCommand;
                account.EditCommand = _editCommand;

                _accounts.Add(account);
            }
        }

        /// <summary>
        /// Determines if the selected account can be modified or deleted.
        /// </summary>
        /// <returns></returns>
        private bool CanEditDelete()
        {
            return (_selected >= 0);
        }

        private void Edit()
        {

        }

        /// <summary>
        /// Deletes an existing account from the collection.
        /// </summary>
        private void Delete()
        {
            if (_selected < 0) return;

            var index = (_selected > 0) ? _selected - 1 : 0;

            _accounts.RemoveAt(_selected);

            if (_accounts.Count == 0)
            {
                SelectedIndex = (-1);
                return;
            }

            SelectedIndex = index;
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public AccountsViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled) return;

            _deleteCommand = new RelayCommand(new Action(Delete), CanEditDelete);
            _editCommand = new RelayCommand(new Action(Edit), CanEditDelete);
            _settings = Services.SettingsServices.SettingsService.Instance;

            LoadAccounts(_settings.Accounts);
        }

        #endregion

        #region Public methods

        public void Add()
        {
            var account = new AccountModel("sagepayroll")
            {
                DeleteCommand = _deleteCommand,
                EditCommand = _editCommand
            };

            _accounts.Add(account);

            SelectedIndex = (_accounts.Count - 1);
        }

        #endregion

        #region Public properties

        /// <summary>
        /// The collection of accounts.
        /// </summary>
        public ObservableCollection<AccountModel> Accounts
        {
            get { return _accounts; }
        }

        /// <summary>
        /// The selected account index.
        /// </summary>
        public int SelectedIndex
        {
            get { return _selected; }
            set
            {
                if (value == _selected) return;

                if ((_selected >= 0) && (_selected < _accounts.Count)) _accounts[_selected].IsSelected = false;

                _selected = value;

                if (SelectedIndex >= 0) _accounts[_selected].IsSelected = true;

                _deleteCommand.RaiseCanExecuteChanged();
                _editCommand.RaiseCanExecuteChanged();

                base.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// The relay command for handling account deletes.
        /// </summary>
        public RelayCommand DeleteCommand
        {
            get { return _deleteCommand; }
        }

        /// <summary>
        /// The relay command for handling account edits.
        /// </summary>
        public RelayCommand EditCommand
        {
            get { return _deleteCommand; }
        }

        #endregion
    }
}
