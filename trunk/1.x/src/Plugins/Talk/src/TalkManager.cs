/* [ Plugins/Talk/TalkManager.cs ] NyFolder (Talk Plugin)
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
using System.Threading;
using System.Collections;

using Niry;
using Niry.Utils;
using Niry.Network;
using Niry.GUI.Gtk2;

using NyFolder;
using NyFolder.GUI;
using NyFolder.Utils;
using NyFolder.Protocol;
using NyFolder.GUI.Glue;
using NyFolder.PluginLib;

namespace NyFolder.Plugins.Talk {
	public static class TalkManager {
		// ============================================
		// PUBLIC Events
		// ============================================

		// ============================================
		// PRIVATE Members
		// ============================================
		private static NotebookViewer notebookViewer = null;
		private static Hashtable talkFrames = null;	// [UserInfo] = TalkFrame

		// ============================================
		// PUBLIC Methods
		// ============================================
		public static void Initialize (INyFolder iNyFolder) {
			talkFrames = Hashtable.Synchronized(new Hashtable());
			notebookViewer = iNyFolder.MainWindow.NotebookViewer;
			notebookViewer.TabRemoved += new ObjectEventHandler(OnTalkFrameRemoved);
		}

		public static void Uninitialize() {
			if (talkFrames == null) return;

			foreach (UserInfo userInfo in talkFrames.Keys) {
				TalkFrame talkFrame = talkFrames[userInfo] as TalkFrame;

				// Remove Page From Notebook Viewer
				notebookViewer.RemoveCustom(talkFrame);
			}
			talkFrames.Clear();
			talkFrames = null;

			// Remove Events From NotebookViewer
			notebookViewer.TabRemoved -= new ObjectEventHandler(OnTalkFrameRemoved);
			notebookViewer = null;
		}

		public static TalkFrame AddTalkFrame (UserInfo userInfo) {
			TalkFrame talkFrame = LookupTalkFrame(userInfo);
			if (talkFrame != null) return(talkFrame);

			// Initialize Talk Frame
			talkFrame = new TalkFrame(userInfo);
			talkFrame.Message += new StringEventHandler(OnSendMessage);
			talkFrames.Add(userInfo, talkFrame);

			notebookViewer.AppendCustom(talkFrame, userInfo.Name, 
										new Gtk.Image("TalkBubble", IconSize.Menu));

			return(talkFrame);
		}

		public static void InsertError (UserInfo userInfo, string error) {
			TalkFrame talkFrame = LookupTalkFrame(userInfo);
			if (talkFrame != null) talkFrame.InsertError(error);
		}
		
		public static void InsertStatus (UserInfo userInfo, string status) {
			TalkFrame talkFrame = LookupTalkFrame(userInfo);
			if (talkFrame != null) talkFrame.InsertStatus(status);
		}
			
		public static void InsertMessage (UserInfo userInfo, string message) {
			TalkFrame talkFrame = AddTalkFrame(userInfo);
			talkFrame.InsertMessage(userInfo, message);
		}

		// ============================================
		// PRIVATE (Methods) Event Handlers
		// ============================================
		private static void OnSendMessage (object sender, string message) {
			TalkFrame talkFrame = sender as TalkFrame;
			
			PeerSocket peer = (PeerSocket) P2PManager.KnownPeers[talkFrame.UserInfo];
			if (peer != null) {
				SendMessage(peer, message);
				Gtk.Application.Invoke(delegate {
					talkFrame.InsertMessage(MyInfo.GetInstance(), message);
				});
			} else {
				Gtk.Application.Invoke(delegate {
					talkFrame.InsertError("Couldn't Send Message: " + message);
				});
			}
		}

		private static void OnTalkFrameRemoved (object o, object page) {
			if (talkFrames.ContainsValue(page) == true) {
				TalkFrame talkFrame = page as TalkFrame;
				talkFrame.Message -= new StringEventHandler(OnSendMessage);

				PeerSocket peer = (PeerSocket) P2PManager.KnownPeers[talkFrame.UserInfo];
				SendStatus(peer, MyInfo.GetInstance().Name + " has closed the conversation Window...");

				talkFrames.Remove(talkFrame.UserInfo);
			}
		}

		// ============================================
		// PRIVATE Methods
		// ============================================
		private static TalkFrame LookupTalkFrame (UserInfo userInfo) {
			return((TalkFrame) talkFrames[userInfo]);
		}

		private static void SendMessage (PeerSocket peer, string message) {
			XmlRequest xmlRequest = new XmlRequest();
			xmlRequest.FirstTag = "msg";
			xmlRequest.BodyText = message;
			peer.Send(xmlRequest.GenerateXml());
		}

		private static void SendStatus (PeerSocket peer, string status) {
			XmlRequest xmlRequest = new XmlRequest();
			xmlRequest.FirstTag = "msg";
			xmlRequest.BodyText = status;
			xmlRequest.Attributes.Add("type", "status");
			peer.Send(xmlRequest.GenerateXml());
		}

#if false
		private static void SendError (PeerSocket peer, string error) {
			XmlRequest xmlRequest = new XmlRequest();
			xmlRequest.FirstTag = "msg";
			xmlRequest.BodyText = error;
			xmlRequest.Attributes.Add("type", "error");
			peer.Send(xmlRequest.GenerateXml());
		}
#endif
	}
}
