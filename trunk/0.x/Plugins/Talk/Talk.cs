/* [ Plugins/Talk/Talk.cs ] NyFolder (Talk Plugin)
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
using System.Threading;
using System.Collections;

using Niry;
using Niry.Utils;
using Niry.Network;
using Niry.GUI.Gtk2;

using NyFolder;
using NyFolder.GUI;
using NyFolder.Utils;
using NyFolder.Protocol;
using NyFolder.GUI.Glue;

namespace NyFolder.Plugins.Talk {
	public class Talk : Plugin {
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
		public Talk() {
			// Initialize Talk Stock
			Gdk.Pixbuf pixbuf;
			pixbuf = new Gdk.Pixbuf(null, "TalkBubble.png");
			StockIcons.AddToStock("TalkBubble", pixbuf);
			StockIcons.AddToStockImages("TalkBubble", pixbuf);
		}

		~Talk() {
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
			// Initialize Talk Component
			InitializeTalkMenu();
			SetSensitiveTalkMenu(P2PManager.IsListening());

			// Initialize Talk Menu (Network Viewer Right Click Popup)
			NetworkViewer nv = nyFolder.Window.NotebookViewer.NetworkViewer as NetworkViewer;
			nv.RightMenu += new RightMenuHandler(OnInsertNetworkRightMenu);

			// Initialize Protocol Events
			NetworkManager.AddProtocolEvent += new SetProtocolEventHandler(OnAddProtocolCmd);
			NetworkManager.DelProtocolEvent += new SetProtocolEventHandler(OnDelProtocolCmd);
		}

		protected void OnMainAppQuit (object sender) {
			// Initialize Protocol Events
			NetworkManager.AddProtocolEvent -= new SetProtocolEventHandler(OnAddProtocolCmd);
			NetworkManager.DelProtocolEvent -= new SetProtocolEventHandler(OnDelProtocolCmd);
		}

		protected void OnAddProtocolCmd (P2PManager p2p, CmdManager cmd) {
			// Initalize Talk Manager
			TalkManager.Initialize(this.nyFolder);

			// Add Protocol Event Handler
			CmdManager.UnknownEvent += new ProtocolHandler(OnUnknownEvent);

			// Sensitivize Talk Menu
			SetSensitiveTalkMenu(true);
		}

		protected void OnDelProtocolCmd (P2PManager p2p, CmdManager cmd) {
			// Uninitalize Talk Manager
			TalkManager.Uninitialize();

			// Del Protocol Event Handler
			CmdManager.UnknownEvent -= new ProtocolHandler(OnUnknownEvent);

			// Sensitivize Talk Menu
			SetSensitiveTalkMenu(false);
		}

		protected void OnUnknownEvent (PeerSocket peer, XmlRequest xml) {
			if (xml.FirstTag != "msg") return;

			Gtk.Application.Invoke(delegate {
				UserInfo userInfo = peer.Info as UserInfo;
				string type = (string) xml.Attributes["type"];
				if (type == null) {
					TalkManager.InsertMessage(userInfo, xml.BodyText);
				} else if (type == "status") {
					TalkManager.InsertStatus(userInfo, xml.BodyText);
				} else if (type == "error") {
					TalkManager.InsertError(userInfo, xml.BodyText);
				}
			});
		}

		protected void OnTalkWith (object sender, EventArgs args) {
			TalkWithDialog dialog = new TalkWithDialog();
			if (dialog.Run() == ResponseType.Ok) {
				NetworkViewer nv = nyFolder.Window.NotebookViewer.NetworkViewer;

				// Open Talk Frame
				UserInfo userInfo = nv.GetUserInfo(dialog.GetPeerSelected());
				TalkManager.AddTalkFrame(userInfo);
			}
			dialog.Destroy();
		}

		protected void OnTalkRightMenu (object sender, EventArgs args) {
			ExtMenuItem menuItem = sender as ExtMenuItem;
			UserInfo userInfo = menuItem.ExtraData as UserInfo;

			// Open Talk Frame
			TalkManager.AddTalkFrame(userInfo);
		}

		// ============================================
		// PRIVATE Methods
		// ============================================
		private void InitializeTalkMenu() {
			ActionEntry[] entries = new ActionEntry[] {
				new ActionEntry("TalkMenu", null, "_Talk", null, null, null),

				new ActionEntry("Talk", "TalkBubble", "Talk", null, 
								"Start Talk With...", new EventHandler(OnTalkWith)),
			};

			string ui = "<ui>" +
						"  <menubar name='MenuBar'>" +
						"    <menu action='ToolMenu'>" +
						"      <menuitem action='Talk' />" +
						"    </menu>" +
						"  </menubar>" +
						"</ui>";

			nyFolder.Window.Menu.AddMenus(ui, entries);
		}

		private void OnInsertNetworkRightMenu (object sender, PopupMenu menu) {
			NetworkViewer networkViewer = sender as NetworkViewer;

			foreach (TreePath treePath in networkViewer.SelectedItems) {
				UserInfo userInfo = networkViewer.Store.GetUserInfo(treePath);
				ExtMenuItem menuItem = new ExtMenuItem("Talk", "TalkBubble", userInfo);
				menu.AddItem(menuItem, new EventHandler(OnTalkRightMenu));
			}
		}

		private void SetSensitiveTalkMenu (bool sensitive) {
			nyFolder.Window.Menu.SetSensitive("/MenuBar/ToolMenu/Talk", sensitive);
		}

		// ============================================
		// PUBLIC Properties
		// ============================================
	}
}
