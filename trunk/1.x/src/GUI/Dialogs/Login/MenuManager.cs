/* [ GUI/Dialogs/Login/MenuManager.cs ] NyFolder Login Dialog Menu Manager
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
using System.Text;

using NyFolder;
using NyFolder.Utils;
using NyFolder.GUI.Base;

namespace NyFolder.Dialogs.Login {
	/// Login Dialog Menu Manager
	public class MenuManager : UIManager {
		// ============================================
		// PUBLIC Constructors
		// ============================================
		/// Create New Login Dialog UIManager
		public MenuManager() : base("LoginMenuGroup") {
			AddMenus(GetUIString(), GetActionEntries());
		}

		// ============================================
		// PRIVATE Methods
		// ============================================
		private string GetUIString() {
			StringBuilder sb = new sb();
			sb.Append("<ui>");
			sb.Append("  <menubar name='MenuBar'>");
			sb.Append("    <menu action='FileMenu'>");
			sb.Append("      <separator />");
			sb.Append("      <menuitem action='ProxySettings'/>");
			sb.Append("      <separator />");
			sb.Append("      <menuitem action='Quit'/>");
			sb.Append("    </menu>");
			sb.Append("    <menu action='HelpMenu' position='bot'>");
			sb.Append("      <menuitem action='About' position='bot'/>");
			sb.Append("    </menu>");
			sb.Append("  </menubar>");
			sb.Append("</ui>");
			return(sb.ToString());
		}

		private ActionEntry[] GetActionEntries() {
			ActionEntry[] entries = new ActionEntry[] {
				new ActionEntry("FileMenu", null, "_File", null, null, null),
				new ActionEntry("HelpMenu", null, "_Help", null, null, null),
	
				// File Menu
				new ActionEntry("ProxySettings", "Proxy", "Proxy Settings", null, 
								"Setup Proxy", new EventHandler(ActionActivated)),
				new ActionEntry("Quit", Gtk.Stock.Quit, "Quit", "<control>Q", 
								"Quit NyFolder", new EventHandler(ActionActivated)),

				// Help Menu
				new ActionEntry("About", Gtk.Stock.About, "About", null, 
								"About NyFolder", new EventHandler(ActionActivated)),
			};
			return(entries);
		}
	}
}
