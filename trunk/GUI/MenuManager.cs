/* [ GUI/MenuManager.cs ] NyFolder (Menu & Toolbar UIManager)
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
using Niry.GUI.Gtk2;

namespace NyFolder.GUI {
	public delegate void RightMenuHandler (object sender, PopupMenu menu);

	public sealed class MenuManager : Gtk.UIManager {
		// ============================================
		// PUBLIC Events
		// ============================================
		public event EventHandler Activated = null;

		// ============================================
		// PRIVATE Members
		// ============================================
		private ActionGroup actionGroup;

		private const string uiInfo = 
			"<ui>" +
			"  <menubar name='MenuBar'>" +
			"    <menu action='FileMenu'>" +
			"      <separator />" +
			"      <menuitem action='ProxySettings'/>" +
			"      <separator />" +
			"      <menuitem action='Quit'/>" +
			"    </menu>" +
			"    <menu action='ViewMenu'>" +
			"      <menuitem action='Refresh'/>" +
			"      <separator />" +
			"      <menuitem action='ViewToolBar'/>" +
			"      <menuitem action='ViewUserPanel'/>" +
			"    </menu>" +
			"    <menu action='GoMenu'>" +
			"      <menuitem action='GoUp'/>" +
			"      <menuitem action='GoHome'/>" +
			"      <separator />" +
			"      <menuitem action='GoNetwork'/>" +
			"      <menuitem action='GoMyFolder'/>" +
			"    </menu>" +
			"    <menu action='NetworkMenu'>" +
			"      <menuitem action='NetOnline'/>" +
			"      <menuitem action='SetPort'/>" +
			"      <separator />" +
			"      <menuitem action='AddPeer'/>" +
			"      <menuitem action='RmPeer'/>" +
			"    </menu>" +
			"    <menu action='HelpMenu'>" +
			"      <menuitem action='About'/>" +
			"    </menu>" +
			"  </menubar>" +
			"  <toolbar name='ToolBar'>" +
			"    <toolitem action='GoUp'/>" +
			"    <toolitem action='Refresh'/>" +
			"    <toolitem action='GoHome'/>" +
			"    <separator />" +
			"    <toolitem action='GoNetwork'/>" +
			"    <toolitem action='GoMyFolder'/>" +
			"  </toolbar>" +
			"</ui>";

		// ============================================
		// PUBLIC Constructors
		// ============================================
		public MenuManager() {
			ActionEntry[] entries = new ActionEntry[] {
				new ActionEntry("FileMenu", null, "_File", null, null, null),
				new ActionEntry("ViewMenu", null, "_View", null, null, null),
				new ActionEntry("GoMenu", null, "_Go", null, null, null),
				new ActionEntry("NetworkMenu", null, "_Network", null, null, null),
				new ActionEntry("HelpMenu", null, "_Help", null, null, null),
	
				// File Menu
				new ActionEntry("ProxySettings", "Proxy", "Proxy Settings", null, 
								"Setup Proxy", new EventHandler(ActionActivated)),
				new ActionEntry("Quit", Gtk.Stock.Quit, "Quit", "<control>Q", 
								"Quit Shared Folder", new EventHandler(ActionActivated)),
				// View Menu
				new ActionEntry("Refresh", Gtk.Stock.Refresh, "Refresh", null, 
								null, new EventHandler(ActionActivated)),

				// Go Menu
				new ActionEntry("GoUp", Gtk.Stock.GoUp, "UP", null, 
								"Open The Parent Folder", new EventHandler(ActionActivated)),
				new ActionEntry("GoHome", Gtk.Stock.Home, "Home", null, 
								"Open The Root Directory", new EventHandler(ActionActivated)),
				new ActionEntry("GoMyFolder", "StockMyFolder", "MyFolder", null, 
								"My Shared Folder", new EventHandler(ActionActivated)),
				new ActionEntry("GoNetwork", "StockNetwork", "Network", null, 
								"View Network", new EventHandler(ActionActivated)),
				// Network
				new ActionEntry("SetPort", Gtk.Stock.Preferences, "Set P2P Port", null, 
								"Set P2P Port", new EventHandler(ActionActivated)),
				new ActionEntry("AddPeer", Gtk.Stock.Network, "Add Peer", null, 
								"Add New Peer", new EventHandler(ActionActivated)),
				new ActionEntry("RmPeer", Gtk.Stock.Delete, "Remove Peer", null, 
								"Remove Peer", new EventHandler(ActionActivated)),
				new ActionEntry("DownloadManager", "Download", "Download Manager", null,
								"Download Manager", new EventHandler(ActionActivated)),
				// Help Menu
				new ActionEntry("About", Gtk.Stock.About, "About", null, 
								"About Shared Folder", new EventHandler(ActionActivated)),
			};

			// Toggle items
			ToggleActionEntry[] toggleEntries = new ToggleActionEntry[] {
				// View
				new ToggleActionEntry("ViewToolBar", null, "ToolBar", null,
										null, new EventHandler(ActionActivated), true),
				new ToggleActionEntry("ViewUserPanel", null, "User Panel", null,
										null, new EventHandler(ActionActivated), true),
				// Network
				new ToggleActionEntry("NetOnline", null, "Online", null,
										null, new EventHandler(ActionActivated), false)
			};

			actionGroup = new ActionGroup("group");
			actionGroup.Add(entries);
			actionGroup.Add(toggleEntries);
			InsertActionGroup(actionGroup, 0);
			AddUiFromString(uiInfo);
		}

		// ============================================
		// PUBLIC Methods
		// ============================================
		public void AddMenus (string ui, ActionEntry[] entries) {
			AddUiFromString(ui);
			actionGroup.Add(entries);
			EnsureUpdate();
		}

		public void SetSensitive (string path, bool sensitive) {
			Widget widget = GetWidget(path);
			if (widget != null) widget.Sensitive = sensitive;
		}

		// ============================================
		// PRIVATE STATIC (Methods) Event Handler
		// ============================================
		private void ActionActivated (object sender, EventArgs args) {
			if (Activated != null) Activated(sender, args);				
		}

		// ============================================
		// PUBLIC Properties
		// ============================================
		public ActionGroup GroupAction {
			get { return(this.actionGroup); }
		}
	}
}
