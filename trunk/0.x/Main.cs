/* [ Main.cs ] NyFolder (Main)
 * Author: Matteo Bertozzi
 * ============================================================================
 * This program (NyFolder) is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
 */

using Gtk;

using System;
using System.Threading;

using Niry;
using Niry.Utils;
using Niry.Network;

using NyFolder;
using NyFolder.GUI;
using NyFolder.Utils;
using NyFolder.Protocol;

namespace NyFolder {
	public class NyFolderApp : INyFolder {
		// ============================================
		// PUBLIC Event(s)
		// ============================================
		public event BlankEventHandler QuittingApplication;
		public event BlankEventHandler MainWindowStarted;

		// ============================================
		// PUBLIC STATIC Members
		// ============================================
		public static bool RestartApplication = false;

		// ============================================
		// PROTECTED Members
		// ============================================
		protected GUI.Window window = null;
		protected UserInfo myInfo = null;

		// ============================================
		// PUBLIC Constructors
		// ============================================
		public NyFolderApp() {
			MainWindowStarted = null;
			QuittingApplication = null;
		}

		// ============================================
		// PUBLIC Methods
		// ============================================
		public void Initialize() {
			// Initialize NyFolder Paths
			Paths.Initialize();

			// Set Home Directory as Current Environment Path
			Environment.CurrentDirectory = Paths.HomeDirectory;

			// Initialize Proxy Settings
			Proxy.Initialize();

			// Initialize (Gtk GUI) Stock Icons
			GUI.StockIcons.Initialize();
		}

		public void Run() {
			// Start With Login Dialog
			GUI.Dialogs.Login dialog = new GUI.Dialogs.Login();
			dialog.Response += new ResponseHandler(OnLoginResponse);
		}

		public void Quit() {
			// Quitting Event
			if (QuittingApplication != null) QuittingApplication(this);

			// Do User Logout
			Logout();
		}

		public void Logout() {
			// Disconnect User From Http Server
			Protocol.MyInfo.Logout();
		}

		// ============================================
		// PRIVATE Methods
		// ============================================
		private void RunMainWindow() {
			// Start NyFolder Window
			this.window = new GUI.Window();
			this.window.Logout += new BlankEventHandler(OnLogout);

			Debug.Log("Logged In as {0}", myInfo.Name);
			Debug.Log("Shared Path: {0}", Paths.UserSharedDirectory(myInfo.Name));

			// Add GUI Glue
			new GUI.Glue.FolderManager(window.Menu, window.UserPanel, window.NotebookViewer);
			new GUI.Glue.NetworkManager(window.Menu, window.UserPanel, window.NotebookViewer);
			new GUI.Glue.ProtocolManager(window.NotebookViewer);

			// NyFolder Window ShowAll
			this.window.ShowAll();

			// Start 'Main Window Started' Event
			if (MainWindowStarted != null) MainWindowStarted(this);
		}

		private void DoLogin (GUI.Dialogs.Login dialog) {
			if (dialog.ValidateInput() == false)
				return;

			if ((myInfo = dialog.CheckLogin()) != null) {
				dialog.Destroy();
			}
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
			RestartApplication = true;

			// Destroy Main Window
			window.Destroy();
			window = null;

			Gtk.Application.Quit();
		}

		// ============================================
		// PUBLIC Properties
		// ============================================
		public GUI.Window Window {
			get { return(this.window); }
		}

		public UserInfo MyInfo {
			get { return(this.myInfo); }
		}

		// ============================================
		//               APPLICATION MAIN
		// ============================================
		public static int Main (string[] args) {
			do {
				NyFolderApp.RestartApplication = false;
				P2PManager p2pManager = null;
				NyFolderApp nyFolder = null;

				try {
					// Initialize P2PManager
					p2pManager = P2PManager.GetInstance();

					// Initialize Gtk Support
					Gtk.Application.Init();

					// Initialize NyFolder Application
					nyFolder = new NyFolderApp();
					nyFolder.Initialize();

					// Initialize Plugins
					new PluginManager(nyFolder);

					// Run NyFolder Application
					nyFolder.Run();

					// Run GtkMain
					Gtk.Application.Run();
				} catch (Exception e) {
					Console.WriteLine("{0} {1} Error", Info.Name, Info.Version);
					Console.WriteLine("Source:  {0}", e.Source);
					Console.WriteLine("Message: {0}", e.Message);
					Console.WriteLine("Stack Trace:\n{0}", e.StackTrace);
					Console.WriteLine();
					Console.WriteLine("Please, Report Bug(s) To Matteo Bertozzi <theo.bertozzi@gmail.com>");
					return(1);
				} finally {
					if (nyFolder != null) nyFolder.Quit();
					if (p2pManager != null) p2pManager.Kill();
				}
			} while (NyFolderApp.RestartApplication == true);
			return(0);
		}
	}
}
