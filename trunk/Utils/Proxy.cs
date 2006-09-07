/* [ Utils/Proxy.cs  ] NyFolder Proxy
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

namespace NyFolder.Utils {
	public static class Proxy {
		// ============================================
		// PUBLIC Members
		// ============================================
		public static string ConfigFile = "Proxy.conf";

		// ============================================
		// PUBLIC Members
		// ============================================
		public static bool UseProxyAuth;
		public static bool EnableProxy;
		public static string Username;
		public static string Password;
		public static string Host;
		public static int Port;

		// ============================================
		// PUBLIC Methods
		// ============================================
		public static void Initialize() {
			try {
				string file = Path.Combine(Utils.Paths.ConfigDirectory, ConfigFile);
				ParseProxyConfig(file);
			} catch {
				UseProxyAuth = false;
				EnableProxy = false;
				Username = null;
				Password = null;
				Host = null;
				Port = 80;
			}
		}

		public static WebProxy GetConfig() {
			if (EnableProxy == false || Host == null)
				return(null);

			WebProxy proxy = new WebProxy(Host, Port);

			if (UseProxyAuth == false || Username == null)
				return(proxy);

			proxy.Credentials = new NetworkCredential(Username, Password);

			return(proxy);
		}

		public static void Save() {
			string file = Path.Combine(Paths.ConfigDirectory, ConfigFile);
			XmlTextWriter xmlWriter = new XmlTextWriter(file, Encoding.UTF8);
			xmlWriter.Formatting = Formatting.Indented;

			xmlWriter.WriteStartElement("proxy-settings");
			
			// Proxy
			xmlWriter.WriteStartElement("proxy");	

			// Enabled
			xmlWriter.WriteStartAttribute("enabled");
			xmlWriter.WriteString(EnableProxy.ToString());
			xmlWriter.WriteEndAttribute();

			if (EnableProxy == true) {
				// Set Host
				xmlWriter.WriteStartAttribute("host");
				xmlWriter.WriteString(Host);
				xmlWriter.WriteEndAttribute();

				// Set Port
				xmlWriter.WriteStartAttribute("port");
				xmlWriter.WriteString(Port.ToString());
				xmlWriter.WriteEndAttribute();
			}
			xmlWriter.WriteEndElement();

			// Proxy Auth
			xmlWriter.WriteStartElement("auth");	

			// Enabled
			xmlWriter.WriteStartAttribute("enabled");
			xmlWriter.WriteString(UseProxyAuth.ToString());
			xmlWriter.WriteEndAttribute();

			if (UseProxyAuth == true) {
				// Set Host
				xmlWriter.WriteStartAttribute("username");
				xmlWriter.WriteString(Username);
				xmlWriter.WriteEndAttribute();

				// Set Port
				xmlWriter.WriteStartAttribute("password");
				xmlWriter.WriteString(Password);
				xmlWriter.WriteEndAttribute();
			}
			xmlWriter.WriteEndElement();
			xmlWriter.WriteEndElement();
			xmlWriter.Close();
		}

		// ============================================
		// PRIVATE Methods
		// ============================================
		private static void ParseProxyConfig (string file) {
			XmlTextReader xmlReader = new XmlTextReader(file);

			while (xmlReader.Read()) {
				if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.HasAttributes) {
					string elementName = xmlReader.Name;
					for (int i=0; i < xmlReader.AttributeCount; i++) {
						xmlReader.MoveToAttribute(i);

						if (xmlReader.Name == "enabled") {
							if (elementName == "proxy") {
								EnableProxy = bool.Parse(xmlReader.Value);
							} else if (elementName == "auth") {
								UseProxyAuth = bool.Parse(xmlReader.Value);
							}
						} else if (xmlReader.Name == "host") {
							Host = xmlReader.Value;
						} else if (xmlReader.Name == "port") {
							Port = int.Parse(xmlReader.Value);
						} else if (xmlReader.Name == "username") {
							Username = xmlReader.Value;
						} else if (xmlReader.Name == "password") {
							Password = xmlReader.Value;
						}
					}
				}
			}

			xmlReader.Close();
		}
	}
}
