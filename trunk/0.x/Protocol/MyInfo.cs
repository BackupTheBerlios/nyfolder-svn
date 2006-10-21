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

using Niry;
using Niry.Utils;

using NyFolder;
using NyFolder.Utils;

namespace NyFolder.Protocol {
	public delegate void LoginEventHandler (UserInfo info, bool status, string msg);

	/// Rapresent Current Logged in User
	public static class MyInfo {
		// ============================================
		// PUBLIC Events
		// ============================================
		public static event LoginEventHandler LoginChecked = null;

		// ============================================
		// PRIVATE (Singleton) Members
		// ============================================
		private static Timer timerWebStatusUpdate = null;
		private static UserInfo myInfo = null;
	
		// ============================================
		// PUBLIC STATIC Methods
		// ============================================
		public static void Initialize (string username) {
			myInfo = new UserInfo(username);
			myInfo.Informations.Add("magic", "None");
		}

		public static void Initialize (string username, bool secureAuth) {
			myInfo = new UserInfo(username, secureAuth);
			if (secureAuth == false) myInfo.Informations.Add("magic", "None");
		}

		public static UserInfo GetInstance() {
			return(myInfo);
		}

		// ============================================
		// PUBLIC Methods (HttpRequest Required)
		// ============================================
		/// Do Login on Web Server (Require Secure Auth)
		public static void Login (string password) {
			if (myInfo == null) {
				string message = "Login Failed, MyInfo is not Initialized";
				if (LoginChecked != null) LoginChecked(myInfo, false, message);
				return;
			}

			// If it's Insecure Login, it's always OK
			if (myInfo.SecureAuthentication == false) {
				string message = "Login Ok, Insecure Authentication";
				if (LoginChecked != null) LoginChecked(myInfo, true, message);
				return;
			}

			// Get Domain
			string domain = myInfo.GetDomain();
			if (domain == null) {
				string message = "Domain Not Found, Check your UserName";
				if (LoginChecked != null) LoginChecked(myInfo, false, message);
				return;
			}

			myInfo.Informations.Add("Password", password);
			Thread thread = new Thread(new ThreadStart(CheckSecureLogin));
			thread.Start();			
		}

		/// Do Logout on Web Server (Require Secure Auth)
		public static void Logout() {
			if (myInfo == null) return;

			if (timerWebStatusUpdate != null) {
				timerWebStatusUpdate.Dispose();
				timerWebStatusUpdate = null;
			}

			// If it's Insecure Login, No Logout
			if (myInfo.SecureAuthentication == false)
				return;

			// Do Logout
			try {
				HttpRequest.Logout(myInfo);
			} catch (Exception e) {
				Debug.Log("Logout Failed For {0}", myInfo.Name);
				Debug.Log(e.Message);
			}
		}

		/// Connect to Web Server (Require Secure Auth)
		public static void ConnectToWebServer (int port) {
			if (myInfo == null) return;

			// If it's Insecure Login, Don't Connect To Web Server
			if (myInfo.SecureAuthentication == false)
				return;

			// Connect To Web Server
			HttpRequest.Connect(myInfo, port);
		}

		/// Disconnect to Web Server (Require Secure Auth)
		public static void DisconnectFromWebServer() {
			if (myInfo == null) return;

			// If it's Insecure Login, You're Not Connected To Web Server
			if (myInfo.SecureAuthentication == false)
				return;

			// Disconnect From Web Server
			HttpRequest.Disconnect(myInfo);
		}

		// ============================================
		// PUBLIC Methods
		// ============================================

		// ============================================
		// PRIVATE Methods
		// ============================================
		private static void CheckSecureLogin() {
			string message = "Login Failed";
			bool status = false;

			// Get And Remove Password From UserInfo
			string password = (string) myInfo.Informations["Password"];
			myInfo.Informations.Remove("Password");

			try {
				Login login = new Login(myInfo);

				if ((status = login.CheckSecureLogin(password)) == true)
					message = "Login Ok";

				// Start Update Web Status Timer ~5min (Less :D)
				if (timerWebStatusUpdate != null) timerWebStatusUpdate = null;
				timerWebStatusUpdate = new Timer(new TimerCallback(UpdateWebStatus), null, 0, 250000);
			} catch (Exception e) {
				message = e.Message;
				status = false;
			}

			// Send Login Checked Event
			if (LoginChecked != null) LoginChecked(myInfo, status, message);
		}

		// Web Server Time is 5 Minutes (300 sec, 300000 msec)
		private static void UpdateWebStatus (object obj) {
			try {
				HttpRequest.Update(myInfo);
			} catch (Exception e) {
				Debug.Log("Web Status Update Failed: {0}", e.Message);
			}
		}

		// ============================================
		// PUBLIC Properties
		// ============================================
		/// Get Username
		public static string Name {
			get { return(myInfo.Name); }
		}
	}
}
