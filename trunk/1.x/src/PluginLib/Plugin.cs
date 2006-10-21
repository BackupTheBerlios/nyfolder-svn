/* [ PluginLib/Plugin.cs ] NyFolder Plugin Abstract Class
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
using NyFolder;

namespace NyFolder.PluginLib {
	/// Plugin Abstract Class
	/// Plugin is Loaded with the Default Constructor
	/// and Started with the Initialize() Method.
	public abstract class Plugin {
		// ============================================
		// PROTECTED Members
		// ============================================
		/// Contains Plugin Information
		/// This Members MUST be set into Constructor
		protected PluginInfo pluginInfo;

		// ============================================
		// PUBLIC Methods
		// ============================================
		/// Initialize The Plugin Class
		public abstract void Initialize (INyFolder nyfolder);

		// ============================================
		// PUBLIC Properties
		// ============================================
		/// Return a Plugin Information Class
		public PluginInfo Info {
			get { return(this.pluginInfo); }
		}
	}
}
