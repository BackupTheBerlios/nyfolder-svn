/* [ GUI/Dialogs/AddPeer.cs ] NyFolder (Add Peer Dialog)
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
using Glade;
using System;

namespace NyFolder.GUI.Dialogs {
	/// Add Peer Dialog
	public class AddPeer {
		[Glade.WidgetAttribute]
		private Gtk.Dialog dialog;
		[Glade.WidgetAttribute]
		private Gtk.Image image;
		[Glade.WidgetAttribute]
		private Gtk.Entry entryUserName;
		[Glade.WidgetAttribute]
		private Gtk.CheckButton checkSecureAuth;
		[Glade.WidgetAttribute]
		private Gtk.Entry entryIP;
		[Glade.WidgetAttribute]
		private Gtk.SpinButton spinPort;
		[Glade.WidgetAttribute]
		private Gtk.Expander expander;

		/// Create New Add Peer Dialog
		public AddPeer() {
			XML xml = new XML(null, "AddPeerDialog.glade", "dialog", null);
			xml.Autoconnect(this);

			this.image.Pixbuf = StockIcons.GetPixbuf("Network");
			checkSecureAuth.Toggled += new EventHandler(OnCheckSecureAuthToggled);

			this.dialog.Response += new ResponseHandler(OnResponse);
			this.dialog.ShowAll();

			// Widget.Sensitive
			OnCheckSecureAuthToggled(checkSecureAuth, null);
		}

		/// Run Dialog
		public ResponseType Run() {
			return((ResponseType) dialog.Run());
		}

		/// Destroy Dialog
		public void Destroy() {
			dialog.Destroy();
		}

		// ============================================
		// PRIVATE Methods
		// ============================================
		private void MessageErrorDialog (string title, string message) {
			MessageDialog dialog;

			dialog = new MessageDialog (null, DialogFlags.Modal, MessageType.Error,
										ButtonsType.Close, true, 
										"<span size='x-large'><b>{0}</b></span>\n\n{1}",
										title, message);
			dialog.Run();
			dialog.Destroy();
		}

		// ============================================
		// PRIVATE (Methods) Event Handler
		// ============================================
		private void OnCheckSecureAuthToggled (object sender, EventArgs args) {
			CheckButton checkButton = sender as CheckButton;
			bool status = !checkButton.Active;
			this.expander.Sensitive = status;
			this.expander.Expanded = status;
		}

		private void OnResponse (object sender, ResponseArgs args) {
			if (args.ResponseId == ResponseType.Ok) {
				// Check UserName
				if (Username == null) {
					MessageErrorDialog ("Invalid UserName", 
										"Please Set UserName, Null Username Found");
					return;
				}

				if (SecureAuthentication == false) {
					if (Ip == null) {
						MessageErrorDialog ("Invalid Ip", 
											"Please Set Ip, Null Ip Found");
						Username = "";
						return;
					}

					if (Port == 0) {
						MessageErrorDialog ("Invalid Port", 
											"Please Set Port, Null Port Found");
						Username = "";
						return;
					}
				}
			}
		}

		// ============================================
		// PUBLIC Properties
		// ============================================
		/// Get or Set User UserName
		public string Username {
			set { entryUserName.Text = value; }
			get { return(entryUserName.Text == "" ? null : entryUserName.Text); }
		}

		/// Get or Set User Ip
		public string Ip {
			set { entryIP.Text = value; }
			get { return(entryIP.Text == "" ? null : entryIP.Text); }
		}

		/// Get or Set User Port
		public int Port {
			set { spinPort.Value = value; }
			get { return(spinPort.ValueAsInt); }
		}

		/// Get or Set if User has Secure Authentication
		public bool SecureAuthentication {
			set { checkSecureAuth.Active = value; }
			get { return(checkSecureAuth.Active); }
		}
	}
}
