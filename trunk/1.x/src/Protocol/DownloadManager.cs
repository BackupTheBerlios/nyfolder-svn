/* [ Protocol/DownloadManager.cs ] NyFolder Download Manager
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
using Niry.Utils;

using NyFolder;
using NyFolder.Utils;
using NyFolder.Protocol;

namespace NyFolder.Protocol {
	/// Download Manager
	public static class DownloadManager {
		// ============================================
		// PUBLIC Events
		// ============================================
		public static event BlankEventHandler ReceivedPart = null;
		public static event BlankEventHandler Finished = null;
		public static event BlankEventHandler Aborted = null;
		public static event BlankEventHandler Added = null;

		// ============================================
		// PRIVATE Members
		// ============================================
		private static int numDownloads = 0;

		// ============================================
		// PUBLIC Methods
		// ============================================
		/// Initialize Download Manager
		public static void Initialize() {
		}

		// ============================================
		// PRIVATE (Methods) Event Handlers
		// ============================================

		// ============================================
		// PRIVATE Methods
		// ============================================

		// ============================================
		// PUBLIC Properties
		// ============================================
		/// Get Number of Downloads
		public static int NDownloads {
			get { return(numDownloads); }
		}
	}
}
