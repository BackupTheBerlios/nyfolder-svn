/* [ PluginLib/PluginManager.cs ] NyFolder Plugin Manager/Loader
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
using System.Collections;

using Niry;

using NyFolder;
using NyFolder.Utils;

namespace NyFolder.PluginLib {
	/// Plugin Manager, Plugins Initializing...
	public static class PluginManager {
		// ============================================
		// PUBLIC Events
		// ============================================
		/// Event Raised When Plugin fail to Start.
		public static event StringEventHandler LoadFailed = null;
		/// Event Raised When Plugin's Initialize() Failed.
		public static event StringEventHandler InitFailed = null;
		/// Event Raised When Plugin's Initialize() is Called.
		public static event BlankEventHandler Initing = null;

		// ============================================
		// PRIVATE Members
		// ============================================
		private static Hashtable pluginsTable = null;
		private static INyFolder nyFolder = null;

		// ============================================
		// PUBLIC Methods
		// ============================================
		/// Initialize The Plugin Manager and Load Base Plugins (User and System)
		public static void Initialize (INyFolder nyFolderApp) {
			// Set Interface to Access into NyFolder Application
			nyFolder = nyFolderApp;

			// Initialize Plugins Table
			pluginsTable = new Hashtable();

			// Load System's Plugins
			FindAssemblies(Paths.SystemPluginDirectory);
			// Load User's Plugins
			FindAssemblies(Paths.UserPluginDirectory);
		}

		/// Start The Plugins In The Priority Order
		public static void RunPlugins() {
			Plugin[] pluginList = SortPluginsByPriority();
			foreach (Plugin plugin in pluginList) {
				try {
					// Raise Initing Event
					if (Initing != null) Initing(plugin);

					// Initialize Plugin
					plugin.Initialize(nyFolder);
				} catch (Exception e) {
					// Raise InitFailed Event
					string msg = "Plugin's Initialization Error: " + e.Message;
					if (InitFailed != null) InitFailed(plugin, msg);

					// Remove Plugin From Table
					pluginsTable.Remove(plugin.Info);
				}
			}
		}

		// ============================================
		// PRIVATE Methods
		// ============================================
		/// Load All the Plugins found in the dir path specified.
		public static void FindAssemblies (string dirPath) {
			if (dirPath == null || dirPath == "") return;

			DirectoryInfo dirInfo = new DirectoryInfo(dirPath);
			if (dirInfo.Exists == false) return;

			foreach (FileInfo file in dirInfo.GetFiles()) {
				if (file.Extension != ".dll") continue;

				// Load Assembly and Scan It
				Assembly asm = Assembly.LoadFrom(file.FullName);
				ScanAssemblyForPlugin(asm);
			}
		}

		/// Check if Assembly is a Plugin and if it's Load it (Call Default Constructor)
		private static void ScanAssemblyForPlugin (Assembly asm) {
			foreach (Type t in asm.GetTypes()) {
				if (t.IsSubclassOf(typeof(Plugin)) == true) {
					// Type t of Assmbly file seems a plugin, Load it.
					LoadPlugin(t);
				}
			}
		}

		/// Create a New instance of The Plugin, and add it to the Plugins Table
		/// If an error occured, LoadFailed event is Raised.
		private static void LoadPlugin (Type pluginClass) {
			Plugin plugin = null;
			try {
				// Call Default Plugin Constructor
				plugin = (Plugin) Activator.CreateInstance(pluginClass);
			} catch (Exception e) {
				// Plugin Initialization Error Occurred, Raise Event
				string msg = "Plugin's Initialization Error: " + e.Message;
				if (LoadFailed != null) LoadFailed(pluginClass, msg);
				return;
			}

			// Add The Plugin To The Plugins Table
			pluginsTable.Add(plugin.Info, plugin);
		}

		/// Returns a Plugin Array with Plugin Sorted By Priority
		private static Plugin[] SortPluginsByPriority() {
			// Initialize Plugins Priority Sorted Array
			Plugin[] plugins = new Plugin[pluginsTable.Count];

			ArrayList keys = new ArrayList(pluginsTable.Keys);
			keys.Sort();

			// Fill Plugins Array
			int i = 0;
			foreach (PluginInfo pluginInfo in keys) {
				plugins[i++] = pluginsTable[pluginInfo] as Plugin;
			}

			keys = null;
			return(plugins);
		}

		// ============================================
		// PUBLIC Properties
		// ============================================
		/// Get The Plugins Table, Key is a PluginInfo Class 
		/// and Value is Plugin Class.
		public static Hashtable PluginsTable {
			get { return(pluginsTable); }
		}
	}
}
