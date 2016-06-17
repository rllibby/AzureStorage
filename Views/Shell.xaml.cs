/*
 *  Copyright © 2016, Russell Libby
 */

using Template10.Controls;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Controls;

namespace AzureStorage.Views
{
    /// <summary>
    /// Shell page.
    /// </summary>
    public sealed partial class Shell : Page
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public Shell()
        {
            Instance = this;
            InitializeComponent();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="navigationService">The navigation service to initialize with.</param>
        public Shell(INavigationService navigationService) : this()
        {
            SetNavigationService(navigationService);
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Sets the navigation service.
        /// </summary>
        /// <param name="navigationService">The navigation service to set.</param>
        public void SetNavigationService(INavigationService navigationService)
        {
            MyHamburgerMenu.NavigationService = navigationService;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// The static instance.
        /// </summary>
        public static Shell Instance { get; set; }

        /// <summary>
        /// The static hamburger menu.
        /// </summary>
        public static HamburgerMenu HamburgerMenu => Instance.MyHamburgerMenu;

        #endregion
    }
}

