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
using System.Collections;

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

namespace NyFolder.Plugins.BuddyList {
	/// BuddyList (Plugin)
	public class BuddyList : Plugin {
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
		/// Create New BuddyList
		public BuddyList() {
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

			// Initialize Buddy List Component
			InitializeBuddyListMenu();

			// Network GUI Options
			NetworkViewer networkViewer = window.NotebookViewer.NetworkViewer;
			NetworkManager.UserAccept += new AcceptUserHandler(OnAcceptUser);
			networkViewer.RefreshPeers += new BlankEventHandler(OnNetViewerRefresh);
			networkViewer.UserLoggedIn += new PeerSelectedHandler(OnUserLoggedIn);
		}

		private void OnMainWindowClose (object sender) {
			GUI.Window window = sender as GUI.Window;
			NetworkViewer networkViewer = window.NotebookViewer.NetworkViewer;

			NetworkManager.UserAccept -= new AcceptUserHandler(OnAcceptUser);
			networkViewer.RefreshPeers -= new BlankEventHandler(OnNetViewerRefresh);
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
			if (userInfo.SecureAuthentication == false)
				return;

			// Add Only Secure Auth User
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
		
		// When Network Viewer is Refreshed
		private void OnNetViewerRefresh (object sender) {
			NetworkViewer networkViewer = sender as NetworkViewer;

			if (ShowOfflineBuddies == true) {
				// Load All Offline Buddies
				networkViewer.Store.Add(GetOfflineUsers());
			}
		}

		private void OnShowOfflineBuddies (object sender, EventArgs args) {
			Gtk.Application.Invoke(delegate {
				NetworkViewer networkViewer = nyFolder.MainWindow.NotebookViewer.NetworkViewer;
				ToggleAction action = sender as ToggleAction;

				if (action.Active == true) {
					// Load All Offline Buddies
					networkViewer.Store.Add(GetOfflineUsers());
				} else {
					// Remove All Offline Buddies
					networkViewer.Store.RemoveAllOffline();
				}
			});
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

		private UserInfo[] GetOfflineUsers() {
			Users usersDb = new Users();
			string[] users = usersDb.GetAllAcceptedUsers();
			usersDb.Dispose();

			ArrayList offlineUsers = new ArrayList();
			foreach (string username in users) {
				if (UserIsInList(username) == false)
					offlineUsers.Add(new UserInfo(username, true));
			}

			return((UserInfo[]) offlineUsers.ToArray(typeof(UserInfo)));
		}

		private bool UserIsInList (string username) {
			if (P2PManager.KnownPeers == null)
				return(false);

			foreach (UserInfo userInfo in P2PManager.KnownPeers.Keys) {
				if (userInfo.Name == username) return(true);
			}

			return(false);
		}

		private void InitializeBuddyListMenu() {
			ToggleActionEntry[] toggleEntries = new ToggleActionEntry[] {
				new ToggleActionEntry("ShowOffLineBuddies", null, 
									  "Show Offline Buddies", null,
									   null, new EventHandler(OnShowOfflineBuddies), false),
			};

			string ui = "<ui>" +
						"  <menubar name='MenuBar'>" +
						"    <menu action='NetworkMenu'>" +
						"      <separator />" +
						"      <menuitem action='ShowOffLineBuddies' />" +
						"    </menu>" +
						"  </menubar>" +
						"</ui>";

			nyFolder.MainWindow.Menu.AddMenus(ui, toggleEntries);
		}

		// ============================================
		// PUBLIC Properties
		// ============================================
		public bool ShowOfflineBuddies {
			get {
				MenuManager menu = nyFolder.MainWindow.Menu;
				string menuPath = "/MenuBar/NetworkMenu/ShowOffLineBuddies";
				CheckMenuItem check = (CheckMenuItem) menu.GetWidget(menuPath);
				return(check.Active);
			}
		}
	}
}
