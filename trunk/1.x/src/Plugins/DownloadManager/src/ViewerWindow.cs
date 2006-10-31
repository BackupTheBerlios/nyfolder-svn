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
		private Gtk.Notebook notebook;
		private TabLabel tabDownloads;
		private TabLabel tabUploads;

		// ============================================
		// PUBLIC Constructors
		// ============================================
		public ViewerWindow() : base("Download/Upload Viewer") {
			// Initialize Notebook
			this.notebook = new Gtk.Notebook();
			this.notebook.ShowTabs = true;
			this.notebook.Scrollable = true;
			Add(this.notebook);

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
		}

		// ============================================
		// PUBLIC Methods
		// ============================================

		// ============================================
		// PROTECTED (Methods) Event Handlers
		// ============================================

		// ============================================
		// PRIVATE Methods
		// ============================================

		// ============================================
		// PUBLIC Properties
		// ============================================
	}
}
