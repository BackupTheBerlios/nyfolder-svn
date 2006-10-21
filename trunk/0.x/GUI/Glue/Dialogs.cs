/* [ GUI/Glue/Dialogs.cs ] NyFolder (Dialogs Glue)
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
using System.IO;
using System.Text;

using Niry;
using Niry.Utils;
using Niry.Network;

using NyFolder;
using NyFolder.GUI;
using NyFolder.Utils;
using NyFolder.Protocol;

namespace NyFolder.GUI.Glue {
	/// Utility Dialogs
	public static class Dialogs {
		/// Simple Message Error Dialog
		public static void MessageError (string title, string message) {
			MessageDialog dialog;

			dialog = new MessageDialog (null, DialogFlags.Modal, MessageType.Error,
										ButtonsType.Close, true, 
										"<span size='x-large'><b>{0}</b></span>\n\n{1}",
										title, message);
			dialog.Run();
			dialog.Destroy();
		}

		/// Simple Yes/No Question Dialog
		public static bool QuestionDialog (string title, string message) {
			MessageDialog dialog;
			dialog = new MessageDialog(null, DialogFlags.Modal, MessageType.Question, 
										ButtonsType.YesNo, true, 
										"<span size='x-large'><b>{0}</b></span>\n\n{1}",
										title, message);
			bool response = (ResponseType) dialog.Run() == ResponseType.Yes;
			dialog.Destroy();
			return(response);
		}

		/// Save File Dialog
		public static string SaveFile (string path, string fileName) {
			FileChooserDialog dialog = new FileChooserDialog("Save File", null,
															 FileChooserAction.Save);
			dialog.AddButton(Stock.Cancel, ResponseType.Cancel);
			dialog.AddButton(Stock.Save, ResponseType.Ok);

			dialog.SelectMultiple = false;
			dialog.SetCurrentFolder(path);
			dialog.CurrentName = fileName;

			string filePath = null;
			do {
				if ((ResponseType) dialog.Run() != ResponseType.Ok) {
					filePath = null;
					break;
				}

				if (File.Exists(dialog.Filename) == false) {
					filePath = dialog.Filename;
					break;
				}

				if (ReplaceExistingFile(dialog.Filename) == true) {
					filePath = dialog.Filename;
					break;
				}
			} while (true);
			dialog.Destroy();
			return(filePath);
		}

		/// Replace Existing File Dialog
		public static bool ReplaceExistingFile (string fileName) {
			FileInfo fileInfo = new FileInfo(fileName);

			StringBuilder msg = new StringBuilder();
			msg.AppendFormat("A file named \"{0}\" already exists.\n", fileInfo.Name);
			msg.Append("Do you want to replace it ?\n\n");
			msg.AppendFormat("The file already exists in \"{0}\".\n", fileInfo.DirectoryName);
			msg.Append("Replacing it will overwrite its contents.");

			return(QuestionDialog("Replace File", msg.ToString()));
		}


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
					MessageError("Proxy Settings", e.Message);
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
					MessageError("Fetch " + userInfo.Name + " Information", e.Message);
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
	}
}
