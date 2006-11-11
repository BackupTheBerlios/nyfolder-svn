/* [ GUI/Dialogs/RemovePeer.cs ] NyFolder Remove Peer Dialog
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
	/// Remove Peer Dialog
	public class RemovePeer : GladeDialog {
		// ============================================
		// PRIVATE Members
		// ============================================		
		[Glade.WidgetAttribute] private Gtk.VBox vboxMain;
		[Glade.WidgetAttribute] private Gtk.Image image;
		private Gtk.ComboBox comboPeers;

		// ============================================
		// PUBLIC Constructors
		// ============================================
		/// Create New "Remove Peer" Dialog
		public RemovePeer() : base("dialog", "RemovePeerDialog.glade") {
			this.comboPeers = ComboBox.NewText();
			this.vboxMain.PackStart(this.comboPeers, false, false, 2);

			// Add Peers
			if (P2PManager.KnownPeers != null) {
				foreach (UserInfo userInfo in P2PManager.KnownPeers.Keys) {
					this.comboPeers.AppendText(userInfo.Name);
				}
			}
			this.comboPeers.ShowAll();

			// Initialize Dialog Image
			this.image.Pixbuf = StockIcons.GetPixbuf("Network", 96);
		}

		// ============================================
		// PUBLIC Methods
		// ============================================
		/// Get Selected Peer and Return his name, or null if no one is selected.
		public string GetPeerSelected() {
			TreeIter iter;
			if (comboPeers.GetActiveIter(out iter))
				return((string) comboPeers.Model.GetValue(iter, 0));
			return(null);
		}
	}
}
