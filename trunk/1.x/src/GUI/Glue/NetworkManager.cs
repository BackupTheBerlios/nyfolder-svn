/* [ GUI/Glue/NetworkManager.cs ] NyFolder GUI Network Manager Glue
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
	public sealed class NetworkManager {
		// ============================================
		// PRIVATE Members
		// ============================================
		private NotebookViewer notebookViewer;
		private NetworkViewer networkViewer;
		private MenuManager menuManager;
		private UserPanel userPanel;

		private P2PManager p2pManager;

		// ============================================
		// PUBLIC Constructors
		// ============================================
		public NetworkManager (GUI.Window window) {
			// Initialize Components
			this.menuManager = window.Menu;
			this.userPanel = window.UserPanel;
			this.notebookViewer = window.NotebookViewer;
			this.networkViewer = this.notebookViewer.NetworkViewer;

			// Initialize P2PManager & CMD Manager
			this.p2pManager = P2PManager.GetInstance();

			// Network
			SetSensitiveNetworkMenu(P2PManager.IsListening());

			// Add Event Handlers
			menuManager.Activated += new EventHandler(OnMenuActivated);
			networkViewer.ItemRemoved += new PeerSelectedHandler(OnPeerRemove);
			CmdManager.AddProtocolEvent += new SetProtocolEventHandler(OnAddProtocolEvent);
			CmdManager.DelProtocolEvent += new SetProtocolEventHandler(OnDelProtocolEvent);
		}

		// ============================================
		// PUBLIC Methods
		// ============================================

		// ============================================
		// PRIVATE (Methods) Menu Event Handlers
		// ============================================
		private void OnMenuActivated (object sender, EventArgs args) {
			Gtk.Application.Invoke(delegate {
				Action action = sender as Action;
				switch (action.Name) {
					// Network
					case "NetOnline":
						OnNetOnline((ToggleAction) action);
						break;
					case "SetPort":
						Dialogs.SetP2PPort();
						break;
					case "AddPeer":
						UserInfo userInfo = Glue.Dialogs.AddPeer();
						if (userInfo != null) UserConnect(userInfo);
						break;
					case "RmPeer":
						userInfo = Glue.Dialogs.RemovePeer(networkViewer);
						if (userInfo != null) RemoveUser(userInfo);
						break;
				}
			});
		}

		// ============================================
		// PRIVATE (Methods) Event Handlers
		// ============================================
		private void OnNetOnline (ToggleAction action) {
			// Set UserPanel Online/Offline Status
			SetSensitiveNetworkMenu(action.Active);
			userPanel.SetOnlineStatusIcon(action.Active);

			if (action.Active == true) {
				try {
					P2PStartListening();
					ConnectToWebServer();
				} catch {
					action.Active = false;
				}				
			} else {
				try {
					DisconnectFromWebServer();
					this.RemoveAllUsers();
					this.p2pManager.StopListening();
				} catch (Exception e) {
					Base.Dialogs.MessageError("P2P Disconnection Error", e.Message);
				}
			}
		}

		private void OnPeerLogin (PeerSocket peer, UserInfo userInfo) {
			Gtk.Application.Invoke(delegate {
				// Accept Peer (Add to NetworkViewer) or Remove Peer (P2PManager)
				if (AcceptUser(peer) == true) {
					AddUser(userInfo);
				} else {
					P2PManager.RemovePeer(peer);
				}
			});
		}

		private void OnPeerDisconnect (object sender, PeerEventArgs args) {
			PeerSocket peer = sender as PeerSocket;
			Gtk.Application.Invoke(delegate {
				if (peer.Info != null) RemoveUser((UserInfo) peer.Info);
			});
		}

		private void OnPeerError (object sender, PeerEventArgs args) {
			PeerSocket peer = sender as PeerSocket;
			UserInfo userInfo = peer.Info as UserInfo;
			Debug.Log("Peer ({0}) Error: {1}", userInfo.Name, args.Message);
		}

		public void OnPeerRemove (object sender, UserInfo userInfo) {
			Gtk.Application.Invoke(delegate { RemoveUser(userInfo); });
		}

		// =============================================
		// PRIVATE Methods (My Connection/Disconnection)
		// =============================================
		private void OnAddProtocolEvent (P2PManager p2pManager) {
			P2PManager.PeerError += new PeerEventHandler(OnPeerError);
			CmdManager.LoginEvent += new ProtocolLoginHandler(OnPeerLogin);
			P2PManager.PeerDisconnecting += new PeerEventHandler(OnPeerDisconnect);
		}

		private void OnDelProtocolEvent (P2PManager p2pManager) {
			P2PManager.PeerError -= new PeerEventHandler(OnPeerError);
			CmdManager.LoginEvent -= new ProtocolLoginHandler(OnPeerLogin);
			P2PManager.PeerDisconnecting -= new PeerEventHandler(OnPeerDisconnect);
		}

		private void P2PStartListening() {
			try {
				// P2P Start Listening
				this.p2pManager.StartListening();
			} catch (Exception e) {
				Base.Dialogs.MessageError("P2P Starting Error", e.Message);
				// ReTrow Exception
				throw(e);
			}
		}

		private void ConnectToWebServer() {			
			try {
				// Connect To NyFolder Web Server
				MyInfo.ConnectToWebServer(this.p2pManager.CurrentPort);
			} catch (Exception e) {
				Base.Dialogs.MessageError("Connect To Web Server", e.Message);

				// P2P Stop Listening
				this.p2pManager.StopListening();

				// ReTrow Exception
				throw(e);
			}
		}

		private void DisconnectFromWebServer() {			
			try {
				// Disconnect From NyFolder Web Server
				MyInfo.DisconnectFromWebServer();
			} catch (Exception e) {
				Base.Dialogs.MessageError("Disconnection From Web Server", e.Message);
			}
		}

		// ============================================
		// PRIVATE Methods
		// ============================================
		private void SetSensitiveNetworkMenu (bool sensitive) {
			menuManager.SetSensitive("/MenuBar/NetworkMenu/DownloadManager", sensitive);
			menuManager.SetSensitive("/MenuBar/NetworkMenu/AddPeer", sensitive);
			menuManager.SetSensitive("/MenuBar/NetworkMenu/RmPeer", sensitive);
		}

		private void UserConnect (UserInfo userInfo) {
			try {
				// Connect & Send Login
				P2PManager.AddPeer(userInfo, userInfo.Ip, userInfo.Port);
				PeerSocket peer = (PeerSocket) P2PManager.KnownPeers[userInfo];
				CmdManager.Login(peer, MyInfo.GetInstance());
				OnPeerLogin(peer, userInfo);
			} catch (Exception e) {
				string title = "Connection To <b>" + userInfo.Name + "</b> Failed";
				Base.Dialogs.MessageError(title,  e.Message);
			}
		}

		private void AddUser (UserInfo userInfo) {
			networkViewer.Add(userInfo);
		}

		private void RemoveUser (UserInfo userInfo) {
			if (userInfo != null) {
				notebookViewer.Remove(userInfo);
				networkViewer.Remove(userInfo);
				P2PManager.RemovePeer(userInfo);
			}
		}

		private void RemoveAllUsers() {
			notebookViewer.RemoveAll();
			networkViewer.RemoveAll();
			P2PManager.RemoveAllPeer();
		}

		private bool AcceptUser (PeerSocket peer) {
			GUI.Dialogs.AcceptUser dialog = new GUI.Dialogs.AcceptUser(peer);
			ResponseType response = dialog.Run();
			dialog.Destroy();
			return(response == ResponseType.Ok);
		}
	}
}
