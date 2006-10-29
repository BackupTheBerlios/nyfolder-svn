/* [ Protocol/FileList.cs ] NyFolder Protocol File Info
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

using Niry;
using Niry.Utils;
using Niry.Network;

using NyFolder;
using NyFolder.Utils;
using NyFolder.Protocol;

namespace NyFolder.Protocol {
	/// File List, for Upload/Download Manager File Queue
	public class FileList {
		// ============================================
		// PUBLIC Events
		// ============================================

		// ============================================
		// PROTECTED Members
		// ============================================
		/// Contains All the Peer's File List
		///  - Keys: PeerSocket
		///  - Values: ArrayList of FileInfo
		protected Hashtable data = null;

		// ============================================
		// PRIVATE Members
		// ============================================

		// ============================================
		// PUBLIC Constructors
		// ============================================
		public FileList() {
			this.data = Hashtable.Synchronized(new Hashtable());
		}

		// ============================================
		// PUBLIC Methods
		// ============================================
		/// Add Peer's File Info to his list
		public void Add (PeerSocket peer, FileInfo fileInfo) {
			ArrayList fileList = GetPeerFileList(peer);
			fileList.Add(fileInfo);
			UpdatePeerFileList(peer, fileList);
		}

		/// Remove Peer's File Info from his list
		public void Remove (PeerSocket peer, FileInfo fileInfo) {
			ArrayList fileList = (ArrayList) this.data[peer];
			if (fileList != null) fileList.Remove(fileInfo);
		}

		/// Remove all Peer's File List
		public void RemoveAll() {
			foreach (ArrayList fileList in this.data.Values) {
				fileList.Clear();
			}
			this.data.Clear();
		}

		/// Remove All Peer's File List
		public void RemoveAll (PeerSocket peer) {
			if (this.data.ContainsKey(peer) == true) {
				ArrayList fileList = (ArrayList) this.data[peer];
				fileList.Clear();
				this.data.Remove(peer);
			}
		}

		// ============================================
		// PROTECTED (Methods) Event Handlers
		// ============================================
		/// Return The Peer's File List or Create New One
		protected ArrayList GetPeerFileList (PeerSocket peer) {
			ArrayList fileList = null;
			if (this.data.ContainsKey(peer) == false) {
				fileList = ArrayList.Synchronized(new ArrayList());
			} else {
				fileList = (ArrayList) this.data[peer];
			}
			return(fileList);			
		}

		/// Update The Peer's File List
		protected void UpdatePeerFileList (PeerSocket peer, ArrayList list) {
			if (this.data.ContainsKey(peer) == false) {
				this.data.Add(peer, list);
			} else {
				this.data[peer] = list;
			}
		}

		// ============================================
		// PRIVATE Methods
		// ============================================

		// ============================================
		// PUBLIC Properties
		// ============================================
	}
}
