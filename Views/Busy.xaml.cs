/*
 *  Copyright © 2016, Russell Libby 
 */

using Template10.Common;
using Template10.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace AzureStorage.Views
{
    /// <summary>
    /// Busy modal dialog control.
    /// </summary>
    public sealed partial class Busy : UserControl
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public Busy()
        {
            InitializeComponent();
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Hide or show the busy dialog box.
        /// </summary>
        /// <param name="busy">True to show, false to hide.</param>
        /// <param name="text">The text to display.</param>
        public static void SetBusy(bool busy, string text = null)
        {
            WindowWrapper.Current().Dispatcher.Dispatch(() =>
            {
                var modal = Window.Current.Content as ModalDialog;
                var view = modal.ModalContent as Busy;

                if (view == null) modal.ModalContent = view = new Busy();

                modal.IsModal = view.IsBusy = busy;
                view.BusyText = text;
            });
        }

        #endregion

        #region Public properties

        /// <summary>
        /// The text to display.
        /// </summary>
        public string BusyText
        {
            get { return (string)GetValue(BusyTextProperty); }
            set { SetValue(BusyTextProperty, value); }
        }

        /// <summary>
        /// BusyText dependency property.
        /// </summary>
        public static readonly DependencyProperty BusyTextProperty = DependencyProperty.Register(nameof(BusyText), typeof(string), typeof(Busy), new PropertyMetadata("Please wait..."));

        /// <summary>
        /// True if busy and being displayed.
        /// </summary>
        public bool IsBusy
        {
            get { return (bool)GetValue(IsBusyProperty); }
            set { SetValue(IsBusyProperty, value); }
        }

        /// <summary>
        /// Busy dependency property.
        /// </summary>
        public static readonly DependencyProperty IsBusyProperty = DependencyProperty.Register(nameof(IsBusy), typeof(bool), typeof(Busy), new PropertyMetadata(false));

        #endregion
    }
}

