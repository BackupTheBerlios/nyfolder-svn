/* [ Plugins/NetworkBootstrap/ResponseReader.cs ] NyFolder (Network Bootstrap)
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
	public class ResponseElement {
		public Hashtable Attributes;

		public ResponseElement () {
			this.Attributes = new Hashtable();
		}
	}
	
	public class ResponseReader {
		protected XmlTextReader xmlReader = null;		
		protected ArrayList elements = null;
		
		public ResponseReader (string xml) {
			StringReader stringReader = new StringReader(xml);
			this.xmlReader = new XmlTextReader(stringReader);
			this.Parse();
		}
		
		public ResponseReader (Stream stream) {
			this.xmlReader = new XmlTextReader(stream);
			this.Parse();
		}
		
		private void Parse () {			
			this.elements = new ArrayList();
			ResponseElement xmlElement = null;			
				
			while (this.xmlReader.Read()) {
				if (this.xmlReader.NodeType == XmlNodeType.Element && 
					this.xmlReader.HasAttributes &&
					this.xmlReader.Name == "peer") 
				{
					xmlElement = new ResponseElement();

					for (int i=0; i < xmlReader.AttributeCount; i++) {
						this.xmlReader.MoveToAttribute(i);
						xmlElement.Attributes.Add(this.xmlReader.Name, 
												  this.xmlReader.Value);
					}
												
					this.elements.Add(xmlElement);
				}
			}
		}
		
		public ArrayList Elements {
			get { return(this.elements); }
		}
	}
}
