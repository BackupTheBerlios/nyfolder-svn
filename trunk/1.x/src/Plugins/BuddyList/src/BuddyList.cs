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
		private int myAccountId = -1;

		// ============================================
		// INTERNAL Members
		// ============================================	
		internal bool timeoutAddBuddyRet;

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
			CmdManager.AddProtocolEvent += new SetProtocolEventHandler(OnAddProtocolEvent);
			CmdManager.DelProtocolEvent += new SetProtocolEventHandler(OnDelProtocolEvent);
		}

		// ============================================
		// PRIVATE (Methods) Event Handlers
		// ============================================
		private void OnUserLogin (object sender) {
			GUI.Dialogs.Login login = this.nyFolder.LoginDialog;
			if (login.RememberPassword && login.SecureAuthentication) {
				BuddyDb.AddAccount(login.Username, login.Password);
			}
			myAccountId = BuddyDb.AccountId(login.Username);
		}

		private void OnLoginDialogStart (object sender) {
			Gtk.Application.Invoke(delegate {
				GUI.Dialogs.Login login = sender as GUI.Dialogs.Login;

				// Fill User Entry Completion
				string[] accounts = BuddyDb.Accounts();
				login.UserNameCompletion.Model = CreateEntryCompletion(accounts);
				login.UserNameComboBoxAppend(accounts);
				login.UserNameCompletion.TextColumn = 0;

				// Add Event Handler When Password Entry Get Focus
				login.PasswordFocusIn += new FocusInEventHandler(OnPasswordFocusIn);
			});
		}

		private void OnLoginDialogClose (object sender) {
			GUI.Dialogs.Login login = sender as GUI.Dialogs.Login;
			login.PasswordFocusIn -= new FocusInEventHandler(OnPasswordFocusIn);
		}

		private void OnMainWindowStart (object sender) {
			GUI.Window window = sender as GUI.Window;

			// Initialize Buddy List Component
			InitializeBuddyListMenu();

			// Network GUI Event
			NetworkViewer networkViewer = window.NotebookViewer.NetworkViewer;
			NetworkManager.UserAccept += new AcceptUserHandler(OnAcceptUser);
			networkViewer.RefreshPeers += new BlankEventHandler(OnNetViewerRefresh);
			networkViewer.UserLoggedIn += new PeerSelectedHandler(OnUserLoggedIn);
			networkViewer.UserLoggedOut += new PeerSelectedHandler(OnUserLoggedOut);
		}

		private void OnMainWindowClose (object sender) {
			GUI.Window window = sender as GUI.Window;
			NetworkViewer networkViewer = window.NotebookViewer.NetworkViewer;

			// Stop Auto Add Buddy Timeout
			timeoutAddBuddyRet = false;

			// Remove Network GUI Event
			NetworkManager.UserAccept -= new AcceptUserHandler(OnAcceptUser);
			networkViewer.RefreshPeers -= new BlankEventHandler(OnNetViewerRefresh);
			networkViewer.UserLoggedIn -= new PeerSelectedHandler(OnUserLoggedIn);
			networkViewer.UserLoggedOut -= new PeerSelectedHandler(OnUserLoggedOut);
		}

		// ============================================
		// PRIVATE (Methods) Event Handlers 
		// ============================================
		private void OnPasswordFocusIn (object obj, FocusInEventArgs args) {
			Gtk.Application.Invoke(delegate {
				GUI.Dialogs.Login login = this.nyFolder.LoginDialog;
				login.Password = BuddyDb.AccountPassword(login.Username);
			});
		}

		private void OnUserLoggedIn (object sender, UserInfo userInfo) {
			if (userInfo.SecureAuthentication == false)
				return;

			// Add Only Secure Auth User
			BuddyDb.AddUser(myAccountId, userInfo.Name, true);
		}

		private void OnUserLoggedOut (object sender, UserInfo userInfo) {
			if (ShowOfflineBuddies == false)
				return;

			if (userInfo.SecureAuthentication == false)
				return;

			// Add Only Secure Auth User
			if (BuddyDb.UserIsInList(myAccountId, userInfo.Name) == true) {
				Gtk.Application.Invoke(delegate {
					// Load All Offline Buddy
					NetworkViewer networkViewer = sender as NetworkViewer;
					networkViewer.Store.Add(new UserInfo(userInfo.Name, true));
				});
			}
		}

		private AcceptUserType OnAcceptUser (PeerSocket peer, UserInfo userInfo) {
			if (userInfo.SecureAuthentication == false)
				return(AcceptUserType.Ask);

			UsersTable usersDb = new UsersTable();

			// If User isn't into DB
			if (usersDb.IsPresent(myAccountId, userInfo.Name) == false) {
				usersDb.Dispose();
				return(AcceptUserType.Ask);
			}

			// Get User Accept Status
			bool acceptUser = usersDb.GetAccept(myAccountId, userInfo.Name);

			usersDb.Dispose();
			return(acceptUser ? AcceptUserType.Yes : AcceptUserType.No);
		}
		
		// When Network Viewer is Refreshed
		private void OnNetViewerRefresh (object sender) {
			Gtk.Application.Invoke(delegate {
				NetworkViewer networkViewer = sender as NetworkViewer;

				AutoAddBuddyTimeout();
				if (ShowOfflineBuddies == true) {
					// Load All Offline Buddies
					networkViewer.Store.Add(GetOfflineUsers());
				}
			});
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

		private void OnAddProtocolEvent (P2PManager p2pManager) {
			// Auto Add Buddy Timeout (2min)
			timeoutAddBuddyRet = true;
			AutoAddBuddyTimeout();
			GLib.Timeout.Add(120000, AutoAddBuddyTimeout);
		}

		private void OnDelProtocolEvent (P2PManager p2pManager) {
			// Stop Auto Add Buddy Timeout
			timeoutAddBuddyRet = false;
		}

		private bool AutoAddBuddyTimeout() {
			// If I'm Offline Skip
			if (P2PManager.IsListening() == false)
				return(timeoutAddBuddyRet);

			// If Timeout is Ended
			if (timeoutAddBuddyRet != true)
				return(false);

			// Get Offline Users
			UserInfo[] offline = GetOfflineUsers();
			if (offline == null) return(timeoutAddBuddyRet);

			// Connect Offline Users
			foreach (UserInfo userInfo in offline) {
				UserConnect(userInfo);
				Gtk.Application.Invoke(delegate {
					AddUserToNetworkViewer(userInfo);
				});
			}

			return(timeoutAddBuddyRet);
		}

		// ============================================
		// PRIVATE Methods
		// ============================================
		private ListStore CreateEntryCompletion (string[] accNames) {
			// No Account Saved
			if (accNames == null) return(null);

			// Add Account
			ListStore store = new ListStore(typeof(string));
			foreach (string name in accNames)
				store.AppendValues(name);
			return(store);
		}

		private UserInfo[] GetOfflineUsers() {
			string[] users = BuddyDb.AccountAcceptedUsers(myAccountId);
			if (users == null) return(null);

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

		private void UserConnect (UserInfo userInfo) {
			if (userInfo.SecureAuthentication == false)
				return;

			try {
				// Connect & Send Login
				userInfo.GetIpAndPort();
				Debug.Log("Auto Add Buddy: {0} {1}:{2}", userInfo.Name, userInfo.Ip, userInfo.Port);
				P2PManager.AddPeer(userInfo, userInfo.Ip, userInfo.Port);
				PeerSocket peer = (PeerSocket) P2PManager.KnownPeers[userInfo];
				Cmd.Login(peer, MyInfo.GetInstance());
			} catch (Exception e) {
				Debug.Log("Connection To {0} Failed: {1}", userInfo.Name, e.Message);
			}
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

		private void AddUserToNetworkViewer (UserInfo userInfo) {
			NetworkViewer networkViewer = nyFolder.MainWindow.NotebookViewer.NetworkViewer;
			networkViewer.Store.Add(userInfo);
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
