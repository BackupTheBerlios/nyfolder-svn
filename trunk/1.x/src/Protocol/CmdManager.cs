/* [ Protocol/CmdManager.cs ] NyFolder Protocol Commands Manager
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
using System.Collections;

using Niry;
using Niry.Utils;
using Niry.Network;

using NyFolder;
using NyFolder.Utils;
using NyFolder.Protocol;

namespace NyFolder.Protocol {
	public delegate void ProtocolLoginHandler (PeerSocket peer, UserInfo info);
	public delegate void ProtocolHandler (PeerSocket peer, XmlRequest xml);
	public delegate void SetProtocolEventHandler (P2PManager p2pManager);

	public static partial class CmdManager {
		// ============================================
		// PUBLIC (Default Commands) Events
		// ============================================
		public static event ProtocolLoginHandler LoginEvent = null;
		public static event ProtocolHandler QuitEvent = null;
		public static event ProtocolHandler ErrorEvent = null;
		public static event ProtocolHandler GetEvent = null;
		public static event ProtocolHandler AskEvent = null;
		public static event ProtocolHandler AcceptEvent = null;
		public static event ProtocolHandler SndEvent = null;
		public static event ProtocolHandler SndStartEvent = null;
		public static event ProtocolHandler SndEndEvent = null;
		public static event ProtocolHandler SndAbortEvent = null;
		public static event ProtocolHandler RecvAbortEvent = null;
		public static event ProtocolHandler UnknownEvent = null;

		// ============================================
		// PUBLIC Events
		// ============================================
		public static event SetProtocolEventHandler AddProtocolEvent = null;
		public static event SetProtocolEventHandler DelProtocolEvent = null;

		// ============================================
		// PROTECTED Members
		// ============================================

		// ============================================
		// PRIVATE Members
		// ============================================

		// ============================================
		// PUBLIC Methods
		// ============================================
		/// Initialize Protocol Command Manager (Init at Startup)
		public static void Initialize() {
			P2PManager.StatusChanged += new BoolEventHandler(OnP2PStatusChanged);
		}

		// ============================================
		// PRIVATE (Methods) P2PManager Event Handlers
		// ============================================
		private static void OnP2PStatusChanged (object sender, bool status) {
			P2PManager p2pManager = P2PManager.GetInstance();
			if (status == true) {
				// P2P Is Online
				P2PManager.PeerReceived += new PeerEventHandler(OnReceived);

				// Raise Add Protocol Event Handler
				if (AddProtocolEvent != null) AddProtocolEvent(p2pManager);
			} else {
				// Raise Delete Protocol Event Handler
				if (DelProtocolEvent != null) DelProtocolEvent(p2pManager);

				// P2P Is Offline
				P2PManager.PeerReceived -= new PeerEventHandler(OnReceived);
			}
		}

		private static void OnReceived (object sender, PeerEventArgs args) {
			PeerSocket peer = sender as PeerSocket;

			// Get Response String and Check if is Valid Xml
			string xml = peer.GetResponseString();
			if (xml == null) return;

			// Remove Response Message
			peer.ResetResponse();

			// Get Xml Commands
			xml = xml.Trim();
			ArrayList xmlCmds = new ArrayList();
			lock (peer.Response) {
				int splitPos = 0;
				while ((splitPos = xml.IndexOf("><")) >= 0) {
					// Add Xml Command To Cmds
					string cmd = xml.Substring(0, splitPos + 1);
					xmlCmds.Add(cmd);

					// Remove Splitted Part
					xml = xml.Remove(0, splitPos + 1);
				}

				if (XmlRequest.IsEndedXml(xml) == false) {
					peer.Response.Insert(0, xml);
				} else {
					xmlCmds.Add(xml);
				}
			}
			
			// Start New Command Parse Thread
			new CmdParser(peer, xmlCmds);
		}

		// ============================================
		// PRIVATE Methods
		// ============================================

		// ============================================
		// PUBLIC Properties
		// ============================================
	}
}
