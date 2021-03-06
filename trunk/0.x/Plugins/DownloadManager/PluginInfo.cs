/* [ Plugins/Talk/PluginInfo.cs  ] NyFolder Talk Plugin Info
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

namespace NyFolder.Plugins.Talk {
	public class PluginInfo {
		public const string Version = "0.2";
		public const string Name = "Download Manager";
		public const string Author = "Matteo Bertozzi";

		public static string InstallDirectory = AppDomain.CurrentDomain.BaseDirectory;
	}
}
