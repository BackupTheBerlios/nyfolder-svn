/* [ Protocol/Cmd.cs ] NyFolder Protocol Commands
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
using System.Text;

using Niry;
using Niry.Utils;
using Niry.Network;

using NyFolder;
using NyFolder.Utils;
using NyFolder.Protocol;

namespace NyFolder.Protocol {
	public static class Cmd {
		// ============================================
		// PUBLIC Events
		// ============================================

		// ============================================
		// PUBLIC Methods
		// ============================================
		/// Send Login
		public static void Login (PeerSocket peer, UserInfo userInfo) {
			XmlRequest xmlRequest = new XmlRequest();
			xmlRequest.FirstTag = "login";
			xmlRequest.Attributes.Add("name", userInfo.Name);
			xmlRequest.Attributes.Add("secure", userInfo.SecureAuthentication.ToString());

			string magic = Protocol.Login.GenerateMagic(peer);
			xmlRequest.Attributes.Add("magic", magic);

			peer.Send(xmlRequest.GenerateXml());
		}

		/// Send Error
		public static void Error (PeerSocket peer, string message) {
			XmlRequest xmlRequest = new XmlRequest();
			xmlRequest.FirstTag = "error";
			xmlRequest.BodyText = message;
			peer.Send(xmlRequest.GenerateXml());
		}

		/// Send Error
		public static void Error (PeerSocket peer, string f, params object[] objs) {
			StringBuilder message = new StringBuilder();
			message.AppendFormat(f, objs);
			Error(peer, message.ToString());
		}

		/// Request User's Folder
		public static void RequestFolder (PeerSocket peer, string path) {
			XmlRequest xmlRequest = new XmlRequest();
			xmlRequest.FirstTag = "get";
			xmlRequest.BodyText = path;
			xmlRequest.Attributes.Add("what", "folder");
			peer.Send(xmlRequest.GenerateXml());
		}

		/// Request File By Path
		public static void RequestFile (PeerSocket peer, string path) {
			XmlRequest xmlRequest = new XmlRequest();
			xmlRequest.FirstTag = "get";
			xmlRequest.Attributes.Add("what", "file");
			xmlRequest.Attributes.Add("path", path);
			peer.Send(xmlRequest.GenerateXml());
		}

		/// Request File By ID
		public static void RequestFile (PeerSocket peer, uint id) {
			XmlRequest xmlRequest = new XmlRequest();
			xmlRequest.FirstTag = "get";
			xmlRequest.Attributes.Add("what", "file-id");
			xmlRequest.Attributes.Add("id", id);
			peer.Send(xmlRequest.GenerateXml());
		}

		// ============================================
		// PRIVATE Methods
		// ============================================
	}
}
