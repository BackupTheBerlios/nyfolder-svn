/* [ GUI/MenuManager.cs ] NyFolder Menu Manager
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
using System.Text;

using NyFolder;
using NyFolder.Utils;
using NyFolder.GUI.Base;

namespace NyFolder.GUI {
	/// Login Dialog Menu Manager
	public class MenuManager : NyFolder.GUI.Base.UIManager {
		// ============================================
		// PUBLIC Constructors
		// ============================================
		/// Create New Login Dialog UIManager
		public MenuManager() : base("MenuGroup") {
			AddMenus(GetUIString(), 
					 GetActionEntries(),
					 GetToggleActionEntries());
		}

		// ============================================
		// PRIVATE Methods
		// ============================================
		private string GetUIString() {
			StringBuilder sb = new StringBuilder();
			sb.Append("<ui>");
			sb.Append("  <menubar name='MenuBar'>");
			sb.Append("    <menu action='FileMenu'>");
			sb.Append("      <separator />");
			sb.Append("      <menuitem action='ProxySettings'/>");
			sb.Append("      <separator />");
			sb.Append("      <menuitem action='Logout'/>");
			sb.Append("      <menuitem action='Quit'/>");
			sb.Append("    </menu>");
			sb.Append("    <menu action='ViewMenu'>");
			sb.Append("      <menuitem action='Refresh'/>");
			sb.Append("      <separator />");
			sb.Append("      <menuitem action='ViewToolBar'/>");
			sb.Append("      <menuitem action='ViewUserPanel'/>");
			sb.Append("    </menu>");
			sb.Append("    <menu action='GoMenu'>");
			sb.Append("      <menuitem action='GoUp'/>");
			sb.Append("      <menuitem action='GoHome'/>");
			sb.Append("      <separator />");
			sb.Append("      <menuitem action='GoNetwork'/>");
			sb.Append("      <menuitem action='GoMyFolder'/>");
			sb.Append("    </menu>");
			sb.Append("    <menu action='ToolMenu'>");
			sb.Append("    </menu>");
			sb.Append("    <menu action='NetworkMenu'>");
			sb.Append("      <menuitem action='NetOnline'/>");
			sb.Append("      <menuitem action='SetPort'/>");
			sb.Append("      <separator />");
			sb.Append("      <menuitem action='AddPeer'/>");
			sb.Append("      <menuitem action='RmPeer'/>");
			sb.Append("    </menu>");
			sb.Append("    <menu action='HelpMenu' position='bot'>");
			sb.Append("      <menuitem action='About' position='bot'/>");
			sb.Append("    </menu>");
			sb.Append("  </menubar>");
			sb.Append("  <toolbar name='ToolBar'>");
			sb.Append("    <toolitem action='GoUp'/>");
			sb.Append("    <toolitem action='Refresh'/>");
			sb.Append("    <toolitem action='GoHome'/>");
			sb.Append("    <separator />");
			sb.Append("    <toolitem action='GoNetwork'/>");
			sb.Append("    <toolitem action='GoMyFolder'/>");
			sb.Append("  </toolbar>");
			sb.Append("</ui>");
			return(sb.ToString());
		}

		private ActionEntry[] GetActionEntries() {
			ActionEntry[] entries = new ActionEntry[] {
				new ActionEntry("FileMenu", null, "_File", null, null, null),
				new ActionEntry("ViewMenu", null, "_View", null, null, null),
				new ActionEntry("GoMenu", null, "_Go", null, null, null),
				new ActionEntry("ToolMenu", null, "_Tool", null, null, null),
				new ActionEntry("NetworkMenu", null, "_Network", null, null, null),
				new ActionEntry("HelpMenu", null, "_Help", null, null, null),
	
				// File Menu
				new ActionEntry("ProxySettings", "Proxy", "Proxy Settings", null, 
								"Setup Proxy", new EventHandler(ActionActivated)),
				new ActionEntry("Logout", "Logout", "Logout", null, 
								"Logout", new EventHandler(ActionActivated)),
				new ActionEntry("Quit", Gtk.Stock.Quit, "Quit", "<control>Q", 
								"Quit NyFolder", new EventHandler(ActionActivated)),
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
								"About NyFolder", new EventHandler(ActionActivated)),
			};
			return(entries);
		}

		private ToggleActionEntry[] GetToggleActionEntries() {
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
			return(toggleEntries);
		}
	}
}
