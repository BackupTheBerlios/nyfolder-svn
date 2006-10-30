/* [ GUI/Glue/ProtocolManager.cs ] NyFolder Protocol Manager Glue
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

using System;
using System.IO;
using System.Text;

using Niry;
using Niry.Utils;
using Niry.Network;

using NyFolder;
using NyFolder.GUI;
using NyFolder.Utils;
using NyFolder.Protocol;
using NyFolder.GUI.Base;

namespace NyFolder.GUI.Glue {
	/// Protocol Manager
	public sealed class ProtocolManager {
		// ============================================
		// PRIVATE Members
		// ============================================
		private NotebookViewer notebookViewer;
		private NetworkViewer networkViewer;

		// ============================================
		// PUBLIC Constructors
		// ============================================
		public ProtocolManager (GUI.Window window) {
			// Initialize Components
			this.notebookViewer = window.NotebookViewer;
			this.networkViewer = this.notebookViewer.NetworkViewer;

			// Add Events Handler
			CmdManager.AddProtocolEvent += new SetProtocolEventHandler(OnAddProtocolEvent);
			CmdManager.DelProtocolEvent += new SetProtocolEventHandler(OnDelProtocolEvent);
		}

		// ============================================
		// PROTECTED (Methods) Event Handlers
		// ============================================
		private void OnAddProtocolEvent (P2PManager p2pManager) {
			// Network Viewer
			networkViewer.SendFile += new SendFileHandler(OnSendFile);

			// Folder Viewers
			notebookViewer.SaveFile += new SendFileHandler(OnSaveFile);
			notebookViewer.FileSend += new FileSendEventHandler(OnSendFileMenu);
			notebookViewer.FolderRefresh += new StringEventHandler(OnFolderRefresh); 

			// Protocol Commands
			CmdManager.GetEvent += new ProtocolHandler(OnGetEvent);
			CmdManager.AskEvent += new ProtocolHandler(OnAskEvent);
			CmdManager.SndEvent += new ProtocolHandler(OnSndEvent);
			CmdManager.SndStartEvent += new ProtocolHandler(OnSndStartEvent);
			CmdManager.SndEndEvent += new ProtocolHandler(OnSndEndEvent);
			CmdManager.SndAbortEvent += new ProtocolHandler(OnSndAbortEvent);
			CmdManager.RecvAbortEvent += new ProtocolHandler(OnRecvAbortEvent);
		}

		private void OnDelProtocolEvent (P2PManager p2pManager) {
			// Network Viewer
			networkViewer.SendFile -= new SendFileHandler(OnSendFile);

			// Folder Viewers
			notebookViewer.SaveFile -= new SendFileHandler(OnSaveFile);
			notebookViewer.FileSend -= new FileSendEventHandler(OnSendFileMenu);
			notebookViewer.FolderRefresh -= new StringEventHandler(OnFolderRefresh); 

			// Protocol Commands
			CmdManager.GetEvent -= new ProtocolHandler(OnGetEvent);
			CmdManager.AskEvent -= new ProtocolHandler(OnAskEvent);
			CmdManager.SndEvent -= new ProtocolHandler(OnSndEvent);
			CmdManager.SndStartEvent -= new ProtocolHandler(OnSndStartEvent);
			CmdManager.SndEndEvent -= new ProtocolHandler(OnSndEndEvent);
			CmdManager.SndAbortEvent -= new ProtocolHandler(OnSndAbortEvent);
			CmdManager.RecvAbortEvent -= new ProtocolHandler(OnRecvAbortEvent);
		}

		// =================================================
		// PROTECTED (Methods) Network Viewer Event Handlers
		// =================================================
		private void OnSendFile (object obj, UserInfo userInfo, string path) {
		Gtk.Application.Invoke(delegate {
			PeerSocket peer = P2PManager.KnownPeers[userInfo] as PeerSocket;
			UploadManager.Add(peer, path);
		});
		}

		// =================================================
		// PROTECTED (Methods) Folder Viewers Event Handlers
		// =================================================
		private void OnSaveFile (object obj, UserInfo userInfo, string path) {
		Gtk.Application.Invoke(delegate {
#if true
			PeerSocket peer = P2PManager.KnownPeers[userInfo] as PeerSocket;

			// Save File Dialog
			string saveAs = Base.Dialogs.SaveFile(Paths.UserSharedDirectory(MyInfo.Name), path.Substring(1));
			if (saveAs == null) return;

			// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
			// DHO, TODO ME
			// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
			DownloadManager.Accept(peer, 0, path, saveAs);
			Cmd.RequestFile(peer, path);
#else
			Debug.Log("TODO: ProtocolManager.OnSaveFile() Dho, Dho, Dho!!!");
#endif
		});
		}

		private void OnSendFileMenu (object obj, string path, bool isDir) {
		Gtk.Application.Invoke(delegate {
			if (isDir == true) {
				Base.Dialogs.MessageError("Ask Send File Error",
										  "Directory Send Not Supported (Now)");
				return;
			}

			PeerSocket peer = obj as PeerSocket;
			UploadManager.Add(peer, path);
		});
		}

		private void OnFolderRefresh (object obj, string path) {
		Gtk.Application.Invoke(delegate {
			FolderViewer folderViewer = obj as FolderViewer;
			PeerSocket peer = P2PManager.KnownPeers[folderViewer.UserInfo] as PeerSocket;
			Cmd.RequestFolder(peer, path);
		});
		}

		// =================================================
		// PROTECTED (Methods) Protocol Cmds Event Handlers
		// =================================================
		/// <get what='file' id='10' />
		private void OnGetEvent (PeerSocket peer, XmlRequest xml) {
		Gtk.Application.Invoke(delegate {
			string what = (string) xml.Attributes["what"];

			switch (what) {
				case "file-id":
					uint id = uint.Parse((string) xml.Attributes["id"]);
					UploadManager.Send(peer, id);
					// TODO: Manage ID Not Found
					break;
				case "file":
					string filePath = (string) xml.Attributes["path"];
					filePath = Path.Combine(Paths.UserSharedDirectory(MyInfo.Name), filePath.Substring(1));
					UploadManager.Send(peer, filePath);
					break;
				case "file-list":
					string folderPath = (string) xml.Attributes["path"];
					Cmd.SendFileList(peer, folderPath);
					break;
			}
		});
		}

		/// <ask what='file' id='10' path='/pippo.txt' size='1024' />
		private void OnAskEvent (PeerSocket peer, XmlRequest xml) {
		Gtk.Application.Invoke(delegate {
			string what = (string) xml.Attributes["what"];

			if (what == "file") {
				AcceptFileQuestion(peer, xml);
			}
		});
		}

		/// <snd what='file' id='10' part='13'>...</snd>
		private void OnSndEvent (PeerSocket peer, XmlRequest xml) {
		Gtk.Application.Invoke(delegate {
			string what = (string) xml.Attributes["what"];

			switch (what) {
				case "file":
					uint id = uint.Parse((string) xml.Attributes["id"]);
					DownloadManager.GetFilePart(peer, id, xml);
					break;
				case "file-list":
					UserInfo userInfo = peer.Info as UserInfo;
					FolderViewer folderViewer = notebookViewer.LookupPage(userInfo);
					folderViewer.Fill((string) xml.Attributes["path"], xml.BodyText);
					break;
			}
		});
		}

		/// <snd-start what='file' id='10' name='/pippo.txt' size='1024' />  
		private void OnSndStartEvent (PeerSocket peer, XmlRequest xml) {
		Gtk.Application.Invoke(delegate {
			string what = (string) xml.Attributes["what"];

			if (what == "file") {
				uint id = uint.Parse((string) xml.Attributes["id"]);
				DownloadManager.InitDownload(peer, id, xml);
			}
		});
		}

		/// <snd-end what='file' id='10 />
		private void OnSndEndEvent (PeerSocket peer, XmlRequest xml) {
		Gtk.Application.Invoke(delegate {
			string what = (string) xml.Attributes["what"];

			if (what == "file") {
				uint id = uint.Parse((string) xml.Attributes["id"]);
				DownloadManager.FinishedDownload(peer, id);
			}
		});
		}

		/// <snd-abort what='file' id='10' />
		private void OnSndAbortEvent (PeerSocket peer, XmlRequest xml) {
		Gtk.Application.Invoke(delegate {
			string what = (string) xml.Attributes["what"];

			if (what == "file") {
				uint id = uint.Parse((string) xml.Attributes["id"]);
				DownloadManager.AbortDownload(peer, id);
			}
		});
		}

		/// <recv-abort what='file' id='10' />
		private void OnRecvAbortEvent (PeerSocket peer, XmlRequest xml) {
		Gtk.Application.Invoke(delegate {
			string what = (string) xml.Attributes["what"];

			if (what == "file") {
				uint id = uint.Parse((string) xml.Attributes["id"]);
				UploadManager.Abort(peer, id);
			}
		});
		}

		// ============================================
		// PRIVATE Methods
		// ============================================
		private void AcceptFileQuestion (PeerSocket peer, XmlRequest xml) {
			string name = (string) xml.Attributes["name"];
			string size = (string) xml.Attributes["size"];
			uint id = uint.Parse((string) xml.Attributes["id"]);

			UserInfo userInfo = peer.Info as UserInfo;

			StringBuilder questionMsg = new StringBuilder();
			questionMsg.AppendFormat("Accept File '<b>{0}</b>' ", name);
			questionMsg.AppendFormat("(Size <b>{0}</b>)\n", FileUtils.GetSizeString(long.Parse(size)));
			questionMsg.AppendFormat("From User '<b>{0}</b>' ?", userInfo.Name);

			// Accept Yes/No Dialog
			bool accept = Base.Dialogs.QuestionDialog("Accept File", questionMsg.ToString());
			if (accept == false) return;

			// Save File Dialog
			string savePath = Base.Dialogs.SaveFile(Paths.UserSharedDirectory(MyInfo.Name), name);
			if (savePath == null) return;

			// Send Accept File Command
			Debug.Log("Accept File '{0}' From '{1}', Save as '{2}'", userInfo.Name, name, savePath);

			DownloadManager.Accept(peer, id, name, savePath);
			Cmd.RequestFile(peer, id);
		}
	}
}
