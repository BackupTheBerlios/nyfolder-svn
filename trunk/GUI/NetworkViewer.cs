/* [ GUI/NetworkViewer.cs ] NyFolder (Network Viewer)
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
using System.Text;
using System.Text.RegularExpressions;

using Niry;
using Niry.Utils;
using Niry.GUI.Gtk2;

using NyFolder;
using NyFolder.Utils;
using NyFolder.Protocol;

namespace NyFolder.GUI {
	public delegate void PeerSelectedHandler (object sender, UserInfo userInfo);
	public delegate void SendFileHandler (object sender, UserInfo userInfo, string path);

	public class NetworkViewer : Gtk.ScrolledWindow {
		// ============================================
		// PUBLIC Events
		// ============================================
		public event PeerSelectedHandler ItemActivated = null;
		public event PeerSelectedHandler ItemRemoved = null;
		public event RightMenuHandler RightMenu = null;
		public event SendFileHandler SendFile = null;
		
		// ============================================
		// PROTECTED Members
		// ============================================
		protected Gtk.IconView iconView;
		protected NetworkStore store;

		// ============================================
		// PRIVATE Members
		// ============================================
		private static TargetEntry[] dndTargetTable = new TargetEntry[] {
			new TargetEntry("TEXT", 0, 0),
			new TargetEntry("STRING", 0, 1),
			new TargetEntry("text/plain", 0, 2),
			new TargetEntry("text/uri-list", 0, 3),
			new TargetEntry("_NETSCAPE_URL", 0, 4),
			new TargetEntry("application/x-color", 0, 5),
			new TargetEntry("application/x-rootwindow-drop", 0, 6),
			new TargetEntry("property/bgimage", 0, 7),
			new TargetEntry("property/keyword", 0, 8),
			new TargetEntry("x-special/gnome-icon-list", 0, 9),
			new TargetEntry("x-special/gnome-reset-background", 0, 10)
		};

		// ============================================
		// PUBLIC Constructors
		// ============================================
		public NetworkViewer() {
			// Initialize Scrolled Window
			BorderWidth = 0;
			ShadowType = ShadowType.EtchedIn;
			SetPolicy(PolicyType.Automatic, PolicyType.Automatic);

			// Initialize Network Store
			this.store = new NetworkStore();

			// Initialize Icon View
			iconView = new IconView(store);
			iconView.TextColumn = NetworkStore.COL_NAME;
			iconView.PixbufColumn = NetworkStore.COL_PIXBUF;
			iconView.SelectionMode = SelectionMode.Single;

			// Initialize Icon View Events
			iconView.ItemActivated += new ItemActivatedHandler(OnItemActivated);
			iconView.ButtonPressEvent += new ButtonPressEventHandler(OnItemClicked);

			// Initialize Icon View Drag & Drop
			iconView.EnableModelDragDest(dndTargetTable, Gdk.DragAction.Copy);
			iconView.DragDataReceived += new DragDataReceivedHandler(OnDragDataReceived);

			// Add IconView to ScrolledWindow
			Add(iconView);
		}

		// ============================================
		// PUBLIC Methods
		// ============================================
		public void Refresh() {
			Console.WriteLine("ToDo: Refresh Network Viewer");
			#warning ToDo: Refresh Network Viewer
		}

		public void Add (UserInfo userInfo) {
			store.Add(userInfo);
		}

		public void Remove (UserInfo userInfo) {
			store.Remove(userInfo.Name);
		}

		public void RemoveAll() {
			store.Clear();
		}

		public UserInfo GetUserInfo (string username) {
			foreach (object[] row in this.store) {
				if (username.Equals(row[NetworkStore.COL_NAME]) == true)
					return((UserInfo) row[NetworkStore.COL_USER_INFO]);
			}
			return(null);
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
			UserInfo userInfo = store.GetUserInfo(path);

			// Get Drop Uri
			string draggedUris = Encoding.UTF8.GetString(args.SelectionData.Data);
			string[] filesUri = Regex.Split(draggedUris, "\r\n");

			foreach (string uri in filesUri) {
				if (uri == null || uri.Equals("") || uri.Length == 0)
					continue;

				string filePath = uri;

				// Start Event Send File:
				if (filePath.StartsWith("file://") == true) {
					if (Environment.OSVersion.Platform != PlatformID.Unix) {
						// Windows: file:///D:/Prova
						filePath = filePath.Substring(8);
					} else {
						// Unix: file:///home/
						filePath = filePath.Substring(7);
					}
				}

				Debug.Log("Send To '{0}' URI: '{1}'", userInfo.Name, filePath);
				Gtk.Application.Invoke(delegate {
					if (SendFile != null) SendFile(this, userInfo, filePath);
				});
			}

			Drag.Finish(args.Context, true, false, args.Time);
		}

		protected void OnItemActivated (object sender, ItemActivatedArgs args) {
			UserInfo userInfo = store.GetUserInfo(args.Path);
			if (ItemActivated != null) ItemActivated(this, userInfo);
		}

		protected void OnNetRemove (object sender, EventArgs args) {
			foreach (TreePath treePath in iconView.SelectedItems) {
				UserInfo userInfo = store.GetUserInfo(treePath);
				if (ItemRemoved != null) ItemRemoved(this, userInfo);
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
				menu.AddImageItem(Gtk.Stock.Remove, new EventHandler(OnNetRemove));

				// Start Right Menu Event
				if (RightMenu != null) RightMenu(this, menu);

				menu.Popup();
				menu.ShowAll();
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

		public NetworkStore Store {
			get { return(this.store); }
		}
	}
}
