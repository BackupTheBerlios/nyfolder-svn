/* [ Plugins/UsersManager.cs ] NyFolder Users Manager Plugin
 * Author: Matteo Bertozzi
 * ============================================================================
 * This file is part of NyFolder.
 *
 * NyFolder is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 * 
 * NyFolder is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with NyFolder; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
 */

using Gtk;

using System;

using Niry;
using Niry.Utils;
using Niry.GUI.Gtk2;

using NyFolder;
using NyFolder.Utils;
using NyFolder.GUI.Base;
using NyFolder.PluginLib;

namespace NyFolder.Plugins.UsersManager {
	/// Users Manager (Plugin)
	public class UsersManager : Plugin {
		// ============================================
		// PUBLIC Events
		// ============================================

		// ============================================
		// PRIVATE Members
		// ============================================
		private INyFolder nyFolder = null;

		// ============================================
		// PUBLIC Constructors
		// ============================================
		/// Create New Users Manager
		public UsersManager() {
			// Initialize Plugin Info
			this.pluginInfo = new Info();
		}

		// ============================================
		// PUBLIC Methods
		// ============================================
		/// Initialize Users Manager Plugin
		public override void Initialize (INyFolder iNyFolder) {
			this.nyFolder = iNyFolder;

			// Initialize GUI Events
			this.nyFolder.UserLogin += new BlankEventHandler(OnUserLogin);
			this.nyFolder.LoginDialogStarted += new BlankEventHandler(OnLoginDialogStart);
			this.nyFolder.LoginDialogClosed += new BlankEventHandler(OnLoginDialogClose);
		}

		// ============================================
		// PRIVATE (Methods) Event Handlers
		// ============================================
		private void OnUserLogin (object sender) {
			GUI.Dialogs.Login login = this.nyFolder.LoginDialog;
			if (login.RememberPassword && login.SecureAuthentication) {
				SaveAccount(login);
			}
		}

		private void OnLoginDialogStart (object sender) {
			GUI.Dialogs.Login login = sender as GUI.Dialogs.Login;

			// Fill User Entry Completion
			login.UserNameCompletion.Model = CreateUserNameCompletion();
			login.UserNameCompletion.TextColumn = 0;

			// Add Event Handler When User Entry Lost Focus
			login.UserFocusOut += new FocusOutEventHandler(OnUserEntryLostFocus);
		}

		private void OnLoginDialogClose (object sender) {
			GUI.Dialogs.Login login = sender as GUI.Dialogs.Login;
			login.UserFocusOut -= new FocusOutEventHandler(OnUserEntryLostFocus);
		}

		// ============================================
		// PRIVATE (Methods) Event Handlers 
		// ============================================
		private void OnUserEntryLostFocus (object o, FocusOutEventArgs args) {
			GUI.Dialogs.Login login = this.nyFolder.LoginDialog;

			Accounts accounts = new Accounts();
			login.Password = accounts.GetUserPassword(login.Username);
			accounts.Dispose();
		}

		// ============================================
		// PRIVATE Methods
		// ============================================
		private void SaveAccount (GUI.Dialogs.Login login) {
			Accounts accounts = new Accounts();
			accounts.Insert(login.Username, login.Password);
			accounts.Dispose();
		}

		private TreeModel CreateUserNameCompletion() {
			Accounts accounts = new Accounts();
			string[] accNames = accounts.GetAllAccounts();
			accounts.Dispose();

			// No Account Saved
			if (accNames == null) return(null);

			// Add Account
			ListStore store = new ListStore(typeof(string));
			foreach (string name in accNames)
				store.AppendValues(name);
			return(store);
		}

		// ============================================
		// PUBLIC Properties
		// ============================================
	}
}
