/* [ GUI/Dialogs/DownloadManager.cs ] NyFolder (Download Manager)
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
using GLib;
using Glade;
using System;

using Niry;
using Niry.Network;

using NyFolder;
using NyFolder.Utils;
using NyFolder.Protocol;

namespace NyFolder.GUI.Dialogs {
	internal class DownloadStore : ListStore {
		public DownloadStore() : 
			base(typeof(string), typeof(string), typeof(int), typeof(string))
		{
		}

		public void Add (FileSender fs) {
			UserInfo userInfo = fs.Peer.Info as UserInfo;
			string fileSize = Utils.FileProperties.GetSizeString(fs.FileSize);
			AppendValues(userInfo.Name, fs.FileName, fs.SendedPercent, fileSize);
		}

		public void Add (FileReceiver fr) {
			UserInfo userInfo = fr.Peer.Info as UserInfo;
			string fileSize = Utils.FileProperties.GetSizeString(fr.FileSize);
			AppendValues(userInfo.Name, fr.FileName, fr.ReceivedPercent, fileSize);
		}
	}

	internal class DownloadViewer : TreeView {
		public DownloadViewer (DownloadStore store) : base(store) {
			TreeViewColumn col;

			// UserInfo
			col = AppendColumn("User", new CellRendererText(), "text", 0);
			col.Resizable = true;
			col.Spacing = 2;

			// FileName
			col = AppendColumn("FileName", new CellRendererText(), "text", 1);
			col.Resizable = true;
			col.Spacing = 2;

			// Progress
			col = AppendColumn("Progress", new CellRendererProgress(), "value", 2);
			col.Resizable = true;
			col.Expand = true;
			col.Spacing = 2;

			// FileSize
			col = AppendColumn("Size", new CellRendererText(), "text", 3);
			col.Resizable = true;
			col.Spacing = 2;
		}
	}

	public class DownloadManager {
		[Glade.WidgetAttribute] private Gtk.ScrolledWindow swRecv;
		[Glade.WidgetAttribute] private Gtk.ScrolledWindow swSnd;
		[Glade.WidgetAttribute] private Gtk.Window window;
		[Glade.WidgetAttribute] private Gtk.Image image;
		private Protocol.DownloadManager dwManager;
		private DownloadViewer recvViewer;
		private DownloadViewer sndViewer;
		private DownloadStore recvStore;
		private DownloadStore sndStore;
		private bool timeoutRet = true;
		internal uint TimeHandle;

		public DownloadManager() {
			XML xml = new XML(null, "DownloadManagerWindow.glade", "window", null);
			xml.Autoconnect(this);

			// Initialize Window
			this.window.DeleteEvent += new DeleteEventHandler(OnWindowDelete);

			// Initialize Image
			image.Pixbuf = StockIcons.GetPixbuf("Download", 24);

			// Initialize Recv Viewer
			recvStore = new DownloadStore();			
			recvViewer = new DownloadViewer(recvStore);
			swRecv.Add(recvViewer);

			// Initialize Send Viewer
			sndStore = new DownloadStore();
			sndViewer = new DownloadViewer(sndStore);
			swSnd.Add(sndViewer);

			// Get Instance Of Protocol Download Manager
			dwManager = Protocol.DownloadManager.GetInstance();

			// Update Downloads Manually
			OnUpdateRecvStore(null, null);
			OnUpdateSndStore(null, null);

			// Update Downloads With P2P Event
			P2PManager.PeerReceived += new PeerEventHandler(OnUpdateRecvStore);
			P2PManager.PeerSended += new PeerEventHandler(OnUpdateSndStore);

			// Update Downloads With Timeout
			TimeHandle = GLib.Timeout.Add(1000, new TimeoutHandler(UpdateStores));

			this.window.ShowAll();
		}

		// ============================================
		// PRIVATE (Methods) Event Handler
		// ============================================
		private void OnWindowDelete (object sender, DeleteEventArgs args) {
			P2PManager.PeerReceived -= new PeerEventHandler(OnUpdateRecvStore);
			P2PManager.PeerSended -= new PeerEventHandler(OnUpdateSndStore);
			this.timeoutRet = false;
		}

		private void OnUpdateRecvStore (object sender, PeerEventArgs args) {
			Gtk.Application.Invoke(delegate {
				lock (recvStore) {
					recvStore.Clear();
					foreach (FileReceiver fileReceiver in dwManager.Receiving)
						recvStore.Add(fileReceiver);
				}
			});
		}

		private void OnUpdateSndStore (object sender, PeerEventArgs args) {
			Gtk.Application.Invoke(delegate {
				lock (sndStore) {
					sndStore.Clear();
					foreach (FileSender fileSender in dwManager.Sending)
						sndStore.Add(fileSender);
				}
			});
		}

		private bool UpdateStores() {
			if (timeoutRet == true) OnUpdateRecvStore(null, null);
			if (timeoutRet == true) OnUpdateSndStore(null, null);
			return(this.timeoutRet);
		}
	}
}
