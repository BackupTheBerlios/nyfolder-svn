/* [ Plugins/TrayIcon.cs ] NyFolder Tray Icon Plugin
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
using Niry.GUI.Gtk2;

using NyFolder;
using NyFolder.Utils;
using NyFolder.GUI.Base;
using NyFolder.PluginLib;

namespace NyFolder.Plugins.TrayIcon {
	/// Tray Icon (Plugin)
	public class TrayIcon : Plugin {
		// ============================================
		// PUBLIC Events
		// ============================================
		/// Event Raised When Tray Popup Menu is Created
		public event RightMenuHandler PopupMenu = null;

		// ============================================
		// PRIVATE Members
		// ============================================
		private StatusIcon statusIcon = null;
		private INyFolder nyFolder = null;

		// ============================================
		// PUBLIC Constructors
		// ============================================
		/// Create New Tray Icon
		public TrayIcon() {
			// Initialize Plugin Info
			this.pluginInfo = new Info();

			// Initialize Status Icon
			Gdk.Pixbuf pixbuf = new Gdk.Pixbuf(null, "TrayIcon.png");
			this.statusIcon = new StatusIcon(pixbuf);
			this.statusIcon.Visible = true;
			this.statusIcon.Tooltip = "NyFolder, Share Your Files...";

			// Initialize Status Icon Events
			this.statusIcon.Activate += new EventHandler(OnStatusIconActivated);
			this.statusIcon.PopupMenu += new PopupMenuHandler(OnStatusIconPopupMenu);
		}

		// ============================================
		// PUBLIC Methods
		// ============================================
		/// Initialize Tray Icon Plugin
		public override void Initialize (INyFolder iNyFolder) {
			this.nyFolder = iNyFolder;
		}

		/// UnInitialize Tray Icon Plugin
		public override void Uninitialize() {
			this.statusIcon = null;
		}

		// ============================================
		// PRIVATE (Methods) Event Handlers
		// ============================================
		private void OnStatusIconActivated (object sender, EventArgs args) {
			OnMenuShowHideWin(sender, args);
		}

		private void OnStatusIconPopupMenu (object sender, PopupMenuArgs args) {
			PopupMenu menu = new PopupMenu();

			// Append Menu Items
			if (PopupMenu != null) PopupMenu(this, menu);

			if (nyFolder.MainWindow != null)
				menu.AddImageItem("Logout", new EventHandler(OnMenuLogout));

			menu.AddSeparator();

			// Show/Hide Login Dialog Check Box
			if (nyFolder.MainWindow != null) {
				menu.AddCheckItem("Show/Hide Window",
								  nyFolder.MainWindow.Visible,
								  new EventHandler(OnMenuShowHideWin));
			}

			// Show/Hide Login Dialog Check Box
			if (nyFolder.LoginDialog != null) {
				menu.AddCheckItem("Show/Hide Dialog",
								  nyFolder.LoginDialog.Visible, 
								  new EventHandler(OnMenuShowHideWin));
			}

			menu.AddImageItem(Gtk.Stock.Quit, new EventHandler(OnMenuQuit));

			menu.ShowAll();
			menu.Popup();
		}

		// ============================================
		// PRIVATE (Methods) Menu Event Handlers 
		// ============================================
		private void OnMenuLogout (object sender, EventArgs args) {
			this.nyFolder.Logout();
		}

		private void OnMenuQuit (object sender, EventArgs args) {
			this.nyFolder.Quit();
			Gtk.Application.Quit();
		}

		private void OnMenuShowHideWin (object sender, EventArgs args) {
			// Show/Hide Main Window
			if (nyFolder.MainWindow != null)
				nyFolder.MainWindow.Visible = !nyFolder.MainWindow.Visible;

			// Show/Hide Login Dialog
			if (nyFolder.LoginDialog != null)
				nyFolder.LoginDialog.Visible = !nyFolder.LoginDialog.Visible;
		}

		// ============================================
		// PUBLIC Properties
		// ============================================
		/// Get Status Icon Reference
		public StatusIcon StatusIcon {
			get { return(this.statusIcon); }
		}
	}
}
