/* [ GUI/Dialogs/AcceptUser.cs ] NyFolder Accept User Dialog
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
using Niry.Network;

using NyFolder;
using NyFolder.Protocol;
using NyFolder.GUI.Base;

namespace NyFolder.GUI.Dialogs {
	/// Proxy Settings Dialog
	public class AcceptUser : GladeDialog {
		// ============================================
		// PRIVATE GLADE Members
		// ============================================
		[Glade.WidgetAttribute] private Gtk.Label labelTitle;
		[Glade.WidgetAttribute] private Gtk.Entry entryName;
		[Glade.WidgetAttribute] private Gtk.Entry entryIP;
		[Glade.WidgetAttribute] private Gtk.Image image;

		// ============================================
		// PUBLIC Constructors
		// ============================================
		/// Create New Proxy Settings Dialog
		public AcceptUser (PeerSocket peer) : 
			base("dialog", "AcceptUserDialog.glade")
		{
			// Get UserInfo
			UserInfo userInfo = peer.Info as UserInfo;

			// Initialize GUI
			this.labelTitle.Text = "<span size='x-large'><b>Accept User</b> (";
			if (userInfo.SecureAuthentication == true) {
				this.image.Pixbuf = StockIcons.GetPixbuf("SecureAuth");
				this.labelTitle.Text += "Secure";
				this.Dialog.Title += " (Secure Authentication)";
			} else {
				this.image.Pixbuf = StockIcons.GetPixbuf("InsecureAuth");
				this.labelTitle.Text += "Insecure";
				this.Dialog.Title += " (Insecure Authentication)";
			}
			this.labelTitle.Text += ")</span>";
			this.labelTitle.UseMarkup = true;

			entryName.Text = userInfo.Name;
			entryIP.Text = peer.GetRemoteIP().ToString();
		}
	}
}
