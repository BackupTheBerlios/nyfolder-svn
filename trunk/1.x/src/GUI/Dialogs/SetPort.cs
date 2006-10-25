/* [ GUI/Dialogs/SetPort.cs ] NyFolder Set P2P Port Dialog
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
using NyFolder.GUI.Base;

namespace NyFolder.GUI.Dialogs {
	/// Set P2P Port Dialog
	public class SetPort : GladeDialog {
		// ============================================
		// PRIVATE Members
		// ============================================
		[Glade.WidgetAttribute]
		private Gtk.Image image;
		[Glade.WidgetAttribute]
		private Gtk.SpinButton spinPort;

		// ============================================
		// PUBLIC Constructors
		// ============================================
		/// Create New P2P Port Dialog
		public SetPort() : base("dialog", "SetPortDialog.glade") {
			// Set Current Default P2PManager Port
			Port = P2PManager.Port;

			// Initialize Dialog Image
			this.image.Pixbuf = StockIcons.GetPixbuf("Channel", 48);
		}

		// ============================================
		// PUBLIC Properties
		// ============================================
		/// Get or Set P2P Port
		public int Port {
			set { spinPort.Value = value; }
			get { return(spinPort.ValueAsInt); }
		}
	}
}
