/* [ Protocol/UploadManager.cs ] NyFolder Upload Manager
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
	/// Upload Manager
	public static class UploadManager {
		// ============================================
		// PUBLIC Events
		// ============================================
		public static event BlankEventHandler SendedPart = null;
		public static event BlankEventHandler Finished = null;
		public static event BlankEventHandler Aborted = null;
		public static event BlankEventHandler Added = null;

		// ============================================
		// PRIVATE Members
		// ============================================
		private static FileList uploadList = null;
		private static int numUploads = 0;
		private static uint fileId = 0;

		// ============================================
		// PUBLIC (Init/Clear) Methods
		// ============================================
		/// Initialize Upload Manager
		public static void Initialize() {
			// Create Thread-Safe File Lists Instances
			uploadList = new FileList();
		}

		/// Abort All The Uploads in the List
		public static void Clear() {
			uploadList.RemoveAll();
		}

		// ============================================
		// PUBLIC Methods
		// ============================================
		public void Send (PeerSocket peer, string path, string name) {
			// Update File ID
			fileID++;
		}

		public void Abort (PeerSocket peer, uint id) {
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
		/// Get Number of Uploads
		public static int NUploads {
			get { return(numUploads); }
		}
	}
}
