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

using Niry;
using Niry.Utils;
using Niry.Network;
using Niry.GUI.Gtk2;

using NyFolder;
using NyFolder.Utils;
using NyFolder.Protocol;
using NyFolder.GUI.Base;

namespace NyFolder.GUI {
	public delegate void FileSendEventHandler (object sender, string path, bool isDir);

	/// User Shared Folder Viewer
	public class FolderViewer : Gtk.ScrolledWindow {
		// ============================================
		// PUBLIC Events
		// ============================================
		/// Event Raised When user add a Directory
		public event ObjectEventHandler DirectoryAdded = null;
		/// Event Raised When user add a File
		public event ObjectEventHandler FileAdded = null;
		/// Event Raised When folder Refresh is requested
		public event StringEventHandler FolderRefresh = null;
		/// Event Raised When i say "Transfer File"...
		public event FileSendEventHandler FileSend = null;
		/// Event Raised When user Change Directory
		public event BoolEventHandler DirChanged = null;
		/// Event Raised When Mouse Right Key is Pressed
		public event RightMenuHandler RightMenu = null;
		/// Event Raised When Menu "Save File" is Pressed
		public event StringEventHandler SaveFile = null;

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

		// ============================================
		// PUBLIC Constructors
		// ============================================
		/// Create New Folder Viewer
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
			this.store.DirectoryAdded += new ObjectEventHandler(OnStoreDirAdded);
			this.store.FileAdded += new ObjectEventHandler(OnStoreFileAdded);

			// Initialize Icon View
			iconView = new IconView(store);
			iconView.TextColumn = FolderStore.COL_NAME;
			iconView.PixbufColumn = FolderStore.COL_PIXBUF;
			iconView.SelectionMode = SelectionMode.Multiple;

			// Initialize Icon View Events
			iconView.ItemActivated += new ItemActivatedHandler(OnItemActivated);
			iconView.ButtonPressEvent += new ButtonPressEventHandler(OnItemClicked);

			// Initialize Icon View Drag & Drop
			iconView.EnableModelDragDest(Dnd.TargetTable, Gdk.DragAction.Copy);
			iconView.DragDataReceived += new DragDataReceivedHandler(OnDragDataReceived);

			// Add IconView to ScrolledWindow
			Add(iconView);

			// Refresh Icon View
			Refresh();
		}

		~FolderViewer() {
			// Folder Store
			this.store.DirectoryAdded -= new ObjectEventHandler(OnStoreDirAdded);
			this.store.FileAdded -= new ObjectEventHandler(OnStoreFileAdded);

			// Icon View Events
			iconView.ItemActivated -= new ItemActivatedHandler(OnItemActivated);
			iconView.ButtonPressEvent -= new ButtonPressEventHandler(OnItemClicked);

			// Icon View Drag & Drop
			iconView.DragDataReceived -= new DragDataReceivedHandler(OnDragDataReceived);
		}

		// ============================================
		// PUBLIC Methods
		// ============================================
		/// Go Up, To Parent Directory
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

		/// Go To Root (Home) Directory
		public void GoHome() {
			// Directory's Path
			if (currentDirectory == null || baseDirectory == null)
				return;

			// Set New Current Directory & Refresh Folder Viewer
			currentDirectory = new DirectoryInfo(this.baseDirectory);
			Refresh();

			if (DirChanged != null) DirChanged(this, false);
		}

		/// Refresh Folder Viewer and Folder Store
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

		/// Fill Folder Viewer and Folder Store
		public void Fill (string path, string fileList) {
			currentDirectory = new DirectoryInfo(path);

			string[] flist = fileList.Split('\n');
			foreach (string fileInfo in flist) {
				if (fileInfo == "" || fileInfo == null || fileInfo.Length == 0)
					continue;

				string[] file = fileInfo.Split('|');
				if (file[1].Equals("0")) {
					store.AddFile(file[0]);
				} else {
					store.AddDirectory(file[0]);
				}
			}

			this.ShowAll();
		}

