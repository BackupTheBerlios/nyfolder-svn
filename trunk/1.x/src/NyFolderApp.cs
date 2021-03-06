/* [ NyFolderApp.cs ] NyFolder Application
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
using System.Threading;

using Niry;
using Niry.Utils;
using Niry.Network;

using NyFolder;
using NyFolder.Utils;
using NyFolder.Protocol;
using NyFolder.PluginLib;

namespace NyFolder {
	public class NyFolderExit : Exception {
		/// Create New NyFolder Exit Exception
		public NyFolderExit (string msg) : base(msg) {}
		/// Create New NyFolder Exit Exception
		public NyFolderExit (string msg, Exception inner) : base(msg, inner) {}
	}

	/// NyFolder Main Application
	public sealed class NyFolderApp : INyFolder {
		// ============================================
		// PUBLIC Events
		// ============================================
		/// Event Raised When Login Dialog is Started
		public event BlankEventHandler LoginDialogStarted = null;
		/// Event Raised When Login Dialog is Closed
		public event BlankEventHandler LoginDialogClosed = null;
		/// Event Raised When Main Window is Started
		public event BlankEventHandler MainWindowStarted = null;
		/// Event Raised When Main Window is Closed
		public event BlankEventHandler MainWindowClosed = null;
		/// Event Raised When User Do Logout
		public event BlankEventHandler UserLogout = null;
		/// Event Raised When User Logged In
		public event BlankEventHandler UserLogin = null;

		// ============================================
		// PRIVATE STATIC Members
		// ============================================
		/// Restart Application Flag
		public static bool Restart = false;

		// ============================================
		// PRIVATE Members
		// ============================================
		private GUI.Dialogs.Login loginDialog = null;
		private GUI.Window mainWindow = null;
		private UserInfo myInfo = null;

		// ============================================
		// PUBLIC Constructors
		// ============================================
		/// Create New NyFolder Application
		public NyFolderApp() {
		}

		// ============================================
		// PUBLIC Methods
		// ============================================
		public void Initialize() {
		}

		/// Run Application Starting with Login Dialog if No Auto Account
		public void Run() {
			// Start With Login Dialog
			loginDialog = new GUI.Dialogs.Login();
			if (LoginDialogStarted != null) LoginDialogStarted(loginDialog);
			loginDialog.Response += new ResponseHandler(OnLoginResponse);
		}

		/// Quit The Application
		public void Quit() {
			Restart = false;

			// Quitting Event
			if (loginDialog != null && LoginDialogClosed != null) {
				LoginDialogClosed(loginDialog);
				loginDialog.Hide();
				loginDialog.Destroy();
			}
			loginDialog = null;
			
			if (mainWindow != null && MainWindowClosed != null) {
				MainWindowClosed(mainWindow);
				mainWindow.Hide();
				mainWindow.Destroy();
			}
			mainWindow = null;

			// Do User Logout
			if (UserLogout != null) UserLogout(myInfo);
			Protocol.MyInfo.Logout();
			this.myInfo = null;
		}

		/// Logout From the Current Account (Go To Login Dialog)
		public void Logout() {
			// Destroy Main Window
			mainWindow.Destroy();
			Quit();

			Restart = true;
			throw(new NyFolderExit("Logout"));
		}

		// ============================================
		// PRIVATE (Methods) Event Handlers
		// ============================================
		private void OnLoginResponse (object sender, ResponseArgs args) {
			if (args.ResponseId == ResponseType.Ok) {
				DoLogin((GUI.Dialogs.Login) sender);
				if (myInfo != null) RunMainWindow();
			} else {
				Gtk.Application.Quit();
			}
		}

		private void OnLogout (object sender) {
			Logout();
		}

		private void OnQuit (object sender) {
			Quit();
		}

		// ============================================
		// PRIVATE Methods
		// ============================================
		private void RunMainWindow() {
			// Start NyFolder Window
			mainWindow = new GUI.Window();
			mainWindow.Quit += new BlankEventHandler(OnQuit);
			mainWindow.Logout += new BlankEventHandler(OnLogout);

			Debug.Log("Logged In as {0}", myInfo.Name);
			Debug.Log("Shared Path: {0}", Paths.UserSharedDirectory(myInfo.Name));

			// Add GUI Glue
			new GUI.Glue.FolderManager(mainWindow);
			new GUI.Glue.NetworkManager(mainWindow);
			new GUI.Glue.ProtocolManager(mainWindow);

			// NyFolder Window ShowAll
			mainWindow.ShowAll();

			// Start 'Main Window Started' Event
			if (MainWindowStarted != null) MainWindowStarted(mainWindow);
		}

		private void DoLogin (GUI.Dialogs.Login dialog) {
			if (dialog.ValidateInput() == false)
				return;

			if ((myInfo = dialog.CheckLogin()) != null) {
				if (UserLogin != null) UserLogin(myInfo);
				if (LoginDialogClosed != null) LoginDialogClosed(loginDialog);
				dialog.Destroy();
				loginDialog = null;
			}
		}

		// ============================================
		// PUBLIC Properties
		// ============================================
		/// Get NyFolder Main Window or null
		public GUI.Window MainWindow {
			get { return(this.mainWindow); }
		}

		/// Get NyFolder Login Dialog or null
		public GUI.Dialogs.Login LoginDialog {
			get { return(this.loginDialog); }
		}

		/// Get My UserInfo if I've done Login
		public UserInfo MyInfo {
			get { return(this.myInfo); }
		}
	}
}
