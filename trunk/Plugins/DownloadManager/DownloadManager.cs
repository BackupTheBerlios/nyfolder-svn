/* [ Plugins/DownloadManager/DownloadManager.cs ] NyFolder (Plugin)
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
using NyFolder.GUI;
using NyFolder.Utils;
using NyFolder.Protocol;

namespace NyFolder.Plugins.DownloadManager {
	public class DownloadManager : Plugin {
		// ============================================
		// PUBLIC Events
		// ============================================

		// ============================================
		// PROTECTED Members
		// ============================================
		protected GUI.Window dlWindow = null;
		protected INyFolder nyFolder = null;
		protected GUI.Glue guiGlue = null;

		// ============================================
		// PRIVATE Members
		// ============================================

		// ============================================
		// PUBLIC Constructors
		// ============================================
		public DownloadManager() {
			this.dlWindow = null;
		}

		~DownloadManager() {
			this.nyFolder.MainWindowStarted -= new BlankEventHandler(OnMainWindowStarted);
			this.nyFolder.QuittingApplication -= new BlankEventHandler(OnMainAppQuit);
		}

		// ============================================
		// PUBLIC Methods
		// ============================================
		public override void Initialize (INyFolder nyFolder) {
			this.nyFolder = nyFolder;
			this.nyFolder.MainWindowStarted += new BlankEventHandler(OnMainWindowStarted);
			this.nyFolder.QuittingApplication += new BlankEventHandler(OnMainAppQuit);
		}

		// ============================================
		// PROTECTED (Methods) Event Handlers
		// ============================================
		protected void OnMainWindowStarted (object sender) {
			// Initialize Talk Component
			InitializeMenu();

			// Initialize Sensitive Menu
			P2PManager.StatusChanged += new BoolEventHandler(OnP2PStatusChanged);
			SetSensitiveMenu(P2PManager.IsListening());
		}

		protected void OnMainAppQuit (object sender) {
			// Remove Events
			P2PManager.StatusChanged -= new BoolEventHandler(OnP2PStatusChanged);			

			// Destroy Download Manager Window
			if (this.guiGlue != null) this.guiGlue = null;
			if (this.dlWindow != null) {
				this.dlWindow.Destroy();
				this.dlWindow = null;
			}
		}

		protected void OnP2PStatusChanged (object sender, bool isOnline) {
			SetSensitiveMenu(isOnline);			
		}

		protected void OnDlManager (object sender, EventArgs args) {
			if (this.dlWindow == null) {
				this.dlWindow = new GUI.Window();
				this.guiGlue = new GUI.Glue(this.dlWindow);
				this.dlWindow.DeleteEvent += new DeleteEventHandler(OnDlWinClose);
			}
			this.dlWindow.ShowAll();
		}

		protected void OnDlWinClose (object sender, DeleteEventArgs args) {
			this.guiGlue = null;
			this.dlWindow = null;
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

			nyFolder.Window.Menu.AddMenus(ui, entries);
		}

		private void SetSensitiveMenu (bool sensitive) {
			nyFolder.Window.Menu.SetSensitive("/MenuBar/ToolMenu/DownloadManager", sensitive);
		}

		// ============================================
		// PUBLIC Properties
		// ============================================
	}
}
