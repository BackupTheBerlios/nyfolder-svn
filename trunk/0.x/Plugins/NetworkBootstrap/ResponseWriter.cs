/* [ Plugins/NetworkBootstrap/ResponseParser.cs ] NyFolder (Network Bootstrap)
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
using System.Text;
using System.Collections;

namespace NyFolder.Plugins.NetworkBootstrap {
	public class ResponseWriter {
		protected StringWriter stringWriter = null;
		protected XmlTextWriter xmlWriter = null;
		
		public ResponseWriter() {
			this.stringWriter = new StringWriter();

			this.xmlWriter = new XmlTextWriter(this.stringWriter);
			this.xmlWriter.Formatting = Formatting.Indented;
			xmlWriter.WriteStartElement(null, "peerlist", null);
		}

		~ResponseWriter() {
			this.xmlWriter.Close();
			this.stringWriter.Close();
		}
		
		// <peer name='Theo@nyfolder.berlios.de' secure='true' />
		public void Add (string username) {
			xmlWriter.WriteStartElement(null, "peer", null);
			WriteAttribute("name", username);
			WriteAttribute("secure", true);
			xmlWriter.WriteEndElement();
		}

		// <peer name='Theo' secure='false' ip='127.0.0.1' port='7085' />
		public void Add (string username, string ip, int port) {
			xmlWriter.WriteStartElement(null, "peer", null);
			WriteAttribute("name", username);
			WriteAttribute("secure", false);
			WriteAttribute("ip", ip);
			WriteAttribute("port", port.ToString());
			xmlWriter.WriteEndElement();
		}

		public override string ToString() {
			xmlWriter.WriteEndElement();
			return(stringWriter.ToString());
		}

		private void WriteAttribute (string attrName, object attrValue) {
			if (attrValue != null) {
				this.xmlWriter.WriteStartAttribute(null, attrName, null);
				this.xmlWriter.WriteString(attrValue.ToString());
				this.xmlWriter.WriteEndAttribute();
			}
		}
	}
}
