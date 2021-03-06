/* [ Utils/Plugin.cs  ] NyFolder Plugin
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
using Niry.Network;

using NyFolder;
using NyFolder.Protocol;

namespace NyFolder.Utils {
	/// Plugin Interface
	public interface INyFolder {
		/// Event Handler for Quitting Application 
		event BlankEventHandler QuittingApplication;
		/// Event Handler for Main Window Started
		event BlankEventHandler MainWindowStarted;

		/// Main Window
		GUI.Window Window { get; }

		/// My Infos
		UserInfo MyInfo { get; }
	}

	public abstract class Plugin {
		public abstract void Initialize (INyFolder nyFolder);
	}
}
