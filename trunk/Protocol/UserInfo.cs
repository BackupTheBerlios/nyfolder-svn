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

namespace NyFolder.Protocol {
	public class UserInfoException : Exception {
		public UserInfoException (string msg) : base(msg) {}
		public UserInfoException (string msg, Exception inner) : base(msg, inner) {}
	}

	public class UserInfo {
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
		public UserInfo (string username) {
			this.informations = Hashtable.Synchronized(new Hashtable());
			this.secureAuth = false;
			this.name = username;
			this.port = 7085;
		}

		public UserInfo (string username, bool secureAuth) {
			this.informations = Hashtable.Synchronized(new Hashtable());
			this.secureAuth = secureAuth;
			this.name = username;
			this.port = 7085;
		}

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
		public void GetIpAndPort() {
			this.ip = null;
			this.port = 7085;
		}

		public void SetIpAndPort (string ip, int port) {
			this.ip = ip;
			this.port = port;
		}

		// ============================================
		// PUBLIC Methods
		// ============================================
		public string GetName() {
			// username@domain
			int domainStart = name.LastIndexOf('@');
			if (domainStart < 0) return(name);
			return(name.Substring(0, domainStart));
		}

		public string GetDomain() {
			// username@domain
			int domainStart = name.LastIndexOf('@');
			if (domainStart < 0) return(null);
			return(name.Substring(domainStart + 1));
		}

		// ============================================
		// PRIVATE Methods
		// ============================================

		// ============================================
		// PUBLIC Properties
		// ============================================
		public Hashtable Informations {
			get { return(this.informations); }
		}
		
		public string Name {
			get { return(this.name); }
			set { this.name = value; }
		}

		public string Ip {
			get { return(this.ip); }
			set { this.ip = value; }
		}

		public int Port {
			get { return(this.port); }
			set { this.port = value; }
		}

		public bool SecureAuthentication {
			get { return(this.secureAuth); }
			set { this.secureAuth = value; }
		}
	}
}
