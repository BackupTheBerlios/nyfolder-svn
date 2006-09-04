/* [ Plugins/Talk/TalkManager.cs ] NyFolder (Talk Manager)
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
using System.Collections;

using Niry;
using Niry.Network;

using NyFolder;
using NyFolder.GUI;
using NyFolder.Utils;
using NyFolder.Protocol;

namespace NyFolder.Plugins.Talk {
	public static class TalkManager {
		// ============================================
		// PUBLIC Events
		// ============================================

		// ============================================
		// PRIVATE Members
		// ============================================
		private static NotebookViewer notebookViewer = null;
		private static Hashtable talks = null;	// PeerSocket = TalkEditor

		// ============================================
		// PUBLIC Methods
		// ============================================
		public static void Initialize (NotebookViewer nv) {
			notebookViewer = nv;

			talks = new Hashtable();
			CmdManager.UnknownEvent += new ProtocolHandler(OnUnmanagedProtocolEvent);
		}

		public static void Reset() {
			CloseAllTalks();
			CmdManager.UnknownEvent -= new ProtocolHandler(OnUnmanagedProtocolEvent);
		}

		public static TalkEditor GetTalkEditor (PeerSocket peer) {
			TalkEditor talkEditor = (TalkEditor) talks[peer];

			if (talkEditor == null) {
				talkEditor = new TalkEditor(peer);
				talks.Add(peer, talkEditor);
			}

			UserInfo userInfo = peer.Info as UserInfo;
			notebookViewer.AppendCustom(talkEditor, userInfo.Name, 
										new Gtk.Image("TalkBubble", IconSize.Menu));
			return(talkEditor);
		}

		public static void AddMessage (PeerSocket peer, string message) {
			TalkEditor talkEditor = GetTalkEditor(peer);
			talkEditor.AddMessage(peer, message);
		}

		// ============================================
		// PRIVATE (Methods) Event Handlers
		// ============================================
		private static void OnUnmanagedProtocolEvent (PeerSocket peer, XmlRequest xml) {
			if (xml.FirstTag == "msg") {
				Gtk.Application.Invoke(delegate { AddMessage(peer, xml.BodyText); });
			}
		}

		private static void OnWriteMessage (object sender, string message) {
			TalkEditor talkEditor = sender as TalkEditor;
			talkEditor.Peer.Send(message);

			Gtk.Application.Invoke(delegate { talkEditor.AddMessage(message); });
		}

		// ============================================
		// PRIVATE Methods
		// ============================================
		private static void CloseAllTalks() {
			foreach (PeerSocket peer in talks.Keys) {
				TalkEditor talkEditor = talks[peer] as TalkEditor;
				talkEditor.Message -= new MessageEventHandler(OnWriteMessage);
			}
			talks.Clear();
			talks = null;
		}

		// ============================================
		// PUBLIC Properties
		// ============================================
	}
}
