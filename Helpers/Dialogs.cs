/*
 *  Copyright © 2016, Russell Libby 
 */

using System;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace AzureStorage.Helpers
{
    /// <summary>
    /// Static class to simplify dialog handling.
    /// </summary>
    public static class Dialogs
    {
        #region Private constants

        private const string Ok = "OK";

        #endregion

        #region Private methods

        /// <summary>
        /// Command handlers for the ok dialog.
        /// </summary>
        /// <param name="commandLabel">The command selected by the user.</param>
        private static void CommandOk(IUICommand commandLabel) { }

        #endregion

        #region Public methods

        /// <summary>
        /// Shows a dialog to the user.
        /// </summary>
        /// <param name="title">The title for the dialog.</param>
        /// <param name="message">The message to display in the dialog.</param>
        /// <returns></returns>
        public static async Task Show(string title, string message)
        {
            var dialog = new MessageDialog(message, title);

            dialog.Commands.Add(new UICommand(Ok, CommandOk));

            await dialog.ShowAsync().AsTask();
        }

        #endregion
    }
}
