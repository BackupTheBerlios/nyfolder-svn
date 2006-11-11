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

using Niry;
using Niry.Utils;
using Niry.Network;
using Niry.GUI.Gtk2;

using NyFolder;
using NyFolder.Utils;
using NyFolder.Protocol;
using NyFolder.GUI.Base;

namespace NyFolder.GUI {
	public delegate void SendFileHandler (object sender, UserInfo userInfo, string path);

	/// Network Viewer
	public class NetworkViewer : Gtk.ScrolledWindow {
		// ============================================
		// PUBLIC Events
		// ============================================
		public event PeerSelectedHandler UserLoggedOut = null;
		public event PeerSelectedHandler UserLoggedIn = null;
		public event PeerSelectedHandler ItemActivated = null;
		public event PeerSelectedHandler ItemRemoved = null;
		public event BlankEventHandler RefreshPeers = null;
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
			iconView.EnableModelDragDest(Dnd.TargetTable, Gdk.DragAction.Copy);
			iconView.DragDataReceived += new DragDataReceivedHandler(OnDragDataReceived);

			// Add IconView to ScrolledWindow
			Add(iconView);
		}

		// ============================================
		// PUBLIC Methods
		// ============================================
		/// Refresh Network Viewer and Network Store
		public void Refresh() {
			store.Clear();

			if (P2PManager.KnownPeers != null &&
				P2PManager.KnownPeers.Count > 0)
			{
				foreach (UserInfo userInfo in P2PManager.KnownPeers.Keys)
					store.Add(userInfo);
			}

			if (RefreshPeers != null) RefreshPeers(this);
		}

		/// Add New Peer
		public void Add (UserInfo userInfo) {
			store.Add(userInfo);

			// Raise User Logged In Event
			if (userInfo.IsOnline == true && UserLoggedIn != null)
				UserLoggedIn(this, userInfo);
		}

		/// Remove Peer
		public void Remove (UserInfo userInfo) {
			// Raise User Logged Out Event
			if (UserLoggedOut != null) 
				UserLoggedOut(this, userInfo);

			store.Remove(userInfo);
		}

		/// Remove All Peers into Network Store
		public void RemoveAll() {
			store.Clear();
		}

		/// Return UserInfo of specified Username
		public UserInfo GetUserInfo (string username) {
			foreach (object[] row in this.store) {
				UserInfo userInfo = (UserInfo) row[NetworkStore.COL_USER_INFO];
				if (userInfo.Name == username) return(userInfo);
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

			// Select Item (Change Icon To Activate it)
			UserInfo userInfo = store.GetUserInfo(path);
			if (userInfo.IsOnline == false) {
				Drag.Finish(args.Context, false, false, args.Time);
				return;
			}

			// Get Drop Paths
			object[] filesPath = Dnd.GetDragReceivedPaths(args);
			foreach (string filePath in filesPath) {
				Debug.Log("Send To '{0}' URI: '{1}'", userInfo.Name, filePath);
				if (SendFile != null) SendFile(this, userInfo, filePath);
			}

			Drag.Finish(args.Context, true, false, args.Time);
		}

		protected void OnItemActivated (object sender, ItemActivatedArgs args) {
			UserInfo userInfo = store.GetUserInfo(args.Path);
			if (userInfo == null) return;
			if (ItemActivated != null) ItemActivated(this, userInfo);
		}

		protected void OnNetRemove (object sender, EventArgs args) {
			foreach (TreePath treePath in iconView.SelectedItems) {
				UserInfo userInfo = store.GetUserInfo(treePath);
				if (userInfo == null) continue;
				if (userInfo.IsOnline == false) continue;
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
		/// Get Selected Items TreePath
		public TreePath[] SelectedItems {
			get { return(this.iconView.SelectedItems); }
		}

		/// Get Network Store
		public NetworkStore Store {
			get { return(this.store); }
		}
	}
}
