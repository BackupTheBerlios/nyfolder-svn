/* [ GUI/FolderViewer.cs ] NyFolder (Folder Viewer)
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
using System.Text;
using System.Text.RegularExpressions;

using NyFolder;
using NyFolder.Utils;
using NyFolder.Protocol;

namespace NyFolder.GUI {
	public delegate void DirChangedHandler (object sender, bool parent);
	public delegate void FileEventHandler (object sender, string path);

	public class FolderViewer : Gtk.ScrolledWindow {
		// ============================================
		// PUBLIC Events
		// ============================================
		public event FileEventHandler FolderRefresh = null;
		public event DirChangedHandler DirChanged = null;

		// ============================================
		// PROTECTED Members
		// ============================================
		protected Gtk.IconView iconView;
		protected FolderStore store;
		protected UserInfo userInfo;

		protected DirectoryInfo currentDirectory;
		protected string baseDirectory;

		// ============================================
		// PRIVATE Members
		// ============================================
		private static TargetEntry[] dndTargetTable = new TargetEntry[] {
			new TargetEntry("STRING", 0, 0),
			new TargetEntry("text/plain", 0, 1),
			new TargetEntry("text/uri-list", 0, 2),
			new TargetEntry("_NETSCAPE_URL", 0, 3),
			new TargetEntry("application/x-color", 0, 4),
			new TargetEntry("application/x-rootwindow-drop", 0, 5),
			new TargetEntry("property/bgimage", 0, 6),
			new TargetEntry("property/keyword", 0, 7),
			new TargetEntry("x-special/gnome-icon-list", 0, 8),
			new TargetEntry("x-special/gnome-reset-background", 0, 9)
		};

		// ============================================
		// PUBLIC Constructors
		// ============================================
		public FolderViewer (UserInfo userInfo) {
			// Initialize Scrolled Window
			BorderWidth = 0;
			ShadowType = ShadowType.EtchedIn;
			SetPolicy(PolicyType.Automatic, PolicyType.Automatic);

			// Initialize UserInfo
			this.userInfo = userInfo;
			if (this.userInfo == MyInfo.GetInstance()) {
				baseDirectory = Paths.UserSharedDirectory(MyInfo.Name);
			} else {
				baseDirectory = "/";
			}
			currentDirectory = new DirectoryInfo(baseDirectory);

			// Initialize Folder Store
			this.store = new FolderStore();

			// Initialize Icon View
			iconView = new IconView(store);
			iconView.SelectionMode = SelectionMode.Single;
			iconView.TextColumn = FolderStore.COL_NAME;
			iconView.PixbufColumn = FolderStore.COL_PIXBUF;

			// Initialize Icon View Events
			iconView.ItemActivated += new ItemActivatedHandler(OnItemActivated);
			iconView.ButtonPressEvent += new ButtonPressEventHandler(OnItemClicked);

			// Initialize Icon View Drag & Drop
			iconView.EnableModelDragDest(dndTargetTable, Gdk.DragAction.Copy);
			iconView.DragDataReceived += new DragDataReceivedHandler(OnDragDataReceived);

			// Add IconView to ScrolledWindow
			Add(iconView);

			// Refresh Icon View
			Refresh();
		}

		// ============================================
		// PUBLIC Methods
		// ============================================
		public void GoUp() {
			// Directory's Path
			if (currentDirectory == null || baseDirectory == null)
				return;

			// Set New Current Directory & Refresh Folder Viewer
			currentDirectory = currentDirectory.Parent;
			Refresh();

			// if Current Directory = Home Directory Stop Up (false)
			if (DirChanged != null) DirChanged(this, CanGoUp());
		}

		public void GoHome() {
			// Directory's Path
			if (currentDirectory == null || baseDirectory == null)
				return;

			// Set New Current Directory & Refresh Folder Viewer
			currentDirectory = new DirectoryInfo(this.baseDirectory);
			Refresh();

			if (DirChanged != null) DirChanged(this, false);
		}

		public void Refresh() {
			// Directory's Path
			if (currentDirectory == null || baseDirectory == null)
				return;

			// Clear The Store
			store.Clear();

			if (this.userInfo == MyInfo.GetInstance()) {
				// MyFolder
				this.store.Fill(currentDirectory.FullName);
			} else {
				// Peer Folder
				if (FolderRefresh != null) FolderRefresh(this, currentDirectory.FullName);
			}
		}

		public bool CanGoUp() {
			return(!currentDirectory.FullName.Equals(baseDirectory));
		}

		// ============================================
		// PROTECTED (Methods) Event Handlers
		// ============================================
		protected void OnDragDataReceived (object sender, DragDataReceivedArgs args) {
			IconViewDropPosition pos;
			TreePath path;			

			// Get Dest Item Position
			if (iconView.GetDestItemAtPos(args.X, args.Y, out path, out pos) == false) {
				Drag.Finish(args.Context, false, false, args.Time);
				return;
			}

			// Select Item (Change Icon To Activate)
			Console.WriteLine("Item At Pos: {0} {1}", path, pos);

			// Get Drop Uri
			string[] filesUri = Regex.Split(args.SelectionData.Text, "\r\n");
			foreach (string uri in filesUri) {
				if (uri == null || uri.Equals("") || uri.Length == 0)
					continue;

				// Start Event Send File:
				//if (SendFile != null) SendFile(path, uri);
				Console.WriteLine("URI: '{0}'", uri);
			}

			Drag.Finish(args.Context, true, false, args.Time);
		}

		protected void OnItemActivated (object sender, ItemActivatedArgs args) {
			// Get File Path & IsDirectory Flag				
			bool isDir = store.GetIsDirectory(args.Path);
			string path = store.GetFilePath(args.Path);
			
			if (isDir == true) {
				// Replace Parent With Path and ReFill The Model
				currentDirectory = new DirectoryInfo(path);

				// Refresh FileStore
				Gtk.Application.Invoke(delegate { Refresh(); });

				// Sensitize the up button
				if (DirChanged != null) DirChanged(this, true);
			}
		}

		protected void OnItemClicked (object sender, ButtonPressEventArgs args) {
			if (iconView.SelectedItems.Length <= 0)
				return;

			if (args.Event.Device.Source != Gdk.InputSource.Mouse)
				return;

			if (args.Event.Button == 3) {
				Console.WriteLine("Right Click");
			} else {
				Console.WriteLine("{0} Click", args.Event.Button);
			}
		}

		// ============================================
		// PRIVATE Methods
		// ============================================

		// ============================================
		// PUBLIC Properties
		// ============================================
		public TreePath[] SelectedItems {
			get { return(this.iconView.SelectedItems); }
		}

		public FolderStore Store {
			get { return(this.store); }
		}

		public UserInfo UserInfo {
			get { return(this.userInfo); }
		}
	}
}
