/* [ PluginLib/INyFolder.cs ] NyFolder Plugin Interface
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

using Niry;

using NyFolder;
using NyFolder.Utils;
using NyFolder.Protocol;

namespace NyFolder.PluginLib {
	/// Interface to allow Plugin Access into Application
	public interface INyFolder {
		// ============================================
		// PUBLIC Events
		// ============================================
		/// Event Raised When Main Window is Started
		event BlankEventHandler MainWindowStarted;
		/// Event Raised When Main Window is Closed
		event BlankEventHandler MainWindowClosed;
		/// Event Raised When Login Dialog is Started
		event BlankEventHandler LoginDialogStarted;
		/// Event Raised When Login Dialog is Closed
		event BlankEventHandler LoginDialogClosed;

		// ============================================
		// PUBLIC Properties
		// ============================================
		/// Get NyFolder Main Window or null
		GUI.Window MainWindow { get; }
		/// Get NyFolder Login Dialog or null
		GUI.Dialogs.Login LoginDialog { get; }
		/// Get My UserInfo if I've done Login
		UserInfo MyInfo { get; }
	}
}
