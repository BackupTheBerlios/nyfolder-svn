/* [ Plugins/DownloadManager/GUI/Window.cs ] NyFolder (Download Manager Window)
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
using GLib;

using System;
using System.Collections;

using Niry;
using Niry.Utils;
using Niry.Network;

using NyFolder;
using NyFolder.Utils;
using NyFolder.Protocol;

namespace NyFolder.Plugins.DownloadManager.GUI {
	public class Window : Gtk.Window {
		// ============================================
		// PUBLIC Events
		// ============================================
		public event BlankEventHandler RemoveEvent = null;

		// ============================================
		// PROTECTED Members
		// ============================================
		protected FileProgressViewer fpViewer;
		protected Gtk.HButtonBox hbuttonBox;
		protected Gtk.VBox vbox;

		// ============================================
		// PRIVATE Members
		// ============================================

		// ============================================
		// PUBLIC Constructors
		// ============================================
		public Window() : base("Downloads Manager") {
			// Initialize Window Properties
			SetDefaultSize(380, 260);

			// Initialize VBox
			this.vbox = new Gtk.VBox(false, 2);
			this.Add(this.vbox);

			// Initialize File Progress Part
			this.fpViewer = new FileProgressViewer();
			this.fpViewer.Delete += new BlankEventHandler(OnDeleteClicked);
			this.vbox.PackStart(this.fpViewer, true, true, 2);

			// Show All
			this.ShowAll();
		}

		// ============================================
		// PUBLIC Methods
		// ============================================

		// ============================================
		// PRIVATE (Methods) Event Handlers
		// ============================================

		// ============================================
		// PRIVATE (Methods) Menu Events Handler
		// ============================================
		private void OnDeleteClicked (object sender) {
			if (RemoveEvent != null) RemoveEvent(sender);
		}

		// ============================================
		// PRIVATE Methods
		// ============================================

		// ============================================
		// PUBLIC Properties
		// ============================================
		public FileProgressViewer Viewer {
			get { return(this.fpViewer); }
		}
	}
}
