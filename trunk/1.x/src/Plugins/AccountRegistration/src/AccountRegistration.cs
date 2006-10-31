/* [ Plugins/AccountRegistration.cs ] NyFolder AccountRegistration Plugin
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
using Niry.GUI.Gtk2;

using NyFolder;
using NyFolder.GUI;
using NyFolder.Utils;
using NyFolder.GUI.Base;
using NyFolder.PluginLib;

namespace NyFolder.Plugins.AccountRegistration {
	/// Account Registration (Plugin)
	public class AccountRegistration : Plugin {
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
		/// Create New Account Registration
		public AccountRegistration() {
			// Initialize Plugin Info
			this.pluginInfo = new Info();

			// Initialize Stock
			Gdk.Pixbuf pixbuf;
			pixbuf = new Gdk.Pixbuf(null, "Registration.png");
			StockIcons.AddToStock("Registration", pixbuf);
			StockIcons.AddToStockImages("Registration", pixbuf);
		}

		// ============================================
		// PUBLIC Methods
		// ============================================
		/// Initialize Tray Icon Plugin
		public override void Initialize (INyFolder iNyFolder) {
			this.nyFolder = iNyFolder;

			this.nyFolder.LoginDialogStarted += new BlankEventHandler(OnLoginDialogStarted);
		}

		// ============================================
		// PRIVATE (Methods) Event Handlers
		// ============================================
		private void OnLoginDialogStarted (object sender) {
			GUI.Dialogs.Login loginDialog = sender as GUI.Dialogs.Login;

			AddMenu(loginDialog.Menu);
		}

		private void OnRegisterAccount (object sender, EventArgs args) {
			RegistrationDialog dialog = new RegistrationDialog();
			if (dialog.Run() == ResponseType.Ok) {
				Console.WriteLine("TODO: Do Registration");
			}
			dialog.Destroy();
		}

		// ============================================
		// PRIVATE (Methods) Menu Event Handlers 
		// ============================================
		private void AddMenu (GUI.Base.UIManager menuManager) {
			string ui = "<ui>" +
						"  <menubar name='MenuBar'>" +
						"    <menu action='FileMenu'>" +
						"      <menuitem action='RegisterAccount' position='top' />" +
						"    </menu>" +
						"  </menubar>" +
						"</ui>";

			ActionEntry[] entries = new ActionEntry[] {
				new ActionEntry("RegisterAccount", "Registration", "Register Account", null, 
								"Register New Account...", new EventHandler(OnRegisterAccount)),
			};

			menuManager.AddMenus(ui, entries);
		}

		// ============================================
		// PUBLIC Properties
		// ============================================
	}
}
