/* [ Protocol/CmdParser.cs ] NyFolder Protocol Command Parser
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
using System.Threading;
using System.Collections;

using Niry;
using Niry.Utils;
using Niry.Network;

using NyFolder;
using NyFolder.Utils;
using NyFolder.Protocol;

namespace NyFolder.Protocol {
	public static partial class CmdManager {
		public class CmdParser {
			// ============================================
			// PUBLIC Events
			// ============================================
			/// Raised When Parser Fail
			public static StringEventHandler Error = null;
	
			// ============================================
			// PRIVATE Members
			// ============================================
			private ArrayList xmlCmds;
			private PeerSocket peer;
			private Thread thread;

			// ============================================
			// PUBLIC Constructors
			// ============================================
			public CmdParser (PeerSocket peer, ArrayList xmlCmds) {
				// Initialize Events (None)
				Error = null;

				// Initialize Members
				this.xmlCmds = xmlCmds;
				this.peer = peer;

				// Start Command Parser Thread
				this.thread = new Thread(new ThreadStart(ParseXml));
				this.thread.Start();
			}

			// ============================================
			// PUBLIC Methods
			// ============================================

			// ============================================
			// PRIVATE Methods
			// ============================================
			private void ParseXml() {
				foreach (string xml in xmlCmds)
					ParseCommand(xml);
				xmlCmds.Clear();
			}
	
			private void ParseCommand (string xml) {
				XmlRequest xmlRequest = null;
				try {
//					Debug.Log(xml);
					xmlRequest = new XmlRequest(xml);
					xmlRequest.Parse();
				} catch (Exception e) {
					// Raise Xml Parse Error
					if (Error != null) Error(e, xml);
					return;
				}
	
				// Protocol Commands
				SendProtocolEvent(xmlRequest);
			}
	
			private void SendProtocolEvent (XmlRequest xmlRequest) {
				switch (xmlRequest.FirstTag) {
					case "login":
						Login login = new Login(peer, xmlRequest);
						if (login.Authentication() == true && login.User != null) {
							// Add to Known User
							P2PManager.AddPeer(login.User, peer);

							// Start Login Event
							CmdManager.LoginEvent(peer, login.User);
						} else {
							Debug.Log("Auth Failed: {0}", peer.GetRemoteIP());
						}
						break;
					case "quit":
						CmdManager.QuitEvent(peer, xmlRequest);
						break;
					case "error":
						CmdManager.ErrorEvent(peer, xmlRequest);
						break;
					case "get":
						CmdManager.GetEvent(peer, xmlRequest);
						break;
					case "ask":
						CmdManager.AskEvent(peer, xmlRequest);
						break;
					case "snd":
						CmdManager.SndEvent(peer, xmlRequest);
						break;
					case "snd-start":
						CmdManager.SndStartEvent(peer, xmlRequest);
						break;
					case "snd-end":
						CmdManager.SndEndEvent(peer, xmlRequest);
						break;
					case "snd-abort":
						CmdManager.SndAbortEvent(peer, xmlRequest);
						break;
					case "recv-abort":
						CmdManager.RecvAbortEvent(peer, xmlRequest);
						break;
					default:
						CmdManager.UnknownEvent(peer, xmlRequest);
						break;				
				}
			}
		}
	}
}
