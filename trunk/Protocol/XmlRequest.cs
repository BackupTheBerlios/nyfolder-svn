/* [ Protocol/XmlRequest.cs ] NyFolder Protocol (XmlRequest)
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

namespace NyFolder.Protocol {
	/// Xml (Protocol) Request
	public class XmlRequest {
		protected XmlTextReader xmlReader = null;
		protected Hashtable xmlAttributes = null;
		protected StringBuilder xmlText = null;
		protected Stack xmlStack = null;
		protected string tag = null;

		/// Create New Xml Request
		public XmlRequest () {
			this.tag = null;
			this.xmlText = new StringBuilder();
			this.xmlAttributes = new Hashtable();
		}

		/// Create New Xml Request
		public XmlRequest (string xml) {
			StringReader stringReader = new StringReader(xml);
			this.xmlReader = new XmlTextReader(stringReader);
		}

		/// Create New Xml Request	
		public XmlRequest (Stream stream) {
			this.xmlReader = new XmlTextReader(stream);
		}

		// =======================================
		// PUBLIC Methods
		// =======================================

		/// Add MD5Sum to Xml Message
		public void AddAttributeMd5Sum () {
			string md5sum = CryptoUtils.MD5String(this.GenerateXml());
			if (this.xmlAttributes.ContainsKey("md5sum")) {
				// Already Exist, Replace md5sum value
				this.xmlAttributes["md5sum"] = md5sum;
			} else {
				this.xmlAttributes.Add("md5sum", md5sum);
			}
		}

		/// Check MD5Sum if Message have it
		public bool CheckMd5Sum() {
			string md5sum = (string) this.xmlAttributes["md5sum"];
			if (md5sum == null) return(false);			

			// Remove Md5Sum from Attributes
			this.xmlAttributes.Remove("md5sum");
			// Regenerate XmlRequest md5sum
			string md5sumCheck = CryptoUtils.MD5String(this.GenerateXml());

			bool check = md5sumCheck.Equals(md5sum);
			this.xmlAttributes.Add("md5sum", md5sum);
			return(check);
		}

		/// Generate Xml Request
		public string GenerateXml() {
			string xml;

			StringWriter strStream = new StringWriter();
			XmlTextWriter xmlWriter = new XmlTextWriter(strStream);
			
			// Start Tag <tag>
			xmlWriter.WriteStartElement(null, this.tag, null);
			
			// Attributes
			foreach (string key in this.xmlAttributes.Keys) {
				xmlWriter.WriteStartAttribute(null, key, null);
				xmlWriter.WriteString(this.xmlAttributes[key].ToString());
				xmlWriter.WriteEndAttribute();
			}

			// Message Body
			string text = xmlText.ToString();
			if (text != "") {
				string bodyB64 = TextUtils.Base64Encode(text);
				xmlWriter.WriteString(bodyB64);
			}

			// End Tag </tag>
			xmlWriter.WriteEndElement();
			
			// XML -> String
			xml = strStream.ToString();

			// Close Xml Stream
			xmlWriter.Close();
			
			return(xml);
		}
	
		/// Parse Xml Request
		// <tag attr...>ASFJHASJKFHASJGKHSAJGHASJGH</tag>
		public bool Parse () {
			this.xmlStack = new Stack();
			this.xmlText = new StringBuilder();
			this.xmlAttributes = new Hashtable();
	
			while (this.xmlReader.Read()) {
				switch (this.xmlReader.NodeType) {
					case XmlNodeType.Element: {
						this.xmlStack.Push(this.xmlReader.Name);
			
						if (this.xmlReader.HasAttributes) {
							for (int i=0; i < xmlReader.AttributeCount; i++) {
								this.xmlReader.MoveToAttribute(i);
								this.xmlAttributes.Add (this.xmlReader.Name, 
														this.xmlReader.Value);
							}
						}
						break;
					} case XmlNodeType.Text: {
						this.xmlText.Append(this.xmlReader.Value);
						break;
					} case XmlNodeType.EndElement: {
						if (OnXmlEndElement() == false) return(false);
						break;
					} default: {
						return(false);
					}
				}
			}
			return(true);
		}
	
		/// Check if Command 'cmd' is in the form <tag>...</tag> or <tag ... />
		public static bool IsEndedXml (string cmd) {
			try {
				int substrLength;
				int posStartTag;			

				if (cmd[0] == '<' && 
					cmd[cmd.Length - 2] == '/' && 
					cmd[cmd.Length - 1] == '>')
				{
					return(true);
				}

				posStartTag = cmd.IndexOf('<') + 1;
				substrLength = cmd.IndexOf('>') - posStartTag;
				string tag = cmd.Substring(posStartTag, substrLength);
			
				posStartTag = cmd.LastIndexOf('<') + 1;
				substrLength = cmd.LastIndexOf('>') - posStartTag;
				string endTag = cmd.Substring(posStartTag, substrLength);

				return((('/' + tag).StartsWith(endTag)));
			} catch {}
			
			return(false);
		}

		// PRIVATE Methods
		private bool OnXmlEndElement () {
			if (this.xmlReader.Name.Equals(this.xmlStack.Peek()))
				return(false);
			return(true);
		}
	
		// PUBLIC Properties
		/// Get or Set Xml Request First Tag
		public string FirstTag {
			get { return((tag == null) ? (string) this.xmlStack.Peek() : tag); }
			set { this.tag = value; }
		}

		/// Get Xml Request Attributes
		public Hashtable Attributes {
			get { return(this.xmlAttributes); }
		}
		
		/// Get Xml Request Original Base64 Body
		public string BodyBase64 {
			get { return(this.xmlText.ToString()); }
		}
	
		/// Get or Set Body Text
		// BASE64 -> Text	
		public string BodyText {
			get { return(TextUtils.Base64Decode(this.BodyBase64)); }
			set { this.xmlText.Append(value); }
		}
	}
}

