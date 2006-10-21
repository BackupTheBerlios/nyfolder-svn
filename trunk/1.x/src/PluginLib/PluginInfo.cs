/* [ PluginLib/PluginInfo.cs ] NyFolder Plugin Informations Class
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
using System.Collections;

namespace NyFolder.PluginLib {
	/// Interface for the Plugin Informations Class
	public abstract class PluginInfo : IComparable {
		protected string[] authors = null;
		protected string version = null;
		protected string name = null;
		protected string web = null;
		protected int priority = 0;

		/// Get The Plugin's Web Page
		public string Web {
			get { return(this.web); }
		}

		/// Get The Plugin's Name
		public string Name {
			get { return(this.name); }
		}

		/// Get The Plugin's Version
		public string Version {
			get { return(this.version); }
		}

		/// Get The Plugin's Authors
		public string[] Authors { 
			get { return(this.authors); }
		}

		/// Get The Plugin's Priority (Negative or Positive values ar allowed)
		public int Priority { 
			get { return(this.priority); }
		}

		/// Compare Plugins Priority
		public int CompareTo (object obj) {
			PluginInfo pluginInfo = obj as PluginInfo;
			return(Priority - pluginInfo.Priority);
		}
	}
}
