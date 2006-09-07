/* [ GUI/Glue/ProtocolManager.cs ] NyFolder (Protocol Manager)
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
using System.Threading;

using Niry;
using Niry.Utils;
using Niry.Network;

using NyFolder;
using NyFolder.GUI;
using NyFolder.Utils;
using NyFolder.Protocol;

namespace NyFolder.GUI.Glue {
	public class ProtocolManager {
		// ============================================
		// PRIVATE Members
		// ============================================
		private NotebookViewer notebookViewer;

		// ============================================
		// PUBLIC Constructors
		// ============================================
		public ProtocolManager (NotebookViewer nv) {
			this.notebookViewer = nv;

			NetworkManager.AddProtocolEvent += new SetProtocolEventHandler(OnAddProtocolEvent);
			NetworkManager.DelProtocolEvent += new SetProtocolEventHandler(OnDelProtocolEvent);
		}

		// ============================================
		// PRIVATE Methods
		// ============================================


		// ============================================
		// PRIVATE Methods
		// ============================================
		private void AcceptFileQuestion (PeerSocket peer, XmlRequest xml) {
			string name = (string) xml.Attributes["name"];
			string path = (string) xml.Attributes["path"];
			string size = (string) xml.Attributes["size"];
			if (size == null) size = "0";

			UserInfo userInfo = peer.Info as UserInfo;

			StringBuilder questionMsg = new StringBuilder();
			questionMsg.AppendFormat("Accept File '<b>{0}</b>' ", name);
			questionMsg.AppendFormat("(Size <b>{0}</b>)\n", FolderStore.GetSizeString(long.Parse(size)));
			questionMsg.AppendFormat("From User '<b>{0}</b>' ?", userInfo.Name);

			// Accept Yes/No Dialog
			bool accept = Glue.Dialogs.QuestionDialog("Accept File", questionMsg.ToString());
			if (accept == false) return;

			// Save File Dialog
			string savePath = Glue.Dialogs.SaveFile(Paths.UserSharedDirectory(MyInfo.Name), name);
			if (savePath == null) return;

			// Send Accept File Command
			Debug.Log("Accept File '{0}' From '{1}', Save as '{2}'", userInfo.Name, path, savePath);

			DownloadManager.AddToAcceptList(peer, path, savePath);
			CmdManager.AcceptFile(peer, xml);
		}

		// ============================================
		// PRIVATE (Methods) Event Handler
		// ============================================
		private void OnAddProtocolEvent (P2PManager p2p, CmdManager cmd) {
			// NetworkViewer
			NetworkViewer networkViewer = notebookViewer.NetworkViewer;
			networkViewer.SendFile += new SendFileHandler(OnSendFile);
			notebookViewer.SaveFile += new SendFileHandler(OnSaveFile);
			notebookViewer.FolderRefresh += new FileEventHandler(OnFolderRefresh); 

			// Protocol Commands
			CmdManager.GetEvent += new ProtocolHandler(OnGetEvent);
			CmdManager.AskEvent += new ProtocolHandler(OnAskEvent);
			CmdManager.AcceptEvent += new ProtocolHandler(OnAcceptEvent);
			CmdManager.SndEvent += new ProtocolHandler(OnSndEvent);
			CmdManager.SndStartEvent += new ProtocolHandler(OnSndStartEvent);
			CmdManager.SndEndEvent += new ProtocolHandler(OnSndEndEvent);
		}

		private void OnDelProtocolEvent (P2PManager p2p, CmdManager cmd) {
			// NetworkViewer
			NetworkViewer networkViewer = notebookViewer.NetworkViewer;
			networkViewer.SendFile -= new SendFileHandler(OnSendFile);
			notebookViewer.SaveFile -= new SendFileHandler(OnSaveFile);
			notebookViewer.FolderRefresh -= new FileEventHandler(OnFolderRefresh);

			// Protocol Commands
			CmdManager.GetEvent -= new ProtocolHandler(OnGetEvent);
			CmdManager.AskEvent -= new ProtocolHandler(OnAskEvent);
			CmdManager.AcceptEvent -= new ProtocolHandler(OnAcceptEvent);
			CmdManager.SndEvent -= new ProtocolHandler(OnSndEvent);
			CmdManager.SndStartEvent -= new ProtocolHandler(OnSndStartEvent);
			CmdManager.SndEndEvent -= new ProtocolHandler(OnSndEndEvent);
		}

		// ===================================================
		// PRIVATE (Sub Methods) NetworkViewer Event Handler
		// ===================================================
		private void OnSendFile (object sender, UserInfo userInfo, string path) {
			Gtk.Application.Invoke(delegate {
				PeerSocket peer = P2PManager.KnownPeers[userInfo] as PeerSocket;

				try {
					CmdManager.AskSendFile(peer, path);
				} catch {				
					Glue.Dialogs.MessageError("Ask Send File Error", 
													"Directory Send Not Supported (Now)");
				}
			});
		}

		private void OnSaveFile (object sender, UserInfo userInfo, string path) {
			Gtk.Application.Invoke(delegate {
				PeerSocket peer = P2PManager.KnownPeers[userInfo] as PeerSocket;

				// Save File Dialog
				string savePath = Glue.Dialogs.SaveFile(Paths.UserSharedDirectory(MyInfo.Name), path.Substring(1));
				if (savePath == null) return;

				try {
					DownloadManager.AddToAcceptList(peer, path, savePath);
					CmdManager.AcceptFile(peer, path);
				} catch (Exception e) {
					Glue.Dialogs.MessageError("Save File", e.Message);
				}
			});
		}

		private void OnFolderRefresh (object sender, string path) {
			FolderViewer folderViewer = sender as FolderViewer;
			Gtk.Application.Invoke(delegate {
				PeerSocket peer = P2PManager.KnownPeers[folderViewer.UserInfo] as PeerSocket;
				CmdManager.RequestFolder(peer, path);
			});
		}

		// ===================================================
		// PRIVATE (Sub Methods) Protocol Event Handler
		// ===================================================
		public void OnGetEvent (PeerSocket peer, XmlRequest xml) {
			Gtk.Application.Invoke(delegate {
				string what = (string) xml.Attributes["what"];

				if (what == "folder") {
					CmdManager.SendFileList(peer, xml.BodyText);
				}
			});
		}

		// Evento Scatenato quando un utente decide di inviare qualcosa...
		// E' una semplice domanda "Vuoi Accettare questo file?"
		//		<ask what='file' name='...' path='...' size='...' />
		public void OnAskEvent (PeerSocket peer, XmlRequest xml) {
			Gtk.Application.Invoke(delegate {
				string what = (string) xml.Attributes["what"];

				if (what == "file") {
					AcceptFileQuestion(peer, xml);
				}
			});
		}		

		// Evento Scatenato quando un utente accetta un qualcosa...
		// Indica al Ricevente di Iniziare ad Inviare...
		//		<accept what='file' name='...' path='...' size='...' />
		public void OnAcceptEvent (PeerSocket peer, XmlRequest xml) {
			Gtk.Application.Invoke(delegate {
				string what = (string) xml.Attributes["what"];

				if (what == "file") {
					OnAcceptFileEvent(peer, xml);
				}
			});
		}

		public void OnSndEvent (PeerSocket peer, XmlRequest xml) {
			Gtk.Application.Invoke(delegate {
				string what = (string) xml.Attributes["what"];

				if (what == "folder-list") {
					UserInfo userInfo = peer.Info as UserInfo;
					FolderViewer folderViewer = notebookViewer.LookupPage(userInfo);
					folderViewer.Fill((string) xml.Attributes["path"], xml.BodyText);
				} else if (what == "file") {
					try {
						DownloadManager.GetFilePart(peer, xml);
					} catch (Exception e) {
						Glue.Dialogs.MessageError("Download File Part", e.Message);
					}
				}
			});
		}

		public void OnSndStartEvent (PeerSocket peer, XmlRequest xml) {
			Gtk.Application.Invoke(delegate {
				string what = (string) xml.Attributes["what"];

				if (what == "file") {
					try {
						DownloadManager.InitFile(peer, xml);
					} catch (Exception e) {
						Glue.Dialogs.MessageError("Start File Download", e.Message);
					}
				}
			});
		}

		public void OnSndEndEvent (PeerSocket peer, XmlRequest xml) {
			Gtk.Application.Invoke(delegate {
				string what = (string) xml.Attributes["what"];

				if (what == "file") {
					try {
						DownloadManager.SaveFile(peer, xml);
					} catch (Exception e) {
						Glue.Dialogs.MessageError("End File Download", e.Message);
					}
				}
			});
		}

		// ===================================================
		// PRIVATE (Methods) Protocol Event Handler
		// ===================================================
		private void OnAcceptFileEvent (PeerSocket peer, XmlRequest xml) {
			try {
				string path = (string) xml.Attributes["path"];
				UserInfo userInfo = peer.Info as UserInfo;
				UploadManager.Add(userInfo, path);
			} catch (Exception e) {
				Glue.Dialogs.MessageError("Accept File", e.Message);
			}
		}
	}
}
