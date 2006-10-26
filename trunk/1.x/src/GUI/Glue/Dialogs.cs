/* [ GUI/Glue/Dialogs.cs ] NyFolder GUI Glue, Dialogs
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
using NyFolder.Utils;
using NyFolder.Protocol;
using NyFolder.GUI.Base;

namespace NyFolder.GUI.Glue {
	/// GUI Glue Dialogs
	public static class Dialogs {
		// ============================================
		// PUBLIC Methods
		// ============================================
		/// Run Proxy Settings Dialog & Save Configuration
		public static void ProxySettings() {
			GUI.Dialogs.ProxySettings dialog = new GUI.Dialogs.ProxySettings();

			// Setup Proxy
			dialog.UseProxyAuth = Proxy.UseProxyAuth;
			dialog.EnableProxy = Proxy.EnableProxy;
			dialog.Username = Proxy.Username;
			dialog.Password = Proxy.Password;
			dialog.Host = Proxy.Host;
			dialog.Port = Proxy.Port;

			// Run Dialogs
			if (dialog.Run() == ResponseType.Apply) {
				try {
					Proxy.UseProxyAuth = dialog.UseProxyAuth;
					Proxy.EnableProxy = dialog.EnableProxy;
					Proxy.Username = dialog.Username;
					Proxy.Password = dialog.Password;
					Proxy.Host = dialog.Host;
					Proxy.Port = dialog.Port;

					// Save Proxy
					Proxy.Save();
				} catch (Exception e) {
					GUI.Base.Dialogs.MessageError("Proxy Settings", e.Message);
				}
			}
			dialog.Destroy();
		}

		/// Run P2P SetPort & Save Configuration
		public static void SetP2PPort() {
			GUI.Dialogs.SetPort dialog = new GUI.Dialogs.SetPort();

			if (dialog.Run() == ResponseType.Ok) {
				P2PManager.Port = dialog.Port;
			}
			dialog.Destroy();
		}

		/// Run Add Peer Dialog
		public static UserInfo AddPeer() {
			GUI.Dialogs.AddPeer dialog = new GUI.Dialogs.AddPeer();
			ResponseType response;
			string username;
			do {
				response = dialog.Run();
				username = dialog.Username;
			} while (response == ResponseType.Ok && username == null);
			
			bool secureAuth = dialog.SecureAuthentication;
			string ip = dialog.Ip;
			int port = dialog.Port;
			dialog.Destroy();

			if (response == ResponseType.Ok && username != null) {
				UserInfo userInfo = new UserInfo(username, secureAuth);
				userInfo.SetIpAndPort(ip, port);

				try {
					// Get User Ip & Port from Server
					if (userInfo.SecureAuthentication == true) {
						userInfo.GetIpAndPort();
					}
					return(userInfo);
				} catch (Exception e) {
					string title = "Fetch <b>" + userInfo.Name + "</b> Information";
					GUI.Base.Dialogs.MessageError(title, e.Message);
					return(null);
				}
			}
			return(null);
		}

		/// Run Remove Peer Dialog
		public static UserInfo RemovePeer (NetworkViewer networkViewer) {
			GUI.Dialogs.RemovePeer dialog = new GUI.Dialogs.RemovePeer();
			ResponseType response = dialog.Run();
			string peerName = dialog.GetPeerSelected();
			dialog.Destroy();

			if (response == ResponseType.Ok && peerName != null) {
				return(networkViewer.GetUserInfo(peerName));
			}
			return(null);
		}

		// ============================================
		// PRIVATE Methods
		// ============================================
	}
}
