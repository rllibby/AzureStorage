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
    /// User control for adding a new resource.
    /// </summary>
    public sealed partial class AddResourceControl : UserControl
    {
        #region Private properties

        private readonly ResourceContainerModel _resource = new ResourceContainerModel();

        #endregion

        #region Private methods

        /// <summary>
        /// Event that is triggered when text content changes.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The routed event arguments.</param>
        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender.Equals(nameText)) _resource.Name = nameText.Text;
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
        public AddResourceControl()
        {
            InitializeComponent();

            Loaded += OnControlLoaded;
            _resource.OnAdd += AddClicked;
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
        public ResourceContainerModel Resource
        {
            get { return _resource; }
        }

        #endregion
    }
}
