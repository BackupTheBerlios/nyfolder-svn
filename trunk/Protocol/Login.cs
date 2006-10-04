/* [ Protocol/Login.cs ] NyFolder Protocol (Login)
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

namespace NyFolder.Protocol {
	/// Login Checker
	public class Login {
		// ============================================
		// PROTECTED Members
		// ============================================
		protected UserInfo userInfo;

		// ============================================
		// PUBLIC Constructors
		// ============================================
		/// Create New Login Checker
		public Login (UserInfo userInfo) {
			this.userInfo = userInfo;
		}

		/// Create New Login Checker
		public Login (PeerSocket peer, XmlRequest xml) {
			// Get UserName
			string userName = (string) xml.Attributes["name"];
			if (userName == null) return;

			// Get SecureAuth
			bool secureAuth = false;
			string _secureAuth = (string) xml.Attributes["secure"];
			if (_secureAuth != null && _secureAuth == "True") secureAuth = true;

			// Get Magic
			string magic = (string) xml.Attributes["magic"];
			if (secureAuth == true && magic == null) return;

			// Initialize UserInfo
			this.userInfo = new UserInfo(userName, secureAuth, magic);
		}

		// ============================================
		// PUBLIC Methods
		// ============================================
		/// Check Secure Login
		public bool CheckSecureLogin (string password) {
			if (password == null) return(false);
			return(HttpRequest.Login(userInfo, password));
		}

		/// Authenticate User
		public bool Authentication() {
			if (userInfo.SecureAuthentication == false)
				return(true);

			if (MyInfo.GetInstance().SecureAuthentication == false)
				return(true);

			string magic = (string) userInfo.Informations["magic"];
			if (magic == null) return(false);
			return(HttpRequest.Authentication(userInfo));
		}

		// ============================================
		// PUBLIC STATIC Methods
		// ============================================
		public static string GenerateMagic (PeerSocket peer) {
			UserInfo myInfo = MyInfo.GetInstance();
			string userIp = CryptoUtils.SHA1String(peer.GetRemoteIP().ToString());
			string userMagic = CryptoUtils.SHA1String((string) myInfo.Informations["magic"]);
			return(CryptoUtils.MD5String(userIp + userMagic));
		}

		// ============================================
		// PUBLIC Properties
		// ============================================
		/// User
		public UserInfo User {
			get { return(this.userInfo); }
		}
	}
}
