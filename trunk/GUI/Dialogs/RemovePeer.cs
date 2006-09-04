/* [ GUI/Dialogs/RemovePeer.cs ] NyFolder (Remove Peer Dialog)
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

using Niry;
using Niry.Network;

using NyFolder;
using NyFolder.Protocol;

namespace NyFolder.GUI.Dialogs {
	public class RemovePeer {
		[Glade.WidgetAttribute]
		private Gtk.ComboBox comboPeers;
		[Glade.WidgetAttribute]
		private Gtk.VBox vboxMain;
		[Glade.WidgetAttribute]
		private Gtk.Dialog dialog;
		[Glade.WidgetAttribute]
		private Gtk.Image image;

		public RemovePeer() {
			XML xml = new XML(null, "RemovePeerDialog.glade", "dialog", null);
			xml.Autoconnect(this);

			this.comboPeers = ComboBox.NewText();
			this.vboxMain.PackStart(this.comboPeers, false, false, 2);

			// Add Peers
			if (P2PManager.KnownPeers != null) {
				foreach (UserInfo userInfo in P2PManager.KnownPeers.Keys)
					this.comboPeers.AppendText(userInfo.Name);
			}

			this.image.Pixbuf = StockIcons.GetPixbuf("Network");
			this.dialog.ShowAll();
		}

		public ResponseType Run() {
			return((ResponseType) dialog.Run());
		}

		public void Destroy() {
			dialog.Destroy();
		}

		public string GetPeerSelected() {
			TreeIter iter;
			if (comboPeers.GetActiveIter(out iter))
				return((string) comboPeers.Model.GetValue(iter, 0));
			return(null);
		}
	}
}
