/* [ GUI/NotebookViewer.cs ] NyFolder (Notebook Viewer)
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
using System.Collections;

using NyFolder;
using NyFolder.Protocol;

namespace NyFolder.GUI {
	public class NotebookViewer : Gtk.Notebook {
		// ============================================
		// PUBLIC Events
		// ============================================
		public event DirChangedHandler DirChanged = null;

		// ============================================
		// PROTECTED Members
		// ============================================
		protected NetworkViewer networkViewer;

		// ============================================
		// PRIVATE Members
		// ============================================
		private ArrayList pagesCustom;	// Gtk.Widget
		private Hashtable tabsCustom;	// [TabLabel.Button] = Gtk.Widget

		private Hashtable pages;		// [UserInfo] = FolderViewer
		private Hashtable tabs;			// [TabLabel.Button] = UserInfo

		// ============================================
		// PUBLIC Constructors
		// ============================================
		public NotebookViewer() {
			// Initialize Notebook
			this.ShowTabs = true;
			this.Scrollable = true;

			// Initialize Notebook Members
			this.pages = new Hashtable();
			this.tabs = new Hashtable();
			this.tabsCustom = new Hashtable();
			this.pagesCustom = new ArrayList();

			// Initialize Network Viewer
			// =========================================================
			networkViewer = new NetworkViewer();
			networkViewer.ItemActivated += new PeerSelectedHandler(OnNetItemActivated);

			// Initialize Network Viewer Tab Label
			TabLabel tabLabel = new TabLabel("<b>Network</b>");
			tabLabel.Button.Sensitive = false;

			// Add Network Viewer (Default Fixed Page)
			AppendPage(this.networkViewer, tabLabel);
		}

		// ============================================
		// PUBLIC Methods
		// ============================================
		public int Append (UserInfo userInfo) {
			return(Append(userInfo, userInfo.Name));
		}

		public int Append (UserInfo userInfo, string label) {
			FolderViewer folderViewer = LookupPage(userInfo);
			int npage;

			// Page Already Exists
			if (folderViewer != null) {
				npage = PageNum(folderViewer);
				// Set New Page as Current Page
				CurrentPage = npage;
			} else {
				// Initialize Tab Label
				TabLabel tabLabel = new TabLabel(label);
				tabLabel.Button.Clicked += new EventHandler(OnCloseTab);

				// Initialize Folder Viewer
				folderViewer = new FolderViewer(userInfo);
				folderViewer.DirChanged += new DirChangedHandler(OnDirChangedHandler);

				// Add TabLabel -> UserInfo
				this.tabs.Add(tabLabel.Button, userInfo);

				// Add UserInfo -> FolderViewer
				this.pages.Add(userInfo, folderViewer);

				// Append Page To Notebook
				npage = AppendPage(folderViewer, tabLabel);
			}

			// Show All (Refresh Notebook Viewer)
			this.ShowAll();

			return(npage);
		}

		public void Remove (UserInfo userInfo) {
			FolderViewer folderViewer = LookupPage(userInfo);
			if (folderViewer != null) {
				// Remove Folder Viewer Event
				folderViewer.DirChanged -= new DirChangedHandler(OnDirChangedHandler);

				// Remove TabLabel -> UserInfo
				TabLabel tabLabel = (TabLabel) GetTabLabel(folderViewer);
				tabLabel.Button.Clicked -= new EventHandler(OnCloseTab);
				this.tabs.Remove(tabLabel.Button);

				// Remove UserInfo -> FolderViewer
				this.pages.Remove(userInfo);

				// Remove Folder Viewer
				int npage = PageNum(folderViewer);
				RemovePage(npage);

				// Show All (Refresh Notebook Viewer)
				this.ShowAll();
			}
		}

		public void RemoveAll() {
			foreach (UserInfo userInfo in this.pages)
				Remove(userInfo);
			this.pages.Clear();
			this.tabs.Clear();
		}

		public FolderViewer LookupPage (UserInfo userInfo) {
			return((FolderViewer) this.pages[userInfo]);
		}

		public int AppendCustom (Gtk.Widget page, string label, Gtk.Image icon) {
			int npage;

			if (this.pagesCustom.IndexOf(page) < 0) {
				// Initialize Tab Label
				TabLabel tabLabel = new TabLabel(label, icon);
				tabLabel.Button.Clicked += new EventHandler(OnCustomCloseTab);

				// Add TabLabel -> Page
				this.tabsCustom.Add(tabLabel.Button, page);

				// Add Page
				this.pagesCustom.Add(page);

				// Append Page To Notebook
				npage = AppendPage(page, tabLabel);
			} else {
				npage = PageNum(page);
				// Set New Page as Current Page
				CurrentPage = npage;
			}

			// Show All (Refresh Notebook Viewer)
			this.ShowAll();

			return(npage);
		}

		public void RemoveCustom (Gtk.Widget page) {
			if (page != null) {
				// Remove TabLabel -> Page
				TabLabel tabLabel = (TabLabel) GetTabLabel(page);
				tabLabel.Button.Clicked -= new EventHandler(OnCustomCloseTab);
				this.tabsCustom.Remove(tabLabel.Button);

				// Remove UserInfo -> FolderViewer
				this.pagesCustom.Remove(page);

				// Remove Folder Viewer
				int npage = PageNum(page);
				RemovePage(npage);

				// Show All (Refresh Notebook Viewer)
				this.ShowAll();
			}
		}

		public void SelectNetworkViewer() {
			// Set Network Viewer as Current Page
			CurrentPage = 0;

			// Show All (Refresh Notebook Viewer)
			this.ShowAll();
		}

		// ============================================
		// PROTECTED (Methods) Event Handlers
		// ============================================
		protected void OnCloseTab (object sender, EventArgs args) {
			Gtk.Application.Invoke(delegate {
				Remove((UserInfo) this.tabs[sender]);
			});
		}

		protected void OnCustomCloseTab (object sender, EventArgs args) {
			Gtk.Application.Invoke(delegate {
				RemoveCustom((Gtk.Widget) this.tabsCustom[sender]);
			});
		}

		protected void OnNetItemActivated (object sender, UserInfo userInfo) {
			Gtk.Application.Invoke(delegate {
				// Append Page
				Append(userInfo);
			});
		}

		// ============================================
		// PRIVATE Methods
		// ============================================
		private void OnDirChangedHandler (object sender, bool parent) {
			if (DirChanged != null) DirChanged(sender, parent);
		}

		// ============================================
		// PUBLIC Properties
		// ============================================
		public NetworkViewer NetworkViewer {
			get { return(this.networkViewer); }
		}
	}
}
