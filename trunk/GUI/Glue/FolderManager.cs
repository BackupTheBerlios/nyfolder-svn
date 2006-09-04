/* [ GUI/Glue/FolderManager.cs ] NyFolder (Folder Manager)
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
using NyFolder.Protocol;

namespace NyFolder.GUI.Glue {
	public class FolderManager {
		// ============================================
		// PRIVATE Members
		// ============================================
		private NotebookViewer notebookViewer;
		private MenuManager menuManager;
		private UserPanel userPanel;

		// ============================================
		// PUBLIC Constructors
		// ============================================
		public FolderManager (MenuManager menu, UserPanel userPanel, NotebookViewer nv) {
			this.userPanel = userPanel;
			this.notebookViewer = nv;
			this.menuManager = menu;

			// Network Viewer Displayed at Start...
			SetSensitiveGoUpMenu(false);
			SetSensitiveGoHomeMenu(false);

			// Add Event Handlers
			this.menuManager.Activated += new EventHandler(OnMenuActivated);
			this.notebookViewer.SwitchPage += new SwitchPageHandler(OnSwitchPage); 
			this.notebookViewer.DirChanged += new DirChangedHandler(OnDirChangedHandler);
			this.userPanel.FolderButton.Clicked += new EventHandler(OnMyFolderCliecked);
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

		public void FolderViewerGoUp() {
			// Skif Network Viewer
			if (this.notebookViewer.CurrentPage == 0)
				return;

			// Go Up
			FolderViewer folderViewer = notebookViewer.CurrentPageWidget as FolderViewer;
			folderViewer.GoUp();
		}

		public void FolderViewerGoHome() {
			// Skif Network Viewer
			if (this.notebookViewer.CurrentPage == 0)
				return;

			// Go Up
			FolderViewer folderViewer = notebookViewer.CurrentPageWidget as FolderViewer;
			folderViewer.GoHome();
		}

		public void FolderViewerRefresh() {
			// Skif Network Viewer
			if (this.notebookViewer.CurrentPage == 0) {
				NetworkViewer nv = notebookViewer.CurrentPageWidget as NetworkViewer;
				nv.Refresh();
			} else {
				// Go Up
				FolderViewer fv = notebookViewer.CurrentPageWidget as FolderViewer;
				fv.Refresh();
			}
		}

		// ============================================
		// PRIVATE (Methods) Menu Event Handler
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

		// ============================================
		// PRIVATE (Methods) Event Handler
		// ============================================
		private void OnSwitchPage (object o, SwitchPageArgs args) {
			Gtk.Application.Invoke(delegate {
				if (args.Page.GetType() != typeof(FolderViewer)) {
					// NetworkViewer or Custom
					SetSensitiveGoUpMenu(false);
					SetSensitiveGoHomeMenu(false);
				} else {
					// Folder Viewer
					Gtk.Widget page = notebookViewer.GetNthPage((int) args.PageNum);
					FolderViewer folderViewer = page as FolderViewer;
					folderViewer.Refresh();

					bool canGoUp = folderViewer.CanGoUp();
					SetSensitiveGoUpMenu(canGoUp);
					SetSensitiveGoHomeMenu(true);
				}
			});
		}

		private void OnMyFolderCliecked (object sender, EventArgs args) {
			Gtk.Application.Invoke(delegate {
				this.notebookViewer.Append(MyInfo.GetInstance(), "<b>Me</b> ("+ MyInfo.Name +")");
			});
		}

		private void OnDirChangedHandler (object sender, bool parent) {
			Gtk.Application.Invoke(delegate {
				SetSensitiveGoUpMenu(parent);
			});
		}
	}
}
