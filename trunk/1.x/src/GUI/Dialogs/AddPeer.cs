/* [ GUI/Dialogs/AddPeer.cs ] NyFolder Add Peer Dialog
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

using NyFolder;
using NyFolder.Protocol;
using NyFolder.GUI.Base;

namespace NyFolder.GUI.Dialogs {
	/// Remove Add Dialog
	public class AddPeer : GladeDialog {
		// ============================================
		// PRIVATE Members
		// ============================================
		[Glade.WidgetAttribute] private Gtk.CheckButton checkSecureAuth;
		[Glade.WidgetAttribute] private Gtk.Entry entryUserName;
		[Glade.WidgetAttribute] private Gtk.SpinButton spinPort;
		[Glade.WidgetAttribute] private Gtk.Expander expander;
		[Glade.WidgetAttribute] private Gtk.Entry entryIP;
		[Glade.WidgetAttribute] private Gtk.Image image;

		// ============================================
		// PUBLIC Constructors
		// ============================================
		/// Create New "Add Peer" Dialog
		public AddPeer() : base("dialog", "AddPeerDialog.glade") {
			checkSecureAuth.Toggled += new EventHandler(OnCheckSecureAuthToggled);

			this.Dialog.Response += new ResponseHandler(OnResponse);

			// Widget.Sensitive
			OnCheckSecureAuthToggled(checkSecureAuth, null);

			// Initialize Dialog Image
			this.image.Pixbuf = StockIcons.GetPixbuf("Network", 48);
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
			if (args.ResponseId != ResponseType.Ok)
				return;

			// Check UserName
			if (Username == null) {
				string title = "Invalid UserName";
				string message = "Please Set UserName, Null Username Found";
				Base.Dialogs.MessageError(title, message);
				return;
			}

			// Check Insecure Auth Forms
			if (SecureAuthentication == false) {
				if (Ip == null) {
					Username = null;
					string title = "Invalid Ip";
					string message = "Please Set Ip, Null Ip Found";
					Base.Dialogs.MessageError(title, message);
					return;
				}

				if (Port == 0) {
					Username = null;
					string title = "Invalid Port";
					string message = "Please Set Port, Null Port Found";
					Base.Dialogs.MessageError(title, message);
					return;
				}
			}
		}

		// ============================================
		// PUBLIC Properties
		// ============================================
		/// Get or Set User UserName
		public string Username {
			set { entryUserName.Text = value; }
			get { return(TextUtils.IsEmpty(entryUserName.Text) ? null : entryUserName.Text); }
		}

		/// Get or Set User Ip
		public string Ip {
			set { entryIP.Text = value; }
			get { return(TextUtils.IsEmpty(entryIP.Text) ? null : entryIP.Text); }
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
