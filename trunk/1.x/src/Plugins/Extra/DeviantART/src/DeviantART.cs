/* [ Plugins/DeviantART.cs ] NyFolder DeviantART Plugin
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
using System.Net;

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

namespace NyFolder.Plugins.DeviantART {
	/// DeviantART (Plugin)
	public class DeviantART : Plugin {
		// ============================================
		// PUBLIC Const
		// ============================================
		public const int ThumbWidth = 100;
		public const int ThumbHeight = 52;

		// ============================================
		// PRIVATE Members
		// ============================================
		private NotebookViewer notebookViewer = null;
		private INyFolder nyFolder = null;

		// ============================================
		// PUBLIC Constructors
		// ============================================
		/// Create DeviantART Preview
		public DeviantART() {
			// Initialize Plugin Info
			this.pluginInfo = new Info();

			// Initialize Talk Stock
			Gdk.Pixbuf pixbuf;
			pixbuf = new Gdk.Pixbuf(null, "DeviantART.png");
			StockIcons.AddToStock("DeviantART", pixbuf);
			StockIcons.AddToStockImages("DeviantART", pixbuf);
		}

		// ============================================
		// PUBLIC Methods
		// ============================================
		public override void Initialize (INyFolder iNyFolder) {
			this.nyFolder = iNyFolder;

			// Initialize GUI Events
			this.nyFolder.MainWindowStarted += new BlankEventHandler(OnMainWindowStarted);
		}

		// ============================================
		// PRIVATE (Methods) Event Handlers
		// ============================================
		private void OnMainWindowStarted (object sender) {
			// Initialize GUI Components
			InitializeMenu();

			// Initialize Notebook Viewer
			notebookViewer = this.nyFolder.MainWindow.NotebookViewer;

			LoadSectionImages("customization/wallpaper/apple/");
		}

		// ============================================
		// PRIVATE Methods
		// ============================================
		private void InitializeMenu() {
			ActionEntry[] entries = new ActionEntry[] {
				new ActionEntry("DeviantARTMenu", "DeviantArt", "DeviantART", null, null, null),

				// Menu Actions
			};
			
			string ui = "<ui>" +
						"  <menubar name='MenuBar'>" +
						"    <menu action='ToolMenu'>" +
						"      <menu action='DeviantARTMenu'>" +
						"      </menu>" +
						"    </menu>" +
						"  </menubar>" +
						"</ui>";

			nyFolder.MainWindow.Menu.AddMenus(ui, entries);
		}

		private void LoadSectionImages (string section) {
			string url = "http://browse.deviantart.com/" + section;
			WebProxy proxy = Proxy.GetConfig();
			string[] images = UrlExtractor.GetImages(url, proxy);
Gtk.Application.Invoke(delegate {
			ImageViewer imageViewer = new ImageViewer();
			notebookViewer.AppendCustom(imageViewer, section, 
										new Gtk.Image("DeviantART", IconSize.Menu));

			foreach (string imgUrl in images) {
				byte[] imgData = UrlUtils.FetchPage(imgUrl, proxy);
				Gdk.Pixbuf pixbuf = new Gdk.Pixbuf(imgData);
				imageViewer.Add(imgUrl, imgUrl, pixbuf);
			}
});
		}

		private string GenerateRealImagePath (string imagePath) {
			return(Paths.UserSharedDirectory(MyInfo.Name) + imagePath);
		}

		private void LoadImage (UserInfo userInfo, string path) {
			try {
				Gdk.Pixbuf pixbuf = ImageUtils.GetPixbuf(path, ThumbWidth, ThumbHeight);
				LoadImage(userInfo, path, pixbuf);
			} catch (Exception e) {
				Debug.Log("Load {0} Image {1}: {2}", userInfo.Name, path, e.Message);
			}
		}

		private void LoadImage (UserInfo userInfo, string path, Gdk.Pixbuf pixbuf) {
			// Lookup File Iter
			FolderViewer folderViewer = notebookViewer.LookupPage(userInfo);
			if (folderViewer == null) return;

			// Get File Tree Iter
			Gtk.TreeIter iter = folderViewer.Store.GetIter(path);
			if (iter.Equals(Gtk.TreeIter.Zero)) return;

			// Build & Set Thumb Image
			Gtk.Application.Invoke(delegate {
				folderViewer.Store.SetPixbuf(iter, pixbuf);
				folderViewer.ShowAll();
			});
		}

		// ============================================
		// PUBLIC Properties
		// ============================================
	}
}
