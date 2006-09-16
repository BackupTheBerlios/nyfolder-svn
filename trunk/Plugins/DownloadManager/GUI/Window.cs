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
		protected FileProgressViewer fpViewerDownload;
		protected FileProgressViewer fpViewerUpload;
		protected Gtk.HButtonBox hbuttonBox;
		protected Gtk.VBox vbox;

		// ============================================
		// PRIVATE Members
		// ============================================
		private Gtk.VPaned vpaned;

		// ============================================
		// PUBLIC Constructors
		// ============================================
		public Window() : base("Downloads Manager") {
			// Initialize Window Properties
			SetDefaultSize(380, 260);

			// Initialize VBox
			this.vbox = new Gtk.VBox(false, 2);
			this.Add(this.vbox);

			// Initialize VPaned
			this.vpaned = new Gtk.VPaned();
			this.vbox.PackStart(this.vpaned, true, true, 2);

			// Initialize Download Part
			this.fpViewerDownload = new FileProgressViewer("Downloads");
			this.vpaned.Pack1(this.fpViewerDownload, true, true);

			// Initialize Upload Part
			this.fpViewerUpload = new FileProgressViewer("Uploads");
			this.vpaned.Pack2(this.fpViewerUpload, true, true);

			// Initialize HSeparator
			this.vbox.PackStart(new Gtk.HSeparator(), false, false, 2);

			// Initialize ToolBar
			this.hbuttonBox = new Gtk.HButtonBox();
			this.hbuttonBox.Layout = Gtk.ButtonBoxStyle.End;
			this.vbox.PackStart(this.hbuttonBox, false, false, 2);

			// Remove
			Gtk.Button button = new Gtk.Button(Gtk.Stock.Remove);
			button.Clicked += new EventHandler(OnRemove);
			this.hbuttonBox.PackEnd(button, false, false, 2);

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
		private void OnRemove (object sender, EventArgs args) {
			if (RemoveEvent != null) RemoveEvent(this);
		}

		// ============================================
		// PRIVATE Methods
		// ============================================

		// ============================================
		// PUBLIC Properties
		// ============================================
		public FileProgressViewer DownloadsViewer {
			get { return(this.fpViewerDownload); }
		}

		public FileProgressViewer UploadsViewer {
			get { return(this.fpViewerUpload); }
		}
	}
}
