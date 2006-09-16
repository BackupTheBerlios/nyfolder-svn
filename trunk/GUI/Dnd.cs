/* [ GUI/Dnd.cs ] NyFolder (Drag & Drop Utils)
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
using System.Text;
using System.Collections;
using System.Text.RegularExpressions;

namespace NyFolder.GUI {
	public static class Dnd {
		// ============================================
		// PUBLIC Events
		// ============================================

		// ============================================
		// PROTECTED Members
		// ============================================

		// ============================================
		// PRIVATE Members
		// ============================================
		private static TargetEntry[] dndTargetTable = new TargetEntry[] {
			new TargetEntry("TEXT", 0, 0),
			new TargetEntry("STRING", 0, 1),
			new TargetEntry("text/plain", 0, 2),
			new TargetEntry("text/uri-list", 0, 3),
			new TargetEntry("_NETSCAPE_URL", 0, 4),
			new TargetEntry("application/x-color", 0, 5),
			new TargetEntry("application/x-rootwindow-drop", 0, 6),
			new TargetEntry("property/bgimage", 0, 7),
			new TargetEntry("property/keyword", 0, 8),
			new TargetEntry("x-special/gnome-icon-list", 0, 9),
			new TargetEntry("x-special/gnome-reset-background", 0, 10)
		};

		// ============================================
		// PUBLIC Methods
		// ============================================
		public static string[] GetDragReceivedUris (DragDataReceivedArgs args) {
			string uris = Encoding.UTF8.GetString(args.SelectionData.Data);
			return(Regex.Split(uris, "\r\n"));
		}

		public static object[] GetDragReceivedPaths (DragDataReceivedArgs args) {
			string[] uris = GetDragReceivedUris(args);

			ArrayList pathList = ArrayList.Synchronized(new ArrayList());
			foreach (string uri in uris) {
				string path = uri.Trim();

				// Continue, if Null Path is Found
				if (path == null || path == "" || path.Length <= 0 || path == String.Empty)
					continue;

				// Parse Path...
				if (path.StartsWith("file://") == true) {
					if (Environment.OSVersion.Platform != PlatformID.Unix) {
						// Windows: file:///D:/Prova
						path = path.Substring(8);
					} else {
						// Unix: file:///home/
						path = path.Substring(7);
					}
				}

				// Add Path To List
				pathList.Add(path);
			}

			return(pathList.ToArray());
		}

		// ============================================
		// PRIVATE Methods
		// ============================================

		// ============================================
		// PUBLIC Properties
		// ============================================
		public static TargetEntry[] TargetTable {
			get { return(dndTargetTable); }
		}
	}
}
