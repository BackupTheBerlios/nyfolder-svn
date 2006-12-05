/* [ Plugins/ImagePreview.cs ] NyFolder Image Preview Plugin
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
using System.IO;

using Niry;
using Niry.Utils;
using Niry.Network;
using Niry.GUI.Gtk2;

using NyFolder;
using NyFolder.GUI;
using NyFolder.Utils;
using NyFolder.Protocol;
using NyFolder.GUI.Base;
using NyFolder.PluginLib;

namespace NyFolder.Plugins.DownloadManager {
	/// Download Manager (Plugin)
	public class DownloadManager : Plugin {
		// ============================================
		// PUBLIC Const
		// ============================================

		// ============================================
		// PRIVATE Members
		// ============================================
		private ViewerWindow viewerWindow = null;
		private INyFolder nyFolder = null;

		// ============================================
		// PUBLIC Constructors
		// ============================================
		/// Create Download Manager
		public DownloadManager() {
			// Initialize Plugin Info
			this.pluginInfo = new Info();

			// Initialize Talk Stock
			Gdk.Pixbuf pixbuf;
			pixbuf = new Gdk.Pixbuf(null, "Download.png");
			StockIcons.AddToStock("Download", pixbuf);
			StockIcons.AddToStockImages("Download", pixbuf);

			pixbuf = new Gdk.Pixbuf(null, "Upload.png");
			StockIcons.AddToStockImages("Upload", pixbuf);

			pixbuf = new Gdk.Pixbuf(null, "DlTrash.png");
			StockIcons.AddToStockImages("DlTrash", pixbuf);
		}

		// ============================================
		// PUBLIC Methods
		// ============================================
		/// Initialize Download Manager Plugin
		public override void Initialize (INyFolder iNyFolder) {
			this.nyFolder = iNyFolder;

			// Initialize GUI Events
			this.nyFolder.MainWindowStarted += new BlankEventHandler(OnMainWindowStarted);
			this.nyFolder.MainWindowClosed += new BlankEventHandler(OnMainWindowClosed);
		}

		/// UnInitialize Download Manager Plugin
		public override void Uninitialize() {
		}

		// ============================================
		// PRIVATE (Methods) Event Handlers
		// ============================================
		private void OnMainWindowStarted (object sender) {
			// Initialize GUI Components
			InitializeMenu();

			// Initialize Sensitive Menu
			P2PManager.StatusChanged += new BoolEventHandler(OnP2PStatusChanged);
			SetSensitiveMenu(P2PManager.IsListening());
		}

		private void OnMainWindowClosed (object sender) {
			OnDlWinClose(null, null);
		}

		private void OnP2PStatusChanged (object sender, bool isOnline) {
			if (nyFolder.MainWindow != null)
				SetSensitiveMenu(isOnline);
		}

		private void OnDlManager (object sender, EventArgs args) {
			if (viewerWindow == null) {
				viewerWindow = new ViewerWindow();
				viewerWindow.DeleteEvent += new DeleteEventHandler(OnDlWinClose);
				viewerWindow.ShowAll();
			}
			viewerWindow.GrabFocus();
		}

		private void OnDlWinClose (object sender, DeleteEventArgs args) {
			if (viewerWindow != null) {
				viewerWindow.DeleteEvent += new DeleteEventHandler(OnDlWinClose);
				viewerWindow.Destroy();
			}
			viewerWindow = null;
		}

		// ============================================
		// PRIVATE Methods
		// ============================================
		private void InitializeMenu() {
			ActionEntry[] entries = new ActionEntry[] {
				new ActionEntry("DownloadManager", "Download", "Download Manager", null, 
								"Open Download Manager", new EventHandler(OnDlManager))
			};

			string ui = "<ui>" +
						"  <menubar name='MenuBar'>" +
						"    <menu action='ToolMenu'>" +
						"      <menuitem action='DownloadManager' />" +
						"    </menu>" +
						"  </menubar>" +
						"</ui>";

			nyFolder.MainWindow.Menu.AddMenus(ui, entries);
		}

		private void SetSensitiveMenu (bool sensitive) {
			nyFolder.MainWindow.Menu.SetSensitive("/MenuBar/ToolMenu/DownloadManager", sensitive);
		}

		// ============================================
		// PUBLIC Properties
		// ============================================
	}
}
