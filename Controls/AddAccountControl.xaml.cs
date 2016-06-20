/*
 *  Copyright © 2016, Russell Libby 
 */

using AzureStorage.Models;
using System;
using Template10.Mvvm;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace AzureStorage.Controls
{
    /// <summary>
    /// The dialog for adding a new storage account.
    /// </summary>
    public sealed partial class AddAccountControl : UserControl
    {
        #region Private properties

        private readonly AccountModel _account = new AccountModel();

        #endregion

        #region Private methods

        /// <summary>
        /// Event that is triggered when text content changes.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The routed event arguments.</param>
        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender.Equals(nameText)) _account.AccountName = nameText.Text;
            if (sender.Equals(keyText)) _account.AccountKey = keyText.Text;
        }

        /// <summary>
        /// Event that is triggered when add is clicked.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void AddClicked(object sender, EventArgs e)
        {
            OnAdd?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Event that is triggered when the close button is clicked.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The routed event arguments.</param>
        private void CloseClicked(object sender, RoutedEventArgs e)
        {
            OnCancel?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Event that is fired when the control is loaded.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e"></param>
        private void OnControlLoaded(object sender, RoutedEventArgs e)
        {
            nameText.Focus(FocusState.Programmatic);
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public AddAccountControl()
        {
            InitializeComponent();

            Loaded += OnControlLoaded;
            _account.OnAdd += AddClicked;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Event handler to notify when cancelled.
        /// </summary>
        public event EventHandler OnCancel;

        /// <summary>
        /// Event handler to notify when added.
        /// </summary>
        public event EventHandler OnAdd;

        /// <summary>
        /// The account model.
        /// </summary>
        public AccountModel Account
        {
            get { return _account; }
        }

        #endregion
    }
}
