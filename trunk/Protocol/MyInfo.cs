/* [ Protocol/MyInfo.cs ] NyFolder Protocol (MyInfo)
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
	public delegate void LoginEventHandler (UserInfo info, bool status, string msg);

	public static class MyInfo {
		// ============================================
		// PUBLIC Events
		// ============================================
		public static event LoginEventHandler LoginChecked = null;

		// ============================================
		// PRIVATE (Singleton) Members
		// ============================================
		private static UserInfo myInfo = null;
	
		// ============================================
		// PUBLIC STATIC Methods
		// ============================================
		public static void Initialize (string username) {
			myInfo = new UserInfo(username);
		}

		public static void Initialize (string username, bool secureAuth) {
			myInfo = new UserInfo(username, secureAuth);
		}

		public static UserInfo GetInstance() {
			return(myInfo);
		}

		// ============================================
		// PUBLIC Methods (HttpRequest Required)
		// ============================================
		public static void Login (string password) {
			if (myInfo == null)
				throw(new UserInfoException("Login Failed, MyInfo is not Initialized"));
		}

		public static void Logout() {
			if (myInfo == null)
				throw(new UserInfoException("Logout Failed, MyInfo is not Initialized"));
		}

		// ============================================
		// PUBLIC Methods
		// ============================================

		// ============================================
		// PRIVATE Methods
		// ============================================

		// ============================================
		// PUBLIC Properties
		// ============================================
		public static string Name {
			get { return(myInfo.Name); }
		}
	}
}
