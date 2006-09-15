/* [ GUI/Dialogs/SetPort.cs ] NyFolder (Set Port Dialog)
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

namespace NyFolder.GUI.Dialogs {
	public class SetPort {
		[Glade.WidgetAttribute]
		private Gtk.Dialog dialog;
		[Glade.WidgetAttribute]
		private Gtk.Image image;
		[Glade.WidgetAttribute]
		private Gtk.SpinButton spinPort;


		public SetPort() {
			XML xml = new XML(null, "SetPortDialog.glade", "dialog", null);
			xml.Autoconnect(this);

			Port = P2PManager.Port;

			this.image.Pixbuf = StockIcons.GetPixbuf("Channel");
			this.dialog.ShowAll();
		}

		public ResponseType Run() {
			return((ResponseType) dialog.Run());
		}

		public void Destroy() {
			dialog.Destroy();
		}

		// ============================================
		// PUBLIC Properties
		// ============================================
		public int Port {
			set { spinPort.Value = value; }
			get { return(spinPort.ValueAsInt); }
		}
	}
}
