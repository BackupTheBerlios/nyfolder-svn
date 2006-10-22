/* [ GUI/Dialogs/Login/Login.cs ] NyFolder Login Dialog
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

using NyFolder;
using NyFolder.Utils;
using NyFolder.Protocol;

namespace NyFolder.GUI.Dialogs {
	/// Login Dialog
	public class Login {
		// ============================================
		// PUBLIC Events
		// ============================================

		// ============================================
		// PROTECTED Members
		// ============================================
		protected Gtk.CheckButton checkSecureAuth;
		protected Login.MenuManager menuManager;

		// ============================================
		// PRIVATE Members
		// ============================================
		private Gtk.HBox hboxMenu;

		// ============================================
		// PUBLIC Constructors
		// ============================================
		/// Create New Login Dialog
		public Login() {
			SetDefaultSize(240, 355);
			DefaultIcon = StockIcons.GetPixbuf("NyFolderIcon");

			// Initialize Dialog Options
			Title = Info.Name + " Login";
			Logo = StockIcons.GetPixbuf("NyFolderLogo", 240, 140);

			// Initialize Menu's HBox
			this.hboxMenu = new Gtk.HBox(false, 0);
			VBox.PackStart(this.hboxMenu, false, false, 0);
			this.VBox.ReorderChild(this.menuBar, 0);

			// Initialize Menu Manager
			this.menuManager = new MenuManager();
			this.hboxMenu.PackStart(this.menuManager, false, false, 0);
			this.menuManager.Activated += new EventHandler(OnMenuActivated);
			this.AddAccelGroup(this.menuManager.AccelGroup);

			// Remember Password (CheckButton)
			checkRememberPassword.Image = new Gtk.Image(Stock.Save, IconSize.Button);

			// Secure Authentication (CheckButton)
			checkSecureAuth = new CheckButton("Use Secure Authentication");
			checkSecureAuth.Active = true;
			checkSecureAuth.Image = StockIcons.GetImage("Lock", 22);
			checkSecureAuth.Toggled += new EventHandler(OnCheckSecureAuthToggled);
			VBox.PackStart(checkSecureAuth, false, false, 2);

			// Initialize Dialog Buttons
			AddButton(Gtk.Stock.Ok, ResponseType.Ok);
			AddButton(Gtk.Stock.Quit, ResponseType.Close);

			ShowAll();
		}

		// ============================================
		// PUBLIC Methods
		// ============================================
		/// Return true if Input is Valid
		public bool ValidateInput() {
			// Check NULL Username
			if (TextUtils.IsEmpty(Username)) {
				string title = "Invalid UserName";
				string message = "Please Set Your UserName, Null Username Found";
				ShowErrorMessage(title, message);
				return(false);
			}

			// If Insecure Authentication Mode Return Input OK
			if (SecureAuthentication == false)
				return(true);

			// Check NULL Password
			if (TextUtils.IsEmpty(Password)) {
				string title = "Invalid Password";
				string message = "Please Set Your Password, Null Password Found";
				ShowErrorMessage(title, message);
				return(false);
			}

			return(true);
		}

		/// Check Login, Return null if Login failed
		public UserInfo CheckLogin() {
			MyInfo.Initialize(Username, SecureAuthentication);

			if (SecureAuthentication == false)
				return(MyInfo.GetInstance());

			// Login Progress Dialog
			LoginProgressDialog dialog = new LoginProgressDialog(Password);
			ResponseType response = (ResponseType) dialog.Run();
			string responseMsg = dialog.ResponseMessage;
			dialog.Destroy();

			// Login OK
			if (response == ResponseType.Ok)
				return(MyInfo.GetInstance());

			string title = "Login Error";
			if (responseMsg == null) responseMsg = "Unknown Error...";
			ShowErrorMessage(title, responseMsg);
			return(null);
		}

		// ============================================
		// PRIVATE (Methods) Event Handlers
		// ============================================
		private void OnMenuActivated (object sender, EventArgs args) {
			Gtk.Application.Invoke(delegate {
				Action action = sender as Action;
				switch (action.Name) {
					// File Menu
					case "ProxySettings":
						Glue.Dialogs.ProxySettings();
						break;
					case "Quit":
						Gtk.Application.Quit();
						break;
					// Help Menu
					case "About":
						new Dialogs.About();
						break;
				}
			});
		}

		// ============================================
		// PRIVATE Methods
		// ============================================
		private void ShowErrorMessage (string title, string message) {
			WindowUtils.Shake(this, 2);
			Glue.Dialogs.MessageError(title, message);
		}

		// ============================================
		// PUBLIC Properties
		// ============================================
		/// Get or Set Secure Authentication Check Button
		public bool SecureAuthentication {
			set { checkSecureAuth.Active = value; }
			get { return(checkSecureAuth.Active); }
		}

		/// Get The Menu Manager
		public Login.MenuManager Menu {
			get { return(this.menuManager); }
		}

		/// Get The MenuBar
		public Gtk.MenuBar MenuBar {
			get { return((Gtk.MenuBar) this.menuManager.GetWidget("/MenuBar")); }
		}

		/// Get The Menu Bar's HBox
		public Gtk.HBox MenuBarHBox {
			get { return(this.hboxMenu); }
		}
	}
}
