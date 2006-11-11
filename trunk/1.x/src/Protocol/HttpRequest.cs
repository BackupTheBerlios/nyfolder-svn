/* [ Protocol/HttpRequest.cs ] NyFolder Protocol (HttpRequest)
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
using System.IO;
using System.Xml;
using System.Net;
using System.Text;
using System.Collections;
using System.Net.Sockets;

using Niry;
using Niry.Utils;

using NyFolder;
using NyFolder.Utils;

namespace NyFolder.Protocol {
	/// Web Server Interaction
	public sealed class HttpRequest {
		// ============================================
		// PRIVATE STATIC Members
		// ============================================
		private static WebProxy proxy;

		// ============================================
		// PUBLIC Constructors
		// ============================================
		public HttpRequest() {
			proxy = null;
		}

		// ============================================
		// PUBLIC STATIC Methods
		// ============================================
		/// Get User IP
		public static string Ip (UserInfo userInfo) {
			// Make Url & Request
			string url = MakeUrl(userInfo, "GetIp.php", null);
			XmlRequest xml = MakeRequest(url);

			// Parse Xml Response
			if (xml.FirstTag == "ip")
				return(xml.BodyText);

			// Request Error
			throw(new Exception(xml.FirstTag + ": " + xml.BodyText));
		}

		/// Get User Port
		public static int Port (UserInfo userInfo) {
			// Make Url & Request
			string url = MakeUrl(userInfo, "GetPort.php", null);
			XmlRequest xml = MakeRequest(url);

			// Parse Xml Response
			if (xml.FirstTag == "port")
				return(Int32.Parse(xml.BodyText));

			// Request Error
			throw(new Exception(xml.FirstTag + ": " + xml.BodyText));
		}

		/// Check User Authentication
		public static bool Authentication (UserInfo userInfo) {
			// Set Options
			Hashtable options = new Hashtable();
			options.Add("magic", userInfo.Informations["magic"]);

			// Make Url & Request
			string url = MakeUrl(userInfo, "UserAuth.php", options);
			XmlRequest xml = MakeRequest(url);

			// Parse Xml Response
			if (xml.FirstTag == "authentication")
				return(true);

			// Request Error
			return(false);
		}

		/// Do Login and return IpWeb & Magic
		public static bool Login (UserInfo userInfo, string password) {
			// Set Options
			Hashtable options = new Hashtable();
			options.Add("passwd", password);

			// Make Url & Request
			string url = MakeUrl(userInfo, "Login.php", options);
			XmlRequest xml = MakeRequest(url);

			// Parse Xml Response
			if (xml.FirstTag == "login") {
				userInfo.Informations.Add("ipweb", xml.Attributes["ip"]);
				userInfo.Informations.Add("magic", xml.BodyText);
				return(true);
			}

			// Request Error
			throw(new Exception(xml.FirstTag + ": " + xml.BodyText));
		}

		/// Logout On Server
		public static void Logout (UserInfo userInfo) {
			// Set Options
			Hashtable options = new Hashtable();
			options.Add("magic", userInfo.Informations["magic"]);

			// Make Url & Request
			string url = MakeUrl(userInfo, "Logout.php", options);
			XmlRequest xml = MakeRequest(url);

			// Parse Xml Response
			if (xml.FirstTag != "logout") {
				// Request Error
				throw(new Exception(xml.FirstTag + ": " + xml.BodyText));
			}
		}

		/// Connect on Server
		public static void Connect (UserInfo userInfo, int port) {
			// Set Options
			Hashtable options = new Hashtable();
			options.Add("magic", userInfo.Informations["magic"]);
			options.Add("port", port.ToString());

			// Make Url & Request
			string url = MakeUrl(userInfo, "Connect.php", options);
			XmlRequest xml = MakeRequest(url);

			// Parse Xml Response
			if (xml.FirstTag != "connect") {
				// Request Error
				throw(new Exception(xml.FirstTag + ": " + xml.BodyText));
			}
		}

		/// Disconnect from Server
		public static void Disconnect (UserInfo userInfo) {
			// Set Options
			Hashtable options = new Hashtable();
			options.Add("magic", userInfo.Informations["magic"]);

			// Make Url & Request
			string url = MakeUrl(userInfo, "Disconnect.php", options);
			XmlRequest xml = MakeRequest(url);

			// Parse Xml Response
			if (xml.FirstTag != "disconnect") {
				// Request Error
				throw(new Exception(xml.FirstTag + ": " + xml.BodyText));
			}
		}

		/// Update My Server Status
		public static void Update (UserInfo userInfo) {
			// Set Options
			Hashtable options = new Hashtable();
			options.Add("magic", userInfo.Informations["magic"]);

			// Make Url & Request
			string url = MakeUrl(userInfo, "Update.php", options);
			XmlRequest xml = MakeRequest(url);

			// Parse Xml Response
			if (xml.FirstTag != "update") {
				// Request Error
				throw(new Exception(xml.FirstTag + ": " + xml.BodyText));
			}
		}

		/// Check if Username is Available
		public static string UserNameIsAvailable (string username) {
			// Set Options
			Hashtable options = new Hashtable();
			options.Add("user", username);
			options.Add("checkAvail", "username");

			// Make Url & Request
			string url = MakeUrl("Registration.php", options);
			XmlRequest xml = MakeRequest(url);

			// Parse Xml Response
			if (xml.FirstTag == "error") {
				// Request Error
				throw(new Exception(xml.FirstTag + ": " + xml.BodyText));
			}

			return(xml.BodyText);
		}

		public static void RegisterAccount (string user, string pass, string mail) {
			// Set Options
			Hashtable options = new Hashtable();
			options.Add("user", user);
			options.Add("pass", CryptoUtils.MD5String(pass));
			options.Add("mail", mail);

			// Make Url & Request
			string url = MakeUrl("Registration.php", options);
			XmlRequest xml = MakeRequest(url);

			// Parse Xml Response
			if (xml.FirstTag == "error") {
				// Request Error
				throw(new Exception(xml.FirstTag + ": " + xml.BodyText));
			}
		}

		// ============================================
		// PUBLIC STATIC Proxy Methods
		// ============================================
		public static void SetProxyServer (string server, int port) {
			proxy = new WebProxy(server, port);
			proxy.BypassProxyOnLocal = true;
		}

		public static void SetProxyCredential (string userName, string password) {
			proxy.Credentials = new NetworkCredential(userName, password);
		}

		public static void SetProxyCredential (string userName, 
											   string password,
											   string domain)
		{
			proxy.Credentials = new NetworkCredential(userName, password, domain);			
		}

		public static void UnsetProxy() {
			proxy = null;
		}

		// ============================================
		// PRIVATE Methods
		// ============================================
		private static string MakeUrl (UserInfo userInfo, string pg, Hashtable opts) 
		{
			StringBuilder url = new StringBuilder();
			url.Append("http://");
			url.Append(userInfo.GetDomain());
			url.Append("/NyFolder/");
			url.Append(pg);
			url.Append("?");
			url.Append("user=");
			url.Append(userInfo.GetName());

			AddUrlOpts(ref url, opts);

			return(url.ToString());
		}

		public static string MakeUrl (string pg, Hashtable opts) {
			StringBuilder url = new StringBuilder();
			url.Append("http://nyfolder.berlios.de/NyFolder/");
			url.Append(pg);
			url.Append("?");

			AddUrlOpts(ref url, opts);

			return(url.ToString());
		}

		private static void AddUrlOpts (ref StringBuilder url, Hashtable opts) {
			if (opts != null) {
				foreach (string name in opts.Keys) {
					url.Append('&');
					url.Append(name);
					url.Append('=');
					url.Append((string) opts[name]);
				}
			}
		}

		private static XmlRequest MakeRequest (string url) {
//			Debug.Log("Web Request: '{0}'", url);

			// Make Http Request
			WebRequest request = WebRequest.Create(url);
			request.Timeout = 5000;

			if ((proxy = Utils.Proxy.GetConfig()) != null)
				request.Proxy = proxy;

			// Wait Http Response
			WebResponse response = request.GetResponse();
			XmlRequest xmlRequest = new XmlRequest(response.GetResponseStream());
			xmlRequest.Parse();
			response.Close();
			return(xmlRequest);
		}
		
		public static  WebProxy Proxy {
			get { return(proxy); }
		}
	}
}
