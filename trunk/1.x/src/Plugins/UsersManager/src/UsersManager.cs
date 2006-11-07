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
using Niry.Network;
using Niry.GUI.Gtk2;

using NyFolder;
using NyFolder.GUI;
using NyFolder.Utils;
using NyFolder.Protocol;
using NyFolder.GUI.Base;
using NyFolder.GUI.Glue;
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
			this.nyFolder.MainWindowStarted += new BlankEventHandler(OnMainWindowStart);
			this.nyFolder.MainWindowClosed += new BlankEventHandler(OnMainWindowClose);
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

		private void OnMainWindowStart (object sender) {
			GUI.Window window = sender as GUI.Window;
			NetworkViewer networkViewer = window.NotebookViewer.NetworkViewer;

			NetworkManager.UserAccept += new AcceptUserHandler(OnAcceptUser);
			networkViewer.UserLoggedIn += new PeerSelectedHandler(OnUserLoggedIn);
		}

		private void OnMainWindowClose (object sender) {
			GUI.Window window = sender as GUI.Window;
			NetworkViewer networkViewer = window.NotebookViewer.NetworkViewer;

			NetworkManager.UserAccept -= new AcceptUserHandler(OnAcceptUser);
			networkViewer.UserLoggedIn -= new PeerSelectedHandler(OnUserLoggedIn);
		}

		// ============================================
		// PRIVATE (Methods) Event Handlers 
		// ============================================
		private void OnUserEntryLostFocus (object obj, FocusOutEventArgs args) {
			GUI.Dialogs.Login login = this.nyFolder.LoginDialog;

			Accounts accounts = new Accounts();
			login.Password = accounts.GetUserPassword(login.Username);
			accounts.Dispose();
		}

		private void OnUserLoggedIn (object sender, UserInfo userInfo) {
			Users usersDb = new Users();
			if (usersDb.GetUserId(userInfo.Name) < 0) {
				usersDb.Insert(userInfo.Name, true);
			}
			usersDb.Dispose();
		}

		private AcceptUserType OnAcceptUser (PeerSocket peer, UserInfo userInfo) {
			if (userInfo.SecureAuthentication == false)
				return(AcceptUserType.Ask);

			Users usersDb = new Users();

			// If User isn't into DB
			if (usersDb.GetUserId(userInfo.Name) < 0) {
				usersDb.Dispose();
				return(AcceptUserType.Ask);
			}

			// Get User Accept Status
			bool acceptUser = usersDb.GetAccept(userInfo.Name);

			usersDb.Dispose();
			return(acceptUser ? AcceptUserType.Yes : AcceptUserType.No);
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
