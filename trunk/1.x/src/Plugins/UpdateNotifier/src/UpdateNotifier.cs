/* [ Plugins/UpdateNotifer/UpdateNotifer.cs ] NyFolder Update Notifer Plugin
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
using NyFolder.Utils;
using NyFolder.Protocol;

namespace NyFolder.Plugins.UpdateNotifier {
	public class UpdateNotifier {
		// ============================================
		// PUBLIC Events
		// ============================================

		// ============================================
		// PROTECTED Members
		// ============================================

		// ============================================
		// PRIVATE Members
		// ============================================
		private INyFolder nyFolder = null;

		// ============================================
		// PUBLIC Constructors
		// ============================================
		public UpdateNotifier() {
			// Initialize Stock
			Gdk.Pixbuf pixbuf;
			pixbuf = new Gdk.Pixbuf(null, "UpdateNotifier.png");
			StockIcons.AddToStock("UpdateNotifier", pixbuf);
			StockIcons.AddToStockImages("UpdateNotifier", pixbuf);
		}

		// ============================================
		// PUBLIC Methods
		// ============================================
		/// Initialize Update Notifier Plugin
		public override void Initialize (INyFolder iNyFolder) {
			this.nyFolder = iNyFolder;
		}

		// ============================================
		// PROTECTED (Methods) Event Handlers
		// ============================================
		protected void CheckForUpdates() {
			try {
				// Make Http Request
				WebRequest request = WebRequest.Create(url);
				request.Timeout = 5000;

				if ((proxy = Utils.Proxy.GetConfig()) != null)
					request.Proxy = proxy;

				// Wait Http Response
				WebResponse response = request.GetResponse();
				CheckForUpdates(response.GetResponseStream());
				response.Close();
			} catch {}
		}

		protected void CheckForUpdates (Stream stream) {
			XmlTextReader xmlReader = new XmlTextReader(stream);
			string version = null;
			string infos = null;

			while (xmlReader.Read()) {
				if (reader.NodeType != XmlNodeType.Element) continue;

				switch (xmlReader.Name) {
					case "version":
						version = xmlReader.Value;
						break;
					case "infos":
						infos = xmlReader.Value;
						break;
				}
			}
			xmlReader.Close();

			CheckForUpdates(version, infos);
		}

		protected void CheckForUpdates (string version, string infos) {
			if (version == NyFolder.Info.Version) return;

			Gtk.Image updateImage = StockIcons.GetImage("UpdateNotifier", 22);
			Gtk.Button updateBtn = new Gtk.Button(updateImage);
			updateBtn.Relief = ReliefStyle.None;
			updateBtn.Clicked += {
				UpdateInfo updateDialog = new UpdateInfo();
				updateDialog.Version = version;
				updateDialog.Version = infos;
			};

			// Show Update Icon into Main Window
			if (nyFolder.MainWindow != null) {
				nyFolder.MainWindow.MenuBarHBox.PackEnd(updateBtn, false, false, 2);
			}

			// Show Update Icon into Login Dialog
			if (nyFolder.LoginDialog != null) {
				nyFolder.MainWindow.MenuBarHBox.PackEnd(updateBtn, false, false, 2);
			}
		}

		// ============================================
		// PRIVATE Methods
		// ============================================

		// ============================================
		// PUBLIC Properties
		// ============================================
	}
}
