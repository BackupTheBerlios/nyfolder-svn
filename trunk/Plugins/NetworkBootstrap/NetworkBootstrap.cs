/* [ Plugins/NetworkBootstrap/NetworkBootstrap.cs ] NyFolder (Network Bootstrap Plugin)
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

using Gtk;

using System;

using Niry;
using Niry.Utils;
using Niry.Network;

using NyFolder;
using NyFolder.GUI;
using NyFolder.Utils;
using NyFolder.Protocol;

namespace NyFolder.Plugins.NetworkBootstrap {
	public class NetworkBootstrap : Plugin {
		// ============================================
		// PUBLIC Events
		// ============================================

		// ============================================
		// PROTECTED Members
		// ============================================
		protected INyFolder nyFolder = null;

		// ============================================
		// PRIVATE Members
		// ============================================

		// ============================================
		// PUBLIC Constructors
		// ============================================
		public NetworkBootstrap() {
			ResponseReader xml = new ResponseReader(
				"<peerlist>\n" +
				"<peer name='Theo' secure='false' ip='127.0.0.1' port='7085' />\n" +
				"<peer name='Theo@nyfolder.berlios.de' secure='true' />\n" +
				"</peerlist>");
			foreach (ResponseElement element in xml.Elements) {
				Console.WriteLine(element.Attributes["name"]);
			}
		}

		~NetworkBootstrap() {
		}

		// ============================================
		// PUBLIC Methods
		// ============================================
		public override void Initialize (INyFolder nyFolder) {
			this.nyFolder = nyFolder;
			this.nyFolder.MainWindowStarted += new BlankEventHandler(OnMainWindowStarted);
			this.nyFolder.QuittingApplication += new BlankEventHandler(OnMainAppQuit);
		}

		// ============================================
		// PROTECTED (Methods) Event Handlers
		// ============================================
		protected void OnMainWindowStarted (object sender) {
			// Initialize Protocol Events
			P2PManager.StatusChanged += new BoolEventHandler(OnP2PStatusChanged);
		}

		protected void OnMainAppQuit (object sender) {
			// Initialize Protocol Events
			P2PManager.StatusChanged -= new BoolEventHandler(OnP2PStatusChanged);
		}

		protected void OnP2PStatusChanged (object sender, bool status) {
			if (status == false) return;

			// Request All Peer Connected to...
		}

		// ============================================
		// PRIVATE Methods
		// ============================================
		private void RequestPeerList (PeerSocket peer) {
			XmlRequest xml = new XmlRequest();
			xml.FirstTag = "get";
			xml.Attributes.Add("what", "peerlist");
			Console.WriteLine(xml.GenerateXml());
		}

		// ============================================
		// PUBLIC Properties
		// ============================================
	}
}
