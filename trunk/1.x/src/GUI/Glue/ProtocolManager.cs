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

using Niry;
using Niry.Utils;
using Niry.Network;

using NyFolder;
using NyFolder.GUI;
using NyFolder.Utils;
using NyFolder.Protocol;
using NyFolder.GUI.Base;

namespace NyFolder.GUI.Glue {
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
		// PUBLIC Methods
		// ============================================

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
			CmdManager.AcceptEvent += new ProtocolHandler(OnAcceptEvent);
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
			CmdManager.AcceptEvent -= new ProtocolHandler(OnAcceptEvent);
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
		}

		// =================================================
		// PROTECTED (Methods) Folder Viewers Event Handlers
		// =================================================
		private void OnSaveFile (object obj, UserInfo userInfo, string path) {
		}

		private void OnSendFileMenu (object obj, string path, bool isDir) {
		}

		private void OnFolderRefresh (object obj, string path) {
		}

		// =================================================
		// PROTECTED (Methods) Protocol Cmds Event Handlers
		// =================================================
		private void OnGetEvent (PeerSocket peer, XmlRequest xml) {
		}

		private void OnAskEvent (PeerSocket peer, XmlRequest xml) {
		}

		private void OnAcceptEvent (PeerSocket peer, XmlRequest xml) {
		}

		private void OnSndEvent (PeerSocket peer, XmlRequest xml) {
		}

		private void OnSndStartEvent (PeerSocket peer, XmlRequest xml) {
		}

		private void OnSndEndEvent (PeerSocket peer, XmlRequest xml) {
		}

		private void OnSndAbortEvent (PeerSocket peer, XmlRequest xml) {
		}

		private void OnRecvAbortEvent (PeerSocket peer, XmlRequest xml) {
		}


		// ============================================
		// PRIVATE Methods
		// ============================================

		// ============================================
		// PUBLIC Properties
		// ============================================
	}
}
