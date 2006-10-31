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
		}

		// ============================================
		// PUBLIC Methods
		// ============================================
		/// Initialize Tray Icon Plugin
		public override void Initialize (INyFolder iNyFolder) {
			this.nyFolder = iNyFolder;
			Console.WriteLine("Init Reg");
			RegistrationDialog dialog = new RegistrationDialog();
			dialog.Run();
		}

		// ============================================
		// PRIVATE (Methods) Event Handlers
		// ============================================

		// ============================================
		// PRIVATE (Methods) Menu Event Handlers 
		// ============================================

		// ============================================
		// PUBLIC Properties
		// ============================================
	}
}
