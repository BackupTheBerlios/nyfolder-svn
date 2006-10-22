/* [ GUI/Dialogs/Login/ProgressDialog.cs ] NyFolder Login Progress Dialog
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
using NyFolder.GUI;
using NyFolder.Protocol;

namespace NyFolder.GUI.Dialogs.LoginDialog {
	public class ProgressDialog : Gtk.Dialog {
		// ============================================
		// PRIVATE Members
		// ============================================
		private Gtk.ProgressBar progressBar;
		private Gtk.Label labelMessage;
		private string message = null;
		private bool timerRet = true;
		internal uint timer;

		// ============================================
		// PUBLIC Constructors
		// ============================================
		public ProgressDialog (string password) :
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

		// ============================================
		// PRIVATE Methods
		// ============================================
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

		// ============================================
		// PUBLIC Properties
		// ============================================
		public string ResponseMessage {
			get { return(this.message); }
		}
	}
}
