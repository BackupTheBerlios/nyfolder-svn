/* [ Utils/PluginManager.cs  ] NyFolder Plugin Manager
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
using System.IO;
using System.Reflection;

namespace NyFolder.Utils {	
	public class PluginManager {
		// ============================================
		// PRIVATE Members
		// ============================================
		private INyFolder nyFolder;

		// ============================================
		// PUBLIC Constructors
		// ============================================
		public PluginManager (INyFolder nyFolder) {
			this.nyFolder = nyFolder;

			FindAssemblies(Paths.SystemPluginDirectory);
			FindAssemblies(Paths.UserPluginDirectory);
		}

		// ============================================
		// PRIVATE Methods
		// ============================================
		private void ScanAssemblyForPlugins (Assembly asm) {
			foreach (Type t in asm.GetTypes()) {				
				if (t.IsSubclassOf(typeof(Plugin)) == true) {
					Plugin plugin = (Plugin) Activator.CreateInstance(t);				
					plugin.Initialize(nyFolder);
				}
			}
		}

		private void FindAssemblies (string dir) {
			if (dir == null || dir == "") return;

			DirectoryInfo info = new DirectoryInfo(dir);
			if (!info.Exists) return;

			foreach (FileInfo file in info.GetFiles()) {
				if (file.Extension != ".dll") continue;

				Assembly asm = Assembly.LoadFrom(file.FullName);
				ScanAssemblyForPlugins(asm);
			}
		}
	}
}
