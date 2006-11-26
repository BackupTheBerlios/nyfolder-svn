/* [ Plugins/ImagePreview.cs ] NyFolder Image Preview Plugin
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
using Niry.Network;
using Niry.GUI.Gtk2;

using NyFolder;
using NyFolder.GUI;
using NyFolder.Utils;
using NyFolder.Protocol;
using NyFolder.GUI.Base;
using NyFolder.PluginLib;

namespace NyFolder.Plugins.ImagePreview {
	/// Image Preview (Plugin)
	public class ImagePreview : Plugin {
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
		/// Create Image Preview
		public ImagePreview() {
			// Initialize Plugin Info
			this.pluginInfo = new Info();
		}

		// ============================================
		// PUBLIC Methods
		// ============================================
		/// Initialize Tray Icon Plugin
		public override void Initialize (INyFolder iNyFolder) {
			this.nyFolder = iNyFolder;

			// Initialize GUI Events
			this.nyFolder.MainWindowStarted += new BlankEventHandler(OnMainWindowStarted);

			// Initialize Protocol Events
			CmdManager.AddProtocolEvent += new SetProtocolEventHandler(OnAddProtocolCmds);
			CmdManager.DelProtocolEvent += new SetProtocolEventHandler(OnDelProtocolCmds);
		}

		public void RequestImage (UserInfo userInfo, string filePath) {
			// Get Peer From UserInfo
			PeerSocket peer = P2PManager.KnownPeers[userInfo] as PeerSocket;
			if (peer != null) {
				// Generate Xml Request
				XmlRequest xmlRequest = new XmlRequest();
				xmlRequest.FirstTag = "get";
				xmlRequest.BodyText = filePath;
				xmlRequest.Attributes.Add("what", "imgthumb");
				peer.Send(xmlRequest.GenerateXml());
			} else {
				LoadImage(userInfo, filePath);
			}
		}

		public void SendImageThumb (UserInfo userInfo, string filePath) {
			PeerSocket peer = P2PManager.KnownPeers[userInfo] as PeerSocket;
			if (peer != null) SendImageThumb(peer, filePath);
		}

		public void SendImageThumb (PeerSocket peer, string filePath) {
			string imgThumb = GenerateImageThumb(filePath);
			if (imgThumb == null) return;

			// Generate Xml Request
			XmlRequest xmlRequest = new XmlRequest();
			xmlRequest.FirstTag = "snd";
			xmlRequest.BodyText = imgThumb;
			xmlRequest.Attributes.Add("what", "imgthumb");
			xmlRequest.Attributes.Add("path", filePath);
			peer.Send(xmlRequest.GenerateXml());
		}

		public string GenerateImageThumb (string path) {
			try {
				path = GenerateRealImagePath(path);
				Gdk.Pixbuf pixbuf = ImageUtils.GetPixbuf(path, ThumbWidth, ThumbHeight);
				byte[] imgData = pixbuf.SaveToBuffer("png");
				return(Convert.ToBase64String(imgData));
			} catch (Exception e) {
				Debug.Log("Generate Img Thumb: {0}", e.Message);
				return(null);
			}
		}

		// ============================================
		// PRIVATE (Methods) Event Handlers
		// ============================================
		private void OnMainWindowStarted (object sender) {
			// Initialize GUI Components
			InitializeMenu();

			// Initialize Notebook Viewer Events
			notebookViewer = this.nyFolder.MainWindow.NotebookViewer;
			notebookViewer.FileAdded += new ObjectEventHandler(OnFileAdded);
		}

		private void OnAddProtocolCmds (P2PManager p2pManager) {
			CmdManager.GetEvent += new ProtocolHandler(OnGetEvent);
			CmdManager.SndEvent += new ProtocolHandler(OnSndEvent);
		}

		private void OnDelProtocolCmds (P2PManager p2pManager) {
			CmdManager.GetEvent -= new ProtocolHandler(OnGetEvent);
			CmdManager.SndEvent -= new ProtocolHandler(OnSndEvent);
		}

		protected void OnFileAdded (object sender, object storeIter) {
			FolderViewer folderViewer = sender as FolderViewer;
			Gtk.TreeIter iter = (Gtk.TreeIter) storeIter;
			FolderStore store = folderViewer.Store;

			// Request Image Thumb if it's Image
			string filePath = store.GetFilePath(iter);
			if (ImageUtils.IsImage(Path.GetExtension(filePath)) == true) {
				RequestImage(folderViewer.UserInfo, filePath);
			}
		}

		public void OnGetEvent (PeerSocket peer, XmlRequest xml) {
			if (xml.Attributes["what"].Equals("imgthumb")) {
				SendImageThumb(peer, xml.BodyText);
			}
		}

		public void OnSndEvent (PeerSocket peer, XmlRequest xml) {
			if (xml.Attributes["what"].Equals("imgthumb")) {
				string path = (string) xml.Attributes["path"];
				ReceiveImageThumb((UserInfo) peer.Info, path, xml.BodyText);
			}
		}

		protected void OnImagePreview (object sender, EventArgs args) {
			Gtk.ToggleAction action = sender as Gtk.ToggleAction;

			if (action.Active == true) {
				this.notebookViewer.FileAdded += new ObjectEventHandler(OnFileAdded);
			} else {
				this.notebookViewer.FileAdded -= new ObjectEventHandler(OnFileAdded);
			}
		}

		// ============================================
		// PRIVATE Methods
		// ============================================
		private void InitializeMenu() {
			ToggleActionEntry[] entries = new ToggleActionEntry[] {
				new ToggleActionEntry("ImagePreview", null, "Image Preview", null,
									  null, new EventHandler(OnImagePreview), true)
			};

			
			string ui = "<ui>" +
						"  <menubar name='MenuBar'>" +
						"    <menu action='ToolMenu'>" +
						"      <menuitem action='ImagePreview' position='top' />" +
						"    </menu>" +
						"  </menubar>" +
						"</ui>";

			nyFolder.MainWindow.Menu.AddMenus(ui, entries);
		}

		private string GenerateRealImagePath (string imagePath) {
			return(Paths.UserSharedDirectory(MyInfo.Name) + imagePath);
		}

		private void ReceiveImageThumb (UserInfo userInfo, string path, string b64image) {
			try {
				byte[] imgData = Convert.FromBase64String(b64image);
				Gdk.Pixbuf pixbuf = new Gdk.Pixbuf(imgData);
				LoadImage(userInfo, path, pixbuf);
			} catch (Exception e) {
				Debug.Log("Load {0} Image {1}: {2}", userInfo.Name, path, e.Message);
			}
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