		/// Return true if Current Directory isn't Root (Home) Directory
		public bool CanGoUp() {
			return(!currentDirectory.FullName.Equals(baseDirectory));
		}

		// ============================================
		// PROTECTED (Methods) Event Handlers
		// ============================================
		protected void OnDragDataReceived (object sender, DragDataReceivedArgs args) {
			// Get Drop Paths
			object[] filesPath = Dnd.GetDragReceivedPaths(args);

			if (this.userInfo == MyInfo.GetInstance()) {
				// Copy Selected Files Into Directory
				foreach (string filePath in filesPath) {
					FileUtils.CopyAll(filePath, currentDirectory.FullName);
				}

				// Refresh Icon View
				Refresh();
			} else {
				// Send Files
				foreach (string filePath in filesPath) {
					PeerSocket peer = (PeerSocket) P2PManager.KnownPeers[userInfo];
					bool fisDir = FileUtils.IsDirectory(filePath);
	
					Debug.Log("Send To '{0}' URI: '{1}'", userInfo.Name, filePath);
					if (FileSend != null) FileSend(peer, filePath, fisDir);
				}
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
				PopupMenu menu = new PopupMenu();

				// Options
				if (this.userInfo == MyInfo.GetInstance()) {
					AddSendFileMenu(menu);
					menu.AddImageItem(Gtk.Stock.Remove, new EventHandler(OnFileRemove));
				} else {
					menu.AddImageItem(Gtk.Stock.Save, new EventHandler(OnFileSave));
				}

				// Start Right Menu Event
				if (RightMenu != null) RightMenu(this, menu);

				menu.Popup();
				menu.ShowAll();
			}
		}

		protected void OnStoreFileAdded (object sender, object arg) {
			if (FileAdded != null) FileAdded(this, arg);
		}

		protected void OnStoreDirAdded (object sender, object arg) {
			if (DirectoryAdded != null) DirectoryAdded(this, arg);
		}
		
		// ============================================
		// PROTECTED (Methods) Menu Event Handlers
		// ============================================
		protected void OnFileSave (object sender, EventArgs args) {
			foreach (TreePath treePath in iconView.SelectedItems) {
				string fpath = store.GetFilePath(treePath);
				if (SaveFile != null) SaveFile(this, fpath);
			}
		}

		protected void OnFileRemove (object sender, EventArgs args) {
			foreach (TreePath treePath in iconView.SelectedItems)
				FileUtils.RemoveAll(store.GetFilePath(treePath));
			Gtk.Application.Invoke(delegate { Refresh(); });
		}

		protected void OnFileSend (object sender, EventArgs args) {
			ExtMenuItem item = sender as ExtMenuItem;
			PeerSocket peer = (PeerSocket) P2PManager.KnownPeers[item.ExtraData];
			
			// Send File Event if Peer != Null
			if (peer != null) {
				foreach (TreePath treePath in iconView.SelectedItems) {
					string fpath = this.store.GetFilePath(treePath);
					bool fisDir = this.store.GetIsDirectory(treePath);
					if (FileSend != null) FileSend(peer, fpath, fisDir);
				}
			}
		}

		// ============================================
		// PRIVATE Methods
		// ============================================
		private void AddSendFileMenu (PopupMenu menu) {
			if (P2PManager.KnownPeers == null) return;
			if (P2PManager.KnownPeers.Count <= 0) return;

			PopupMenu subMenu = new PopupMenu();
			menu.AddImageItem("Send", subMenu);

			foreach (UserInfo userInfo in P2PManager.KnownPeers.Keys) {
				ExtMenuItem item = new ExtMenuItem(userInfo.Name, userInfo);
				subMenu.AddItem(item, new EventHandler(OnFileSend));
			}
		}

		// ============================================
		// PUBLIC Properties
		// ============================================
		/// Return The Selected Items
		public TreePath[] SelectedItems {
			get { return(this.iconView.SelectedItems); }
		}

		/// Return The Folder's File List
		public FolderStore Store {
			get { return(this.store); }
		}

		/// Return The Folder's User Informations
		public UserInfo UserInfo {
			get { return(this.userInfo); }
		}
	}
}
