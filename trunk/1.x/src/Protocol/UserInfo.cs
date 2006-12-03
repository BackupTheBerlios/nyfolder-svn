/* [ Protocol/UserInfo.cs ] NyFolder Protocol (UserInfo)
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

namespace NyFolder.Protocol {
	public class UserInfoException : Exception {
		public UserInfoException (string msg) : base(msg) {}
		public UserInfoException (string msg, Exception inner) : base(msg, inner) {}
	}

	/// Rapresent User (Informations)
	public class UserInfo : IComparable {
		// ============================================
		// PROTECTED Members
		// ============================================
		protected Hashtable informations = null;
		protected bool secureAuth;
		protected string name;
		protected string ip;
		protected int port;
	
		// ============================================
		// PUBLIC Constructors
		// ============================================
		/// Create New User Info
		public UserInfo (string username) {
			this.informations = Hashtable.Synchronized(new Hashtable());
			this.secureAuth = false;
			this.name = username;
			this.port = 7085;
		}

		/// Create New User Info
		public UserInfo (string username, bool secureAuth) {
			this.informations = Hashtable.Synchronized(new Hashtable());
			this.secureAuth = secureAuth;
			this.name = username;
			this.port = 7085;
		}

		/// Create New User Info
		public UserInfo (string username, bool secureAuth, string magic) {
			this.informations = Hashtable.Synchronized(new Hashtable());
			this.secureAuth = secureAuth;
			this.name = username;
			this.port = 7085;
			this.informations.Add("magic", magic);
		}

		// ============================================
		// PUBLIC Methods (HttpRequest Required)
		// ============================================
		/// Fetch Ip and Port from Web Server (Require Secure Auth)
		public void GetIpAndPort() {
			if (secureAuth == true) {
				this.ip = HttpRequest.Ip(this);
				this.port = HttpRequest.Port(this);
			}
		}

		/// Set User Ip and Port
		public void SetIpAndPort (string ip, int port) {
			this.ip = ip;
			this.port = port;
		}

		// ============================================
		// PUBLIC Methods
		// ============================================
		/// Get User Name without Domain (name@domain)
		public string GetName() {
			// username@domain
			int domainStart = name.LastIndexOf('@');
			if (domainStart < 0) return(name);
			return(name.Substring(0, domainStart));
		}

		/// Get User Domain (name@domain)
		public string GetDomain() {
			// username@domain
			int domainStart = name.LastIndexOf('@');
			if (domainStart < 0) return(null);
			return(name.Substring(domainStart + 1));
		}

		/// Two User are Equals if Secure Auth & Name are Equals
		public int CompareTo (object userInfo) {
			return(CompareTo((UserInfo) userInfo));
		}

		/// Two User are Equals if Secure Auth & Name are Equals
		public int CompareTo (UserInfo userInfo) {
			int cmp = Name.CompareTo(userInfo.Name);

			// if usernames are equals but not secure auth return 1
			if (userInfo.SecureAuthentication != SecureAuthentication)
				return((cmp == 0) ? 1 : cmp);

			return(cmp);
		}

		// ============================================
		// PRIVATE Methods
		// ============================================

		// ============================================
		// PUBLIC Properties
		// ============================================
		/// Get Informations Hashtable
		public Hashtable Informations {
			get { return(this.informations); }
		}
		
		/// Get or Set User Username
		public string Name {
			get { return(this.name); }
			set { this.name = value; }
		}

		/// Get or Set User IP
		public string Ip {
			get { return(this.ip); }
			set { this.ip = value; }
		}

		/// Get or Set User Port
		public int Port {
			get { return(this.port); }
			set { this.port = value; }
		}

		/// Get or Set if User uses Secure Auth
		public bool SecureAuthentication {
			get { return(this.secureAuth); }
			set { this.secureAuth = value; }
		}

		/// User Is Online ?
		public bool IsOnline {
			get {
				if (P2PManager.KnownPeers == null) return(false);
				return(P2PManager.KnownPeers.ContainsKey(this));
			}
		}
	}
}
