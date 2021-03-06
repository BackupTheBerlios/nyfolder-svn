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

		~ProtocolManager() {
			CmdManager.AddProtocolEvent -= new SetProtocolEventHandler(OnAddProtocolEvent);
			CmdManager.DelProtocolEvent -= new SetProtocolEventHandler(OnDelProtocolEvent);
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
			PeerSocket peer = P2PManager.KnownPeers[userInfo] as PeerSocket;

			string saveAs = FileAlreadyInUse(path.Substring(1));
			if (saveAs != null) {
				DownloadManager.Accept(peer, 0, path, saveAs);
				Cmd.RequestFile(peer, path);
			}
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
					try {
						ulong id = ulong.Parse((string) xml.Attributes["id"]);
						UploadManager.Send(peer, id);
						// TODO: Manage ID Not Found
					} catch (Exception e) {
						Base.Dialogs.MessageError("File Not Found", e.Message);
					}
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
					ulong id = ulong.Parse((string) xml.Attributes["id"]);
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
				ulong id = ulong.Parse((string) xml.Attributes["id"]);
				DownloadManager.InitDownload(peer, id, xml);
			}
		});
		}

		/// <snd-end what='file' id='10 />
		private void OnSndEndEvent (PeerSocket peer, XmlRequest xml) {
		Gtk.Application.Invoke(delegate {
			string what = (string) xml.Attributes["what"];

			if (what == "file") {
				ulong id = ulong.Parse((string) xml.Attributes["id"]);
				DownloadManager.FinishedDownload(peer, id);
			}
		});
		}

		/// <snd-abort what='file' id='10' />
		private void OnSndAbortEvent (PeerSocket peer, XmlRequest xml) {
		Gtk.Application.Invoke(delegate {
			string what = (string) xml.Attributes["what"];

			if (what == "file") {
				ulong id = ulong.Parse((string) xml.Attributes["id"]);
				DownloadManager.AbortDownload(peer, id);
			}
		});
		}

		/// <recv-abort what='file' id='10' />
		private void OnRecvAbortEvent (PeerSocket peer, XmlRequest xml) {
		Gtk.Application.Invoke(delegate {
			string what = (string) xml.Attributes["what"];

			if (what == "file") {
				ulong id = ulong.Parse((string) xml.Attributes["id"]);
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
			ulong id = ulong.Parse((string) xml.Attributes["id"]);

			UserInfo userInfo = peer.Info as UserInfo;

			StringBuilder questionMsg = new StringBuilder();
			questionMsg.AppendFormat("Accept File '<b>{0}</b>' ", name);
			questionMsg.AppendFormat("(Size <b>{0}</b>)\n", FileUtils.GetSizeString(long.Parse(size)));
			questionMsg.AppendFormat("From User '<b>{0}</b>' ?", userInfo.Name);

			// Accept Yes/No Dialog
			bool accept = Base.Dialogs.QuestionDialog("Accept File", questionMsg.ToString());
			if (accept == false) return;

			// Save File Dialog
			string savePath = FileAlreadyInUse(name);
			if (savePath == null) return;

			// Send Accept File Command
			Debug.Log("Accept File '{0}' From '{1}', Save as '{2}'", userInfo.Name, name, savePath);

			DownloadManager.Accept(peer, id, name, savePath);
			Cmd.RequestFile(peer, id);
		}

		private string FileAlreadyInUse (string fileName) {
			string saveAs = null;
			bool goOn = true;
			do {
				// Save File Dialog
				saveAs = Base.Dialogs.SaveFile(Paths.UserSharedDirectory(MyInfo.Name), fileName);
				if (saveAs == null) return(null);

				if (DownloadManager.IsInList(saveAs) == true) {
					string msg = "Currently you are Downloading file that you have saved as '" + saveAs + "'.\n" +
								 "I've to stop the Download of It in favor of this?";
					goOn = GUI.Base.Dialogs.QuestionDialog("Name Conflict", msg);

					if (goOn == true) DownloadManager.AbortDownload(saveAs);
				} else if (UploadManager.IsInList(saveAs) == true) {
					string msg = "Currently you are Uploading file '"+saveAs+"' that you want replace.\n" +
								 "I've to stop the Upload of It favor of this?";
					goOn = GUI.Base.Dialogs.QuestionDialog("Name Conflict", msg);
					if (goOn == true) DownloadManager.AbortDownload(saveAs);
				}
			} while (goOn == false);
			return(saveAs);
		}
	}
}
