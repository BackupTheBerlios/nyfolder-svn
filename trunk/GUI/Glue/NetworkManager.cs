/* [ GUI/Glue/NetworkManager.cs ] NyFolder (Network Manager)
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
	public delegate void SetProtocolEventHandler (P2PManager p2p, CmdManager cmd);

	public class NetworkManager {
		public static event SetProtocolEventHandler AddProtocolEvent = null;
		public static event SetProtocolEventHandler DelProtocolEvent = null;
		// ============================================
		// PRIVATE Members
		// ============================================
		private NotebookViewer notebookViewer;
		private MenuManager menuManager;
		private UserPanel userPanel;

		private P2PManager p2pManager;
		private CmdManager cmdManager;

		// ============================================
		// PUBLIC Constructors
		// ============================================
		public NetworkManager (MenuManager menu, UserPanel userPanel, NotebookViewer nv) {
			this.userPanel = userPanel;
			this.notebookViewer = nv;
			this.menuManager = menu;

			// Initialize P2PManager & CMD Manager
			this.p2pManager = P2PManager.GetInstance();
			this.cmdManager = CmdManager.GetInstance();

			// Network
			SetSensitiveNetworkMenu(P2PManager.IsListening());

			// Add Event Handlers
			this.menuManager.Activated += new EventHandler(OnMenuActivated);
			notebookViewer.NetworkViewer.ItemRemoved += new PeerSelectedHandler(OnPeerRemove);
		}

		// ============================================
		// PRIVATE Methods
		// ============================================
		protected void AddProtocolEvents() {
			CmdManager.LoginEvent += new ProtocolLoginHandler(OnPeerLogin);
			P2PManager.PeerError += new PeerEventHandler(OnPeerError);
			P2PManager.PeerDisconnecting += new PeerEventHandler(OnPeerDisconnect);
		}

		protected void DelProtocolEvents() {
			CmdManager.LoginEvent -= new ProtocolLoginHandler(OnPeerLogin);
			P2PManager.PeerError -= new PeerEventHandler(OnPeerError);
			P2PManager.PeerDisconnecting -= new PeerEventHandler(OnPeerDisconnect);
		}

		// ============================================
		// PRIVATE Methods
		// ============================================
		private void Connect (UserInfo userInfo) {
			try {
				// Connect & Send Login
				P2PManager.AddPeer(userInfo, userInfo.Ip, userInfo.Port);
				PeerSocket peer = (PeerSocket) P2PManager.KnownPeers[userInfo];
				CmdManager.Login(peer, MyInfo.GetInstance());
				OnPeerLogin(peer, userInfo);
			} catch (Exception e) {
				Glue.Dialogs.MessageErrorDialog("Connenting To " + userInfo.Name + " Failed",  e.Message);
			}
		}

		private void AddUser (UserInfo userInfo) {
			notebookViewer.NetworkViewer.Add(userInfo);
		}

		private void RemoveUser (UserInfo userInfo) {
			notebookViewer.Remove(userInfo);
			notebookViewer.NetworkViewer.Remove(userInfo);
		}

		private void RemoveAllUsers() {
			notebookViewer.RemoveAll();
			notebookViewer.NetworkViewer.RemoveAll();
		}

		private bool AcceptUser (PeerSocket peer) {
			GUI.Dialogs.AcceptUser dialog = new GUI.Dialogs.AcceptUser(peer);
			ResponseType response = dialog.Run();
			dialog.Destroy();
			return(response == ResponseType.Ok);
		}

		private void SetSensitiveNetworkMenu (bool sensitive) {
			menuManager.SetSensitive("/MenuBar/NetworkMenu/DownloadManager", sensitive);
			menuManager.SetSensitive("/MenuBar/NetworkMenu/AddPeer", sensitive);
			menuManager.SetSensitive("/MenuBar/NetworkMenu/RmPeer", sensitive);
		}

		// ============================================
		// PRIVATE (Methods) Event Handler
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
					case "DownloadManager":
						break;
					case "AddPeer":
						UserInfo userInfo = Glue.Dialogs.AddPeer();
						if (userInfo != null) Connect(userInfo);
						break;
					case "RmPeer":
						userInfo = Glue.Dialogs.RemovePeer(notebookViewer.NetworkViewer);
						if (userInfo != null) RemoveUser(userInfo);
						break;
				}
			});
		}

		private void OnNetOnline (ToggleAction action) {
			// Set UserPanel Online/Offline Status
			SetSensitiveNetworkMenu(action.Active);
			userPanel.SetOnlineStatus(action.Active);

			if (action.Active == true) {
				this.p2pManager.StartListening();
				UploadManager.Initialize();
				this.cmdManager.AddPeerEventsHandler();
				this.AddProtocolEvents();

				if (AddProtocolEvent != null) AddProtocolEvent(p2pManager, cmdManager);
			} else {
				if (DelProtocolEvent != null) DelProtocolEvent(p2pManager, cmdManager);

				this.RemoveAllUsers();
				this.DelProtocolEvents();
				this.cmdManager.DelPeerEventsHandler();
				UploadManager.Clear();
				this.p2pManager.StopListening();
			}
		}

		private void OnPeerLogin (PeerSocket peer, UserInfo userInfo) {
			Gtk.Application.Invoke(delegate {
				bool acceptUser = false;
				if (userInfo.SecureAuthentication == true) {
					// Check if User is Present into Db else Ask Accept
//					if (Database.User.IsPresent(userInfo.Name) == false)
						acceptUser = AcceptUser(peer);

					// Add User To DB (Ask ?)
//					if (acceptUser == true)
//						Database.User.Add(userInfo.Name);
				} else {
					acceptUser = AcceptUser(peer);
				}

				// Accept Peer (Add to NetworkViewer) or Remove Peer (P2PManager)
				if (acceptUser == true) {
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
#if false
			Gtk.Application.Invoke(delegate {
				Glue.Dialogs.MessageErrorDialog(userInfo.Name + " Error", args.Message);
			});
			Gtk.Application.Quit();
			Environment.Exit(0);
#endif
		}

		public void OnPeerRemove (object sender, UserInfo userInfo) {
			Gtk.Application.Invoke(delegate { RemoveUser(userInfo); });
		}
	}
}
