/* [ Protocol/CmdManager.cs ] NyFolder Protocol (Cmd Manager)
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
using System.Threading;
using System.Collections;

using Niry;
using Niry.Network;

using NyFolder;
using NyFolder.Utils;

namespace NyFolder.Protocol {
	public delegate void ProtocolLoginHandler (PeerSocket peer, UserInfo info);
	public delegate void ProtocolHandler (PeerSocket peer, XmlRequest xml);

	internal class CmdParse {
		private ArrayList xmlCmds;
		private PeerSocket peer;
		private Thread thread;

		public CmdParse (PeerSocket peer, ArrayList xmlCmds) {
			this.xmlCmds = xmlCmds;
			this.peer = peer;

			// Start Command Parser Thread
			thread = new Thread(new ThreadStart(ParseXml));
			thread.Start();
		}

		private void ParseXml() {
			foreach (string xml in xmlCmds) 
				ParseCommand(xml);
		}

		private void ParseCommand (string xml) {
//#if false
			Debug.Log("==================================================");
			if (peer.Info != null) {
				UserInfo userInfo = this.peer.Info as UserInfo;
				Debug.Log("Response From: {0}", userInfo.Name);
			}
			Debug.Log("Response: '{0}'", xml);
			Debug.Log("==================================================");
//#endif

			// Parse Xml Command
			XmlRequest xmlRequest = new XmlRequest(xml);
			xmlRequest.Parse();

			// Protocol Commands
			switch (xmlRequest.FirstTag) {
				case "login":
					Login login = new Login(peer, xmlRequest);
					if (login.Authentication() == true) {
						// Add to Known User
						P2PManager.AddPeer(login.User, peer);

						// Start Login Event
						CmdManager.StartLoginEvent(peer, login.User);
					}
					break;
				case "quit":
					CmdManager.StartQuitEvent(peer, xmlRequest);
					break;
				case "error":
					CmdManager.StartErrorEvent(peer, xmlRequest);
					break;
				case "get":
					CmdManager.StartGetEvent(peer, xmlRequest);
					break;
				case "ask":
					CmdManager.StartAskEvent(peer, xmlRequest);
					break;
				case "accept":
					CmdManager.StartAcceptEvent(peer, xmlRequest);
					break;
				case "snd":
					CmdManager.StartSndEvent(peer, xmlRequest);
					break;
				case "snd-start":
					CmdManager.StartSndStartEvent(peer, xmlRequest);
					break;
				case "snd-end":
					CmdManager.StartSndEndEvent(peer, xmlRequest);
					break;
				default:
					CmdManager.StartUnknownEvent(peer, xmlRequest);
					break;
			}
		}
	}

	public class CmdManager {
		// ============================================
		// PUBLIC Events
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
		public static event ProtocolHandler UnknownEvent = null;

		// ============================================
		// PRIVATE (Singleton) Members
		// ============================================
		private static CmdManager manager = null;

		// ============================================
		// PRIVATE Members
		// ============================================
		private P2PManager p2pManager = null;
		
		// ============================================
		// PRIVATE Constructors
		// ============================================
		private CmdManager() {
			p2pManager = P2PManager.GetInstance();
		}
		
		~CmdManager() {
			Debug.Log("Cmd Manager - Disposed");
			DelPeerEventsHandler();
			manager = null;
		}

		// ============================================
		// PUBLIC Methods
		// ============================================
		public static CmdManager GetInstance() {
			if (manager == null)
				manager = new CmdManager();
			return(manager);
		}
		
		public void AddPeerEventsHandler() {
			P2PManager.PeerReceived += new PeerEventHandler(OnReceived);
			Debug.Log("Cmd Manager - AddPeerEventsHandler");
		}

		public void DelPeerEventsHandler() {
			P2PManager.PeerReceived -= new PeerEventHandler(OnReceived);
			Debug.Log("Cmd Manager - DelPeerEventsHandler");
		}
		
		// ============================================
		// PUBLIC STATIC Methods (Commands)
		// ============================================
		public static void Login (PeerSocket peer, UserInfo userInfo) {
			XmlRequest xmlRequest = new XmlRequest();
			xmlRequest.FirstTag = "login";
			xmlRequest.Attributes.Add("name", userInfo.Name);
			xmlRequest.Attributes.Add("secure", userInfo.SecureAuthentication.ToString());
			xmlRequest.Attributes.Add("port", manager.p2pManager.CurrentPort);
			peer.Send(xmlRequest.GenerateXml());
		}

		public static void Error (PeerSocket peer, string message) {
			XmlRequest xmlRequest = new XmlRequest();
			xmlRequest.FirstTag = "error";
			xmlRequest.BodyText = message;
			peer.Send(xmlRequest.GenerateXml());
		}

		public static void Error (PeerSocket peer, string f, params object[] objs) {
			StringBuilder message = new StringBuilder();
			message.AppendFormat(f, objs);
			Error(peer, message.ToString());
		}

		public static void RequestFolder (PeerSocket peer, string path) {
			XmlRequest xmlRequest = new XmlRequest();
			xmlRequest.FirstTag = "get";
			xmlRequest.BodyText = path;
			xmlRequest.Attributes.Add("what", "folder");
			peer.Send(xmlRequest.GenerateXml());
		}

		public static void RequestFile (PeerSocket peer, string path) {
			XmlRequest xmlRequest = new XmlRequest();
			xmlRequest.FirstTag = "get";
			xmlRequest.BodyText = path;
			xmlRequest.Attributes.Add("what", "file");
			peer.Send(xmlRequest.GenerateXml());
		}

		public static void AskSendFile (PeerSocket peer, string path) {
			FileInfo fileInfo = new FileInfo(path);

			XmlRequest xmlRequest = new XmlRequest();
			xmlRequest.FirstTag = "ask";
			xmlRequest.Attributes.Add("what", "file");
			xmlRequest.Attributes.Add("name", fileInfo.Name);
			xmlRequest.Attributes.Add("size", fileInfo.Length);

			peer.Send(xmlRequest.GenerateXml());
		}

		// ============================================
		// PUBLIC STATIC Methods (Events)
		// ============================================
		public static void StartLoginEvent (PeerSocket peer, UserInfo info) {
			if (LoginEvent != null) LoginEvent(peer, info);
		}

		public static void StartQuitEvent (PeerSocket peer, XmlRequest xml) {
			if (QuitEvent != null) QuitEvent(peer, xml);
		}

		public static void StartErrorEvent (PeerSocket peer, XmlRequest xml) {
			if (ErrorEvent != null) ErrorEvent(peer, xml);
		}

		public static void StartGetEvent (PeerSocket peer, XmlRequest xml) {
			if (GetEvent != null) GetEvent(peer, xml);
		}

		public static void StartAskEvent (PeerSocket peer, XmlRequest xml) {
			if (AskEvent != null) AskEvent(peer, xml);
		}

		public static void StartAcceptEvent (PeerSocket peer, XmlRequest xml) {
			if (AcceptEvent != null) AcceptEvent(peer, xml);
		}

		public static void StartSndEvent (PeerSocket peer, XmlRequest xml) {
			if (SndEvent != null) SndEvent(peer, xml);
		}

		public static void StartSndStartEvent (PeerSocket peer, XmlRequest xml) {
			if (SndStartEvent != null) SndStartEvent(peer, xml);
		}

		public static void StartSndEndEvent (PeerSocket peer, XmlRequest xml) {
			if (SndEndEvent != null) SndEndEvent(peer, xml);
		}

		public static void StartUnknownEvent (PeerSocket peer, XmlRequest xml) {
			if (UnknownEvent != null) UnknownEvent(peer, xml);
		}

		// ============================================
		// PRIVATE (Methods) Event Handlers
		// ============================================
		private void OnReceived (object sender, PeerEventArgs args) {
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
				while ((splitPos = xml.IndexOf("><", splitPos)) > -1) {
					string cmd = xml.Substring(0, splitPos + 1);
					xml = xml.Remove(0, splitPos + 1);
					xmlCmds.Add(cmd);
				}

				if (XmlRequest.IsEndedXml(xml) == false) {
					peer.Response.Insert(0, xml);
				} else {
					xmlCmds.Add(xml);
				}
			}

			// Start New Command Parse Thread
			new CmdParse(peer, xmlCmds);
		}

		// ============================================
		// PUBLIC Properties
		// ============================================
	}
}
