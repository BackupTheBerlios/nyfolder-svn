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

using Niry;
using Niry.Network;
using Niry.GUI.Gtk2;

using NyFolder;
using NyFolder.GUI;
using NyFolder.Utils;
using NyFolder.Protocol;

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

			pixbuf = new Gdk.Pixbuf(null, "TalkPopup.xpm");
			StockIcons.AddToStockImages("TalkPopup", pixbuf);
		}

		// ============================================
		// PUBLIC Methods
		// ============================================
		public override void Initialize (INyFolder nyFolder) {
			this.nyFolder = nyFolder;
			this.nyFolder.MainWindowStarted += new EventHandler(OnMainWindowStarted);
		}

		// ============================================
		// PROTECTED (Methods) Event Handlers
		// ============================================
		protected void OnMainWindowStarted (object sender, EventArgs args) {
			// Initialize Talk Component
			InitializeTalkMenu();

			// Initialize Talk Menu (Network Viewer Right Click Popup)
			nyFolder.Window.Menu.Activated += new EventHandler(OnMenuActivated);
			NetworkViewer nv = nyFolder.Window.NotebookViewer.NetworkViewer as NetworkViewer;
			nv.RightMenu += new RightMenuHandler(OnInsertNetworkRightMenu);

			SetSensitiveTalkMenu(P2PManager.IsListening());
		}

		private void OnMenuActivated (object sender, EventArgs args) {
			Gtk.Application.Invoke(delegate {
				Action action = sender as Action;
				switch (action.Name) {
					// Network
					case "NetOnline":
						OnNetOnline((ToggleAction) action);
						break;
				}
			});
		}

		private void OnNetOnline (ToggleAction action) {
			SetSensitiveTalkMenu(action.Active);

			if (action.Active == true) {
				TalkManager.Initialize(nyFolder.Window.NotebookViewer);
			} else {
				TalkManager.Reset();
			}
		}

		protected void OnTalkWith (object sender, EventArgs args) {
			Console.WriteLine("Talk With...");
			TalkWindow talkWindow = new TalkWindow(null);
			talkWindow.ShowAll();
		}

		protected void OnTalkRightMenu (object sender, EventArgs args) {
			ExtMenuItem menuItem = sender as ExtMenuItem;
			UserInfo userInfo = menuItem.ExtraData as UserInfo;
			PeerSocket peer = (PeerSocket) P2PManager.KnownPeers[userInfo];

			TalkManager.AddMessage(peer, null);
			Console.WriteLine("Talk With: {0}", userInfo.Name);
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
						"    <menu action='FileMenu'>" +
						"      <menuitem action='Talk' position='top' />" +
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
			nyFolder.Window.Menu.SetSensitive("/MenuBar/FileMenu/Talk", sensitive);
		}

		// ============================================
		// PUBLIC Properties
		// ============================================
	}
}
