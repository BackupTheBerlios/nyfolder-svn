/* [ Plugins/TrayIcon/TrayIcon.cs ] NyFolder (Tray Icon Plugin)
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
using NyFolder.GUI;
using NyFolder.Utils;
using NyFolder.Protocol;

namespace NyFolder.Plugins.TrayIcon {
	public class TrayIcon : Plugin {
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
		protected NotificationArea notificationArea = null;

		// ============================================
		// PUBLIC Constructors
		// ============================================

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
			EventBox eb = new EventBox();
			eb.Add(new Image("NyFolderIcon", IconSize.Menu));

			// hooking event
			eb.ButtonPressEvent += new ButtonPressEventHandler(OnImageClick);
			notificationArea = new NotificationArea("NyFolder");
			notificationArea.Add(eb);

			// showing the trayicon
			notificationArea.ShowAll();
		}

		protected void OnMainAppQuit (object sender) {
			if (notificationArea != null)
				notificationArea.Destroy();
			notificationArea = null;
		}

		// ============================================
		// PRIVATE (Methods) Event Handlers
		// ============================================
		private void OnImageClick (object o, ButtonPressEventArgs args) {
			if (args.Event.Button == 3) {
				PopupMenu menu = new PopupMenu();
				menu.AddImageItem(Gtk.Stock.Quit, new EventHandler(OnAppQuit));
				menu.ShowAll();
				menu.Popup();
			}
   		}

		private void OnAppQuit (object sender, EventArgs args) {
			Gtk.Application.Quit();
		}

		// ============================================
		// PUBLIC Properties
		// ============================================
	}
}
