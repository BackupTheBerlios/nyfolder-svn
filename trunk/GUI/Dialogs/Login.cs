/* [ GUI/Dialogs/Login.cs ] NyFolder (Login Dialog)
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
using System.Net;
using System.Threading;

using Niry;
using Niry.GUI.Gtk2;

using NyFolder;
using NyFolder.Protocol;

namespace NyFolder.GUI.Dialogs {
	internal class LoginProgressDialog : Gtk.Dialog {
		private Gtk.ProgressBar progressBar;
		private Gtk.Label labelMessage;
		private string message = null;
		private bool timerRet = true;
		internal uint timer;

		public LoginProgressDialog (string password) :
			base(MyInfo.Name + " Login", null, DialogFlags.Modal)
		{
			// Initialize Dialog Options
			WindowPosition = Gtk.WindowPosition.Center;

			// Initialize Dialog Events
			Response += new ResponseHandler(OnResponse);

			// Initialize Dialog Components
			AddButton(Gtk.Stock.Close, ResponseType.Close);

			Gtk.HBox hbox = new Gtk.HBox(false, 2);
			Gtk.VBox vbox = new Gtk.VBox(false, 2);
			VBox.PackStart(hbox, true, true, 2);
			hbox.PackStart(StockIcons.GetImage("Channel"), false, false, 2);
			hbox.PackStart(vbox, true, true, 2);

			// Initialize Label
			labelMessage = new Gtk.Label("<b>Waiting for " + MyInfo.Name + " Login...</b>");
			labelMessage.UseMarkup = true;
			vbox.PackStart(labelMessage, false, false, 2);

			// Initialize ProgressBar
			progressBar = new Gtk.ProgressBar();
			vbox.PackStart(progressBar, false, false, 2);

			// Initialize Timer
			timer = GLib.Timeout.Add(100, new GLib.TimeoutHandler(ProgressTimeout));

			// Initialize UserInfo
			MyInfo.LoginChecked += new LoginEventHandler(OnLoginChecked);
			MyInfo.Login(password);

			this.ShowAll();
		}

		private void OnResponse (object sender, ResponseArgs args) {
			timerRet = false;
		}

		private void OnLoginChecked (UserInfo info, bool status, string message) {
			Gtk.Application.Invoke(delegate {
				if (timerRet == true) {
					this.message = message;
					Respond((status == true) ? ResponseType.Ok : ResponseType.No);
				}
			});
		}

		private bool ProgressTimeout() {			
			if (timerRet == true)
				Gtk.Application.Invoke(delegate { progressBar.Pulse(); });
			return(timerRet);
		}

		public string ResponseMessage {
			get { return(this.message); }
		}
	}

	public class Login : LoginDialog {
		// ============================================
		// PROTECTED Members
		// ============================================
		protected Gtk.CheckButton checkSecureAuth;
		protected Gtk.MenuBar menuBar;

		// ============================================
		// PUBLIC Constructors
		// ============================================
		public Login() {
			SetDefaultSize(240, 355);
			DefaultIcon = StockIcons.GetPixbuf("NyFolderIcon");

			// Initialize Dialog Options
			Title = Info.Name + " Login";
			Logo = StockIcons.GetPixbuf("NyFolderLogo", 240, 140);

			// Remember Password (CheckButton)
			checkRememberPassword.Image = new Gtk.Image(Stock.Save, IconSize.Button);

			// Secure Authentication (CheckButton)
			checkSecureAuth = new CheckButton("Use Secure Authentication");
			checkSecureAuth.Active = true;
			checkSecureAuth.Image = StockIcons.GetImage("Lock", 22);
			checkSecureAuth.Toggled += new EventHandler(OnCheckSecureAuthToggled);
			VBox.PackStart(checkSecureAuth, false, false, 2);

			// Initialize MenuBar
			this.menuBar = new Gtk.MenuBar();
			VBox.PackStart(this.menuBar, false, false, 0);
			this.VBox.ReorderChild(this.menuBar, 0);
			InitializeMenuBar();

			// Initialize Dialog Buttons
			AddButton(Gtk.Stock.Ok, ResponseType.Ok);
			AddButton(Gtk.Stock.Quit, ResponseType.Close);

			ShowAll();
		}

		public bool ValidateInput() {
			// Check NULL Username
			if (Username == "" || Username == null) {
				WindowUtils.Shake(this, 2);
				Glue.Dialogs.MessageErrorDialog ("Invalid UserName", 
									"Please Set Your UserName, Null Username Found");
				return(false);
			}

			// If Insecure Authentication Mode Return Input OK
			if (SecureAuthentication == false)
				return(true);

			// Check NULL Password
			if (Password == "" || Password == null) {
				WindowUtils.Shake(this, 2);
				Glue.Dialogs.MessageErrorDialog ("Invalid Password", 
									"Please Set Your Password, Null Password Found");
				return(false);
			}

			return(true);
		}

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

			WindowUtils.Shake(this, 2);
			if (responseMsg != null)
				Glue.Dialogs.MessageErrorDialog("Login Error", responseMsg);
			return(null);
		}

		// ============================================
		// PRIVATE Methods
		// ============================================
		private void InitializeMenuBar() {
			MenuItem item;

			// File Menu
			Menu fileMenu = new Gtk.Menu();
			item = new ImageMenuItem("Proxy Settings");
			((ImageMenuItem) item).Image = StockIcons.GetImage("Proxy", 20);
			item.Activated += new EventHandler(OnProxySettings);
			fileMenu.Append(item);

			item = new SeparatorMenuItem();
			fileMenu.Append(item);

			item = new ImageMenuItem(Gtk.Stock.Quit, null);
			item.Activated += new EventHandler(OnQuit);
			fileMenu.Append(item);

			item = new MenuItem("_File");
			item.Submenu = fileMenu;
			this.menuBar.Append(item);

			// Help Menu
			Menu helpMenu = new Gtk.Menu();
			item = new ImageMenuItem(Gtk.Stock.About, null);
			item.Activated += new EventHandler(OnAbout);
			helpMenu.Append(item);

			item = new MenuItem("_Help");
			item.Submenu = helpMenu;
			this.menuBar.Append(item);
		}

		// ============================================
		// PRIVATE (Methods) Event Handler
		// ============================================
		private void OnCheckSecureAuthToggled (object sender, EventArgs args) {
			CheckButton checkButton = sender as CheckButton;
			this.labelPassword.Sensitive = checkButton.Active;
			this.entryPassword.Sensitive = checkButton.Active;
			this.checkRememberPassword.Sensitive = checkButton.Active;
		}

		private void OnProxySettings (object sender, EventArgs args) {
			Glue.Dialogs.ProxySettings();
		}

		private void OnAbout (object sender, EventArgs args) {
			new Dialogs.About();
		}

		private void OnQuit (object sender, EventArgs args) {
			Gtk.Application.Quit();
		}

		// ============================================
		// PUBLIC Properties
		// ============================================
		public bool SecureAuthentication {
			set { checkSecureAuth.Active = value; }
			get { return(checkSecureAuth.Active); }
		}
	}
}
