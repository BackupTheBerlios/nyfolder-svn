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


		// ============================================
		// PRIVATE (Methods) Event Handler
		// ============================================
		private void OnAddProtocolEvent (P2PManager p2p, CmdManager cmd) {
			// NetworkViewer
			NetworkViewer networkViewer = notebookViewer.NetworkViewer;
			networkViewer.SendFile += new SendFileHandler(OnSendFile);

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
			PeerSocket peer = P2PManager.KnownPeers[userInfo] as PeerSocket;
			try {
				CmdManager.AskSendFile(peer, path);
			} catch {
				Gtk.Application.Invoke(delegate {
					Glue.Dialogs.MessageErrorDialog("Ask Send File Error", 
													"Directory Send Not Supported (Now)");
				});
			}
		}

		// ===================================================
		// PRIVATE (Sub Methods) Protocol Event Handler
		// ===================================================
		public void OnGetEvent (PeerSocket peer, XmlRequest xml) {
			Console.WriteLine("Get Event");
		}

		public void OnAskEvent (PeerSocket peer, XmlRequest xml) {
			Console.WriteLine("Ask Event");
		}

		public void OnAcceptEvent (PeerSocket peer, XmlRequest xml) {
			Console.WriteLine("Accept Event");
		}

		public void OnSndEvent (PeerSocket peer, XmlRequest xml) {
			Console.WriteLine("Snd Event");
		}

		public void OnSndStartEvent (PeerSocket peer, XmlRequest xml) {
			Console.WriteLine("Snd Start Event");
			#warning ToDo All This :)
		}

		public void OnSndEndEvent (PeerSocket peer, XmlRequest xml) {
			Console.WriteLine("Snd End Event");
		}
	}
}
