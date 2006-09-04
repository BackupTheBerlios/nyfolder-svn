/* [ GUI/Dialogs/PeerProperties.cs ] NyFolder (Peer Properties Dialog)
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

using Niry;
using Niry.Network;

using NyFolder;
using NyFolder.Protocol;

namespace NyFolder.GUI.Dialogs {
	public class PeerProperties {
		[Glade.WidgetAttribute] private Gtk.Dialog dialog;
		[Glade.WidgetAttribute] private Gtk.Image imageAvatar;
		[Glade.WidgetAttribute] private Gtk.Image imageSecure;
		[Glade.WidgetAttribute] private Gtk.Entry entryName;
		[Glade.WidgetAttribute] private Gtk.Entry entryIp;
		[Glade.WidgetAttribute] private Gtk.Entry entryPort;
		[Glade.WidgetAttribute] private Gtk.TextView textViewComments;
		[Glade.WidgetAttribute] private Gtk.Notebook notebook;

		public PeerProperties (UserInfo userInfo) {
			XML xml = new XML(null, "PeerPropertiesDialog.glade", "dialog", null);
			xml.Autoconnect(this);

			// Set UserInfo
			entryName.Text = userInfo.Name;
			entryIp.Text = userInfo.GetCurrentIp();

			try {
				entryPort.Text = userInfo.GetPort().ToString();
			} catch (Exception e) {
				entryPort.Text = e.Message;
			}


			this.imageAvatar.Pixbuf = StockIcons.GetPixbuf("Network");
			if (userInfo.SecureAuthentication == true) {
				this.imageSecure.Pixbuf = StockIcons.GetPixbuf("SecureAuth");
			} else {
				this.imageSecure.Pixbuf = StockIcons.GetPixbuf("InsecureAuth");
			}
			this.dialog.ShowAll();
		}

		public ResponseType Run() {
			return((ResponseType) dialog.Run());
		}

		public void Destroy() {
			dialog.Destroy();
		}

		public Gtk.Notebook Notebook {
			get { return(this.notebook); }
		}
	}
}
