/* [ Plugins/NetworkBootstrap/NetworkBootstrap.cs ] NyFolder (Network Bootstrap Plugin)
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

using NyFolder;
using NyFolder.GUI;
using NyFolder.Utils;
using NyFolder.Protocol;
using NyFolder.GUI.Glue;

namespace NyFolder.Plugins.NetworkBootstrap {
	public class NetworkBootstrap : Plugin {
		// ============================================
		// PUBLIC Events
		// ============================================

		// ============================================
		// PROTECTED Members
		// ============================================
		protected INyFolder nyFolder = null;

		// ============================================
		// PRIVATE Members
		// ============================================

		// ============================================
		// PUBLIC Constructors
		// ============================================
		public NetworkBootstrap() {
		}

		~NetworkBootstrap() {
		}

		// ============================================
		// PUBLIC Methods
		// ============================================
		public override void Initialize (INyFolder nyFolder) {
			this.nyFolder = nyFolder;
			this.nyFolder.MainWindowStarted += new BlankEventHandler(OnMainWindowStarted);
			this.nyFolder.QuittingApplication += new BlankEventHandler(OnMainAppQuit);
		}

		// ============================================
		// PROTECTED (Methods) Event Handlers
		// ============================================
		protected void OnMainWindowStarted (object sender) {
			// Initialize Protocol Events
			NetworkManager.AddProtocolEvent += new SetProtocolEventHandler(OnAddProtocolCmd);
			NetworkManager.DelProtocolEvent += new SetProtocolEventHandler(OnDelProtocolCmd);
		}

		protected void OnMainAppQuit (object sender) {
			// Initialize Protocol Events
			NetworkManager.AddProtocolEvent -= new SetProtocolEventHandler(OnAddProtocolCmd);
			NetworkManager.DelProtocolEvent -= new SetProtocolEventHandler(OnDelProtocolCmd);
		}

		protected void OnAddProtocolCmd (P2PManager p2p, CmdManager cmd) {
			CmdManager.LoginEvent += new ProtocolLoginHandler(OnPeerLogin);
			CmdManager.GetEvent += new ProtocolHandler(OnGetEvent);
			CmdManager.UnknownEvent += new ProtocolHandler(OnUnknownEvent);
		}

		protected void OnDelProtocolCmd (P2PManager p2p, CmdManager cmd) {
			CmdManager.LoginEvent -= new ProtocolLoginHandler(OnPeerLogin);
			CmdManager.GetEvent -= new ProtocolHandler(OnGetEvent);
			CmdManager.UnknownEvent -= new ProtocolHandler(OnUnknownEvent);
		}

		// TODO: Add OnAddUser
		protected void OnPeerLogin (PeerSocket peer, UserInfo userInfo) {
			// Waiting Peer Initialization Time, Bad Bad Bad!!!
			Thread.Sleep(6000);
			// Request Peer List
			RequestPeerList(peer);
		}

		protected void OnGetEvent (PeerSocket peer, XmlRequest xml) {
			if (xml.Attributes["what"].Equals("peerlist") == true) {
				SendPeerList(peer);
			}
		}

		protected void OnUnknownEvent (PeerSocket peer, XmlRequest xml) {
			if (xml.FirstTag != "peerlist") return;

			ResponseReader reader = new ResponseReader(xml.BodyText);
			ArrayList peerList = reader.Elements;
			foreach (Hashtable elem in peerList) {
				if (IsInMyList(elem) == true) continue;
				Console.WriteLine(" - {0} {1}", elem["name"], elem["secure"]);
			}
		}

		// ============================================
		// PRIVATE Methods
		// ============================================
		private void RequestPeerList (PeerSocket peer) {
			XmlRequest xml = new XmlRequest();
			xml.FirstTag = "get";
			xml.Attributes.Add("what", "peerlist");
			peer.Send(xml.GenerateXml());
		}

		private void SendPeerList (PeerSocket peer) {
			XmlRequest xml = new XmlRequest();
			xml.FirstTag = "peerlist";
			xml.BodyText = GeneratePeerList();		
			peer.Send(xml.GenerateXml());
		}

		private string GeneratePeerList() {
			ResponseWriter peerList = new ResponseWriter();

			foreach (UserInfo userInfo in P2PManager.KnownPeers.Keys) {
				if (userInfo.SecureAuthentication == true) {
					peerList.Add(userInfo.Name);
				} else {
					peerList.Add(userInfo.Name, userInfo.Ip, userInfo.Port);
				}
			}

			return(peerList.ToString());
		}

		private bool IsInMyList (Hashtable user) {
			foreach (UserInfo userInfo in P2PManager.KnownPeers.Keys) {
				if ((string) user["name"] != userInfo.Name) continue;

				if (bool.Parse((string) user["secure"]) == true) {
					if (userInfo.SecureAuthentication == true)
						return(true);
				} else {
					if (userInfo.SecureAuthentication == false && 
						userInfo.Ip == (string) user["ip"] &&
						userInfo.Port == int.Parse((string) user["port"]))
					{
						return(true);
					}
				}
			}
			return(false);
		}

		// ============================================
		// PUBLIC Properties
		// ============================================
	}
}
