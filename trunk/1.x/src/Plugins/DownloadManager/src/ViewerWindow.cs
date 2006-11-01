/* [ Plugins/DownloadManager/ViewerWindow.cs ] NyFolder Viewer Window
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

using NyFolder;
using NyFolder.GUI;
using NyFolder.Utils;
using NyFolder.Protocol;
using NyFolder.GUI.Base;

namespace NyFolder.Plugins.DownloadManager {
	public class ViewerWindow : Gtk.Window {
		// ============================================
		// PUBLIC Events
		// ============================================

		// ============================================
		// PROTECTED Members
		// ============================================

		// ============================================
		// PRIVATE Members
		// ============================================
		private DownloadViewer downloadViewer;
		private UploadViewer uploadViewer;
		private Gtk.HButtonBox hButtonBox;
		private Gtk.Notebook notebook;
		private TabLabel tabDownloads;
		private TabLabel tabUploads;
		private Gtk.VBox vbox;

		// ============================================
		// PUBLIC Constructors
		// ============================================
		public ViewerWindow() : base("Download/Upload Viewer") {
			// Initialize Window
			SetDefaultSize(400, 300);

			// Initialize VBox
			this.vbox = new Gtk.VBox(false, 2);
			this.Add(this.vbox);

			// Initialize Notebook
			this.notebook = new Gtk.Notebook();
			this.notebook.ShowTabs = true;
			this.notebook.Scrollable = true;
			this.vbox.PackStart(this.notebook, true, true, 2);

			// Add Download Viewer
			this.tabDownloads = new TabLabel("<b>Downloads</b>", StockIcons.GetImage("Download", 22));
			this.tabDownloads.Button.Sensitive = false;
			this.downloadViewer = new DownloadViewer();
			this.notebook.AppendPage(this.downloadViewer, this.tabDownloads);

			// Add Uploads Viewer
			this.tabUploads = new TabLabel("<b>Uploads</b>", StockIcons.GetImage("Upload", 22));
			this.tabUploads.Button.Sensitive = false;
			this.uploadViewer = new UploadViewer();
			this.notebook.AppendPage(this.uploadViewer, this.tabUploads);

			// HButton Box
			this.hButtonBox = new Gtk.HButtonBox();
			this.hButtonBox.Spacing = 4;
			this.hButtonBox.Layout = ButtonBoxStyle.End;
			this.hButtonBox.LayoutStyle = ButtonBoxStyle.End;
			this.vbox.PackStart(this.hButtonBox, false, false, 2);

			Gtk.Button button;

			button = new Gtk.Button(Gtk.Stock.Clear);
			button.Clicked += new EventHandler(OnButtonClear);
			this.hButtonBox.PackStart(button, false, false, 2);

			// Show All
			this.ShowAll();			
		}

		// ============================================
		// PUBLIC Methods
		// ============================================

		// ============================================
		// PROTECTED (Methods) Event Handlers
		// ============================================
		private void OnButtonClear (object sender, EventArgs args) {
		Gtk.Application.Invoke(delegate {
			if (notebook.CurrentPageWidget == downloadViewer) {
				downloadViewer.Clear();
			} else if (notebook.CurrentPageWidget == uploadViewer) {
				uploadViewer.Clear();
			}
		});
		}

		// ============================================
		// PRIVATE Methods
		// ============================================

		// ============================================
		// PUBLIC Properties
		// ============================================
	}
}
