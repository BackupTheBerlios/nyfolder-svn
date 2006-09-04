/* [ Utils/Paths.cs  ] NyFolder Paths
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

using Niry;
using Niry.Utils;

using NyFolder;

namespace NyFolder.Utils {
	public static class Paths {
		private static string home_directory = null;

		public static void Initialize () {
			home_directory = Environment.GetFolderPath(
								Environment.SpecialFolder.Personal);

			FileUtils.CreateDirectory(ConfigDirectory);
			FileUtils.CreateDirectory(UserPluginDirectory);
			FileUtils.CreateDirectory(DefaultSharedDirectory);
		}

		public static string UserSharedDirectory (string username) {
			string path = Path.Combine(DefaultSharedDirectory, username);
			FileUtils.CreateDirectory(path);
			return(path);
		}

		public static string HomeDirectory {
			get { return(home_directory); }
		}

		public static string ConfigDirectory {
			get { return(Path.Combine(home_directory, ".nyFolder")); }
		}

		public static string UserPluginDirectory {
			get { return(Path.Combine(ConfigDirectory, "Plugins")); }
		}

		public static string DefaultSharedDirectory {
			get {return(Path.Combine(ConfigDirectory, "Shared")); }
		}

		// NyFolder's (System's) Directory
		public static string SystemDirectory {
			get { return(NyFolder.Info.InstallDirectory); }
		}

		public static string SystemPluginDirectory {
			get { return(Path.Combine(SystemDirectory, "Plugins")); }
		}
	}
}
