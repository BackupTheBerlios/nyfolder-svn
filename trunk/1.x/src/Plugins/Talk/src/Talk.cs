/* [ Plugins/Talk.cs ] NyFolder Talk Plugin
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
using System.IO;

using Niry;
using Niry.Utils;
using Niry.Network;
using Niry.GUI.Gtk2;

using NyFolder;
using NyFolder.GUI;
using NyFolder.Utils;
using NyFolder.Protocol;
using NyFolder.GUI.Base;
using NyFolder.PluginLib;

namespace NyFolder.Plugins.Talk {
	/// Talk (Plugin)
	public class Talk : Plugin {
		// ============================================
		// PUBLIC Events
		// ============================================

		// ============================================
		// PRIVATE Members
		// ============================================
		private INyFolder nyFolder = null;

		// ============================================
		// PUBLIC Constructors
		// ============================================
		/// Create Talk
		public Talk() {
			// Initialize Plugin Info
			this.pluginInfo = new Info();

			// Initialize Talk Stock
			Gdk.Pixbuf pixbuf;
			pixbuf = new Gdk.Pixbuf(null, "TalkBubble.png");
			StockIcons.AddToStock("TalkBubble", pixbuf);
			StockIcons.AddToStockImages("TalkBubble", pixbuf);
		}

		// ============================================
		// PUBLIC Methods
		// ============================================
		/// Initialize Tray Icon Plugin
		public override void Initialize (INyFolder iNyFolder) {
			this.nyFolder = iNyFolder;

			// Initialize GUI Events
			this.nyFolder.MainWindowStarted += new BlankEventHandler(OnMainWindowStarted);

			// Initialize Protocol Events
			CmdManager.AddProtocolEvent += new SetProtocolEventHandler(OnAddProtocolCmds);
			CmdManager.DelProtocolEvent += new SetProtocolEventHandler(OnDelProtocolCmds);
		}

		// ============================================
		// PRIVATE (Methods) Event Handlers
		// ============================================
		private void OnMainWindowStarted (object sender) {
			// Initialize Talk Component
			InitializeTalkMenu();
			SetSensitiveTalkMenu(P2PManager.IsListening());

			// Initialize Talk Menu (Network Viewer Right Click Popup)
			NetworkViewer nv = nyFolder.MainWindow.NotebookViewer.NetworkViewer as NetworkViewer;
			nv.RightMenu += new RightMenuHandler(OnInsertNetworkRightMenu);
		}

		private void OnAddProtocolCmds (P2PManager p2pManager) {
			// Initalize Talk Manager
			TalkManager.Initialize(this.nyFolder);

			// Add Protocol Event Handler
			CmdManager.UnknownEvent += new ProtocolHandler(OnUnknownEvent);

			// Sensitivize Talk Menu
			SetSensitiveTalkMenu(true);
		}

		private void OnDelProtocolCmds (P2PManager p2pManager) {
			if (nyFolder.MainWindow != null) {
				// Sensitivize Talk Menu
				SetSensitiveTalkMenu(false);
			}

			// Del Protocol Event Handler
			CmdManager.UnknownEvent -= new ProtocolHandler(OnUnknownEvent);

			// Uninitalize Talk Manager
			TalkManager.Uninitialize();
		}

		private void OnUnknownEvent (PeerSocket peer, XmlRequest xml) {
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

		private void OnTalkWith (object sender, EventArgs args) {
			TalkWithDialog dialog = new TalkWithDialog();
			if (dialog.Run() == ResponseType.Ok) {
				NetworkViewer nv = nyFolder.MainWindow.NotebookViewer.NetworkViewer;

				// Open Talk Frame
				UserInfo userInfo = nv.GetUserInfo(dialog.GetPeerSelected());
				TalkManager.AddTalkFrame(userInfo);
			}
			dialog.Destroy();
		}

		private void OnTalkRightMenu (object sender, EventArgs args) {
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

			nyFolder.MainWindow.Menu.AddMenus(ui, entries);
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
			nyFolder.MainWindow.Menu.SetSensitive("/MenuBar/ToolMenu/Talk", sensitive);
		}

		// ============================================
		// PUBLIC Properties
		// ============================================
	}
}
