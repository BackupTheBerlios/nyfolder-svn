/* [ Plugins/Search/Search.cs ] NyFolder (Search Plugin)
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

using NyFolder;
using NyFolder.GUI;
using NyFolder.Utils;
using NyFolder.Protocol;

namespace NyFolder.Plugins.Search {
	public class Search : Plugin {
		// ============================================
		// PUBLIC Events
		// ============================================

		// ============================================
		// PROTECTED Members
		// ============================================
		protected INyFolder nyFolder = null;

		// ============================================
		// PRIVATE Members
		// ============================================

		// ============================================
		// PUBLIC Constructors
		// ============================================
		public Search() {
			// Initialize Search Stock
			Gdk.Pixbuf pixbuf;
			pixbuf = new Gdk.Pixbuf(null, "Search.png");
			StockIcons.AddToStock("Search", pixbuf);
			StockIcons.AddToStockImages("Search", pixbuf);
		}

		// ============================================
		// PUBLIC Methods
		// ============================================
		public override void Initialize (INyFolder nyFolder) {
			this.nyFolder = nyFolder;
			this.nyFolder.MainWindowStarted += new EventHandler(OnMainWindowStarted);
		}

		// ============================================
		// PROTECTED (Methods) Event Handlers
		// ============================================
		protected void OnMainWindowStarted (object sender, EventArgs args) {
			// Initialize Talk Component
			InitializeSearchMenu();

			//SetSensitiveTalkMenu(P2PManager.IsListening());
		}

		protected void OnFindFile (object sender, EventArgs args) {
			new GUI.Window();
		}

		// ============================================
		// PRIVATE Methods
		// ============================================
		private void InitializeSearchMenu() {
			ActionEntry[] entries = new ActionEntry[] {
				new ActionEntry("Search", "Search", "Search", null, 
								"Find File...", new EventHandler(OnFindFile)),
			};

			string ui = "<ui>" +
						"  <menubar name='MenuBar'>" +
						"    <menu action='ToolMenu'>" +
						"      <menuitem action='Search' />" +
						"    </menu>" +
						"  </menubar>" +
						"</ui>";

			nyFolder.Window.Menu.AddMenus(ui, entries);
		}

		// ============================================
		// PUBLIC Properties
		// ============================================
	}
}
