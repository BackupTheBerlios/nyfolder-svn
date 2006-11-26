/* [ GUI/Glue/FolderManager.cs ] NyFolder Folder Manager Glue
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

using Niry;
using Niry.Utils;

using NyFolder;
using NyFolder.GUI;
using NyFolder.Utils;
using NyFolder.Protocol;

namespace NyFolder.GUI.Glue {
	public sealed class FolderManager {
		// ============================================
		// PRIVATE Members
		// ============================================
		private NotebookViewer notebookViewer;
		private MenuManager menuManager;
		private UserPanel userPanel;

		// ============================================
		// PUBLIC Constructors
		// ============================================
		public FolderManager (GUI.Window window) {
			// Initialize Components
			this.menuManager = window.Menu;
			this.userPanel = window.UserPanel;
			this.notebookViewer = window.NotebookViewer;

			// Network Viewer Displayed at Start...
			SetSensitiveGoUpMenu(false);
			SetSensitiveGoHomeMenu(false);

			// Add Event Handlers
			this.menuManager.Activated += new EventHandler(OnMenuActivated);
			this.notebookViewer.SwitchPage += new SwitchPageHandler(OnSwitchPage); 
			this.notebookViewer.DirChanged += new BoolEventHandler(OnDirChanged);
			this.userPanel.FolderButton.Clicked += new EventHandler(OnMyFolderCliecked);
		}

		~FolderManager() {
			this.menuManager.Activated -= new EventHandler(OnMenuActivated);
			this.notebookViewer.SwitchPage -= new SwitchPageHandler(OnSwitchPage); 
			this.notebookViewer.DirChanged -= new BoolEventHandler(OnDirChanged);
			this.userPanel.FolderButton.Clicked -= new EventHandler(OnMyFolderCliecked);
		}

		// ============================================
		// PRIVATE (Methods) Event Handlers
		// ============================================
		private void OnMenuActivated (object sender, EventArgs args) {
			Gtk.Application.Invoke(delegate {
				Action action = sender as Action;
				switch (action.Name) {
					// View
					case "Refresh":
						FolderViewerRefresh();
						break;
					// Go
					case "GoUp":
						FolderViewerGoUp();
						break;
					case "GoHome":
						FolderViewerGoHome();
						break;
					case "GoNetwork":
						this.notebookViewer.SelectNetworkViewer();
						break;
					case "GoMyFolder":
						OnMyFolderCliecked(null, null);
						break;
				}
			});
		}

		/// When User Switch Notebook Viewer Page, 
		/// This Method Catch event and if page is Folder Viewer object
		/// Do Folder Refresh & set menu/toolbar actions.
		/// Else if isn't Folder Viewer object
		/// setup toolbar & menu action (On/Off GoUp, GoHome, ecc)
		private void OnSwitchPage (object o, SwitchPageArgs args) {
			Gtk.Application.Invoke(delegate {
				Gtk.Widget page = notebookViewer.GetNthPage((int) args.PageNum);
				Type objType = page.GetType();
				if (objType != typeof(FolderViewer)) {
					// NetworkViewer or Custom
					SetSensitiveGoUpMenu(false);
					SetSensitiveGoHomeMenu(false);
				} else {
					// Folder Viewer					
					FolderViewer folderViewer = page as FolderViewer;
					folderViewer.Refresh();

					bool canGoUp = folderViewer.CanGoUp();
					SetSensitiveGoUpMenu(canGoUp);
					SetSensitiveGoHomeMenu(true);
				}

				// Set Sensitive Refresh Menu
				SetSensitiveRefreshMenu(objType.IsSubclassOf(typeof(RefreshableViewer)));
			});
		}

		/// Add My Folder To the Notebook Viewer
		private void OnMyFolderCliecked (object sender, EventArgs args) {
			Gtk.Application.Invoke(delegate {
				this.notebookViewer.Append(MyInfo.GetInstance(), "<b>Me</b> ("+ MyInfo.Name +")");
			});
		}

		/// Method Called to Set Sensitive Go Up menu when user change
		/// Folder Viewer Current Directory.
		private void OnDirChanged (object sender, bool parent) {
			Gtk.Application.Invoke(delegate {
				SetSensitiveGoUpMenu(parent);
			});
		}

		// ============================================
		// PRIVATE Methods
		// ============================================
		private void SetSensitiveGoUpMenu (bool sensitive) {
			this.menuManager.SetSensitive("/MenuBar/GoMenu/GoUp", sensitive);
			this.menuManager.SetSensitive("/ToolBar/GoUp", sensitive);
		}

		private void SetSensitiveGoHomeMenu (bool sensitive) {
			this.menuManager.SetSensitive("/MenuBar/GoMenu/GoHome", sensitive);
			this.menuManager.SetSensitive("/ToolBar/GoHome", sensitive);
		}

		private void SetSensitiveRefreshMenu (bool sensitive) {
			this.menuManager.SetSensitive("/MenuBar/ViewMenu/Refresh", sensitive);
			this.menuManager.SetSensitive("/ToolBar/Refresh", sensitive);
		}

		// ============================================
		// PRIVATE Methods (Notebook, Folder, Network Viewer)
		// ============================================
		private void FolderViewerGoUp() {
			Gtk.Widget page = notebookViewer.CurrentPageWidget;
			if (page.GetType() == typeof(FolderViewer)) {
				// Go Up if is Folder Viewer
				FolderViewer folderViewer = page as FolderViewer;
				folderViewer.GoUp();
			}
		}

		private void FolderViewerGoHome() {
			Gtk.Widget page = notebookViewer.CurrentPageWidget;
			if (page.GetType() == typeof(FolderViewer)) {
				// Go Home if is Folder Viewer
				FolderViewer folderViewer = page as FolderViewer;
				folderViewer.GoHome();
			}
		}

		private void FolderViewerRefresh() {
			Gtk.Widget page = notebookViewer.CurrentPageWidget;
			Type objType = page.GetType();
			if (objType.IsSubclassOf(typeof(RefreshableViewer)) == true) {
				RefreshableViewer obj = page as RefreshableViewer;
				obj.Refresh();
			}
		}
	}
}
