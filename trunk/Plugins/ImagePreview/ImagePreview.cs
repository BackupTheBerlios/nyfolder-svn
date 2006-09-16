/* [ Plugins/ImagePreview/ImagePreview.cs ] NyFolder (ImagePreview Plugin)
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

using Niry;
using Niry.Utils;
using Niry.Network;
using Niry.GUI.Gtk2;

using NyFolder;
using NyFolder.GUI;
using NyFolder.Utils;
using NyFolder.Protocol;

namespace NyFolder.Plugins.ImagePreview {
	public class ImagePreview : Plugin {
		// ============================================
		// PUBLIC CONSTS
		// ============================================
		public const int ThumbWidth = 100;
		public const int ThumbHeight = 52;

		// ============================================
		// PROTECTED Members
		// ============================================
		protected INyFolder nyFolder = null;

		// ============================================
		// PRIVATE Members
		// ============================================
		private NotebookViewer notebookViewer = null;

		// ============================================
		// PUBLIC Constructors
		// ============================================
		public ImagePreview() {
		}

		~ImagePreview() {
		}

		// ============================================
		// PUBLIC Methods
		// ============================================
		public override void Initialize (INyFolder nyFolder) {
			this.nyFolder = nyFolder;
			this.nyFolder.MainWindowStarted += new BlankEventHandler(OnMainWindowStarted);
			this.nyFolder.QuittingApplication += new BlankEventHandler(OnMainAppQuit);
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
		// PROTECTED (Methods) Event Handlers
		// ============================================
		protected void OnMainWindowStarted (object sender) {
			// Initialize Notebook Viewer Events
			this.notebookViewer = this.nyFolder.Window.NotebookViewer;
			this.notebookViewer.FileAdded += new ObjectEventHandler(OnFileAdded);

			// Initialize Protocol Events
			P2PManager.StatusChanged += new BoolEventHandler(OnP2PStatusChanged);
		}

		protected void OnMainAppQuit (object sender) {
			// Notebook Viewer Events
			this.notebookViewer.FileAdded -= new ObjectEventHandler(OnFileAdded);
			this.notebookViewer = null;

			// Protocol Events
			P2PManager.StatusChanged -= new BoolEventHandler(OnP2PStatusChanged);
			if (P2PManager.IsListening() == true) OnP2PStatusChanged(null, false);
		}

		protected void OnP2PStatusChanged (object sender, bool status) {
			if (status == true) {
				CmdManager.GetEvent += new ProtocolHandler(OnGetEvent);
				CmdManager.SndEvent += new ProtocolHandler(OnSndEvent);
			} else {
				CmdManager.GetEvent -= new ProtocolHandler(OnGetEvent);
				CmdManager.SndEvent -= new ProtocolHandler(OnSndEvent);
			}
		}

		protected void OnFileAdded (object sender, object storeIter) {
			FolderViewer folderViewer = sender as FolderViewer;
			Gtk.TreeIter iter = (Gtk.TreeIter) storeIter;
			FolderStore store = folderViewer.Store;

			// Request Image Thumb if it's Image
			string filePath = store.GetFilePath(iter);
			if (IsImage(Path.GetExtension(filePath)) == true) {
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

		// ============================================
		// PRIVATE Methods
		// ============================================
		private bool IsImage (string extension) {
			if (extension == String.Empty || extension == null || extension.Length <= 0)
				return(false);
			extension = extension.Substring(1);

			string[] pixbufExt = new string[] {
				"wmf", "ani", "bmp", "gif", "ico", "jpg", "jpeg", "pcx", "png", 
				"pnm", "ras", "tga", "tiff", "wbmp", "xbm", "xpm", "svg"
			};

			foreach (string ext in pixbufExt)
				if (ext == extension) return(true);
			return(false);
		}

		private string GenerateRealImagePath (string imagePath) {
			return(Paths.UserSharedDirectory(MyInfo.Name) + imagePath);
		}

		private void ReceiveImageThumb (UserInfo userInfo, string path, string b64image) {
			byte[] imgData = Convert.FromBase64String(b64image);
			Gdk.Pixbuf pixbuf = new Gdk.Pixbuf(imgData);
			LoadImage(userInfo, path, pixbuf);
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
