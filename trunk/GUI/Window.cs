/* [ GUI/Window.cs ] NyFolder (Main Window)
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
	/// NyFolder Main Window
	public class Window : Gtk.Window {
		// ============================================
		// PUBLIC Events
		// ============================================
		public BlankEventHandler Logout = null;

		// ============================================
		// PROTECTED Members
		// ============================================
		protected NotebookViewer notebookViewer;
		protected Gtk.Statusbar statusBar;
		protected MenuManager menuManager;
		protected UserPanel userPanel;
		
		// ============================================
		// PRIVATE Members
		// ============================================
		private Gtk.VBox vboxRight;
		private Gtk.VBox vboxLeft;
		private Gtk.VBox vboxMain;
		private Gtk.HBox hboxMenu;
		private Gtk.HBox hbox;

		// ============================================
		// PUBLIC Constructors
		// ============================================
		/// Create New NyFolder Window
		public Window() : base(Info.Name + " " + Info.Version) {
			// Initialize Window
			this.SetDefaultSize(620, 320);
			DefaultIcon = StockIcons.GetPixbuf("NyFolderIcon");
			this.DeleteEvent += new DeleteEventHandler(OnWindowDelete);

			// Initialize VBox
			this.vboxMain = new Gtk.VBox(false, 2);
			this.Add(this.vboxMain);

			// Initialize Menu Manager
			this.menuManager = new MenuManager();
			this.menuManager.Activated += new EventHandler(OnMenuActivated);
			this.AddAccelGroup(this.menuManager.AccelGroup);

			// Initialize HBox Menu
			this.hboxMenu = new Gtk.HBox(false, 0);
			this.vboxMain.PackStart(hboxMenu, false, false, 0);

			// Initialize MenuBar
			Gtk.MenuBar menuBar = this.MenuBar;
			this.hboxMenu.PackStart(menuBar, false, false, 0);

			// Initialize HBox
			this.hbox = new Gtk.HBox();
			this.vboxMain.PackStart(this.hbox, true, true, 2);

			// Initialize Left VBox
			this.vboxLeft = new Gtk.VBox(false, 2);
			this.hbox.PackStart(this.vboxLeft, false, false, 2);

			// Initialize User Panel
			this.userPanel = new UserPanel();
			this.vboxLeft.PackStart(this.userPanel, false, false, 2);

			// Initialize Right VBox
			this.vboxRight = new Gtk.VBox(false, 2);
			this.hbox.PackStart(this.vboxRight, true, true, 2);

			// Initialize ToolBar
			Gtk.Toolbar toolBar = this.ToolBar;
			toolBar.ShowArrow = true;
			toolBar.IconSize = Gtk.IconSize.LargeToolbar;
			toolBar.ToolbarStyle = ToolbarStyle.Both;
			this.vboxRight.PackStart(toolBar, false, false, 2);

			// Initialize Notebook Viewer
			this.notebookViewer = new NotebookViewer();
			this.vboxRight.PackStart(this.notebookViewer, true, true, 2);

			// Initialize Status Bar
			this.statusBar = new Gtk.Statusbar();
			this.vboxMain.PackEnd(this.statusBar, false, false, 0);

			// Window Show All
			this.ShowAll();
		}

		// ============================================
		// PRIVATE Methods
		// ============================================


		// ============================================
		// PRIVATE (Methods) Event Handler
		// ============================================
		private void OnWindowDelete (object sender, DeleteEventArgs args) {
			Application.Quit();
			args.RetVal = true;
		}

		private void OnMenuActivated (object sender, EventArgs args) {
			Gtk.Application.Invoke(delegate {
				Action action = sender as Action;
				switch (action.Name) {
					// File Menu
					case "ProxySettings":
						Glue.Dialogs.ProxySettings();
						break;
					case "Logout":
						if (Glue.Dialogs.QuestionDialog("Logout", 
							"Do You Really Want To Logout ?")) 
						{
							if (Logout != null) Logout(this);
						}
						break;
					case "Quit":
						Gtk.Application.Quit();
						break;
					// View Menu
					case "ViewToolBar":
						this.ToolBar.Visible = !this.ToolBar.Visible;
						break;
					case "ViewUserPanel":
						this.UserPanel.Visible = !this.UserPanel.Visible;
						break;
					// Help Menu
					case "About":
						new Dialogs.About();
						break;
				}
			});
		}

		// ============================================
		// PUBLIC Properties
		// ============================================
		/// Get Window Menu & Toolbar Manager
		public MenuManager Menu {
			get { return(this.menuManager); }
		}

		/// Get Window MenuBar
		public Gtk.MenuBar MenuBar {
			get { return((Gtk.MenuBar) this.menuManager.GetWidget("/MenuBar")); }
		}

		/// Get Window ToolBar
		public Gtk.Toolbar ToolBar {
			get { return((Gtk.Toolbar) menuManager.GetWidget("/ToolBar")); }
		}

		/// Get Window User Panel
		public UserPanel UserPanel {
			get { return(this.userPanel); }
		}

		/// Get Window Notebook Viewer
		public NotebookViewer NotebookViewer {
			get { return(this.notebookViewer); }
		}

		/// Get Window Status Bar
		public Gtk.Statusbar StatusBar {
			get { return(this.statusBar); }
		}
	}
}
