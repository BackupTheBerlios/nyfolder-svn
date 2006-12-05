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

		// In This List i couldn't have two file with the same Disk Path
		// so i save it for a faster Search(diskPath)
		/// Keys: string Disk Path
		/// Values: File Info
		private Hashtable diskPaths = null;

		// ============================================
		// PRIVATE Members
		// ============================================

		// ============================================
		// PUBLIC Constructors
		// ============================================
		/// Create New File List
		public FileList() {
			this.data = Hashtable.Synchronized(new Hashtable());
			this.diskPaths = Hashtable.Synchronized(new Hashtable());
		}

		// ============================================
		// PUBLIC Methods
		// ============================================
		/// Add Peer's File Info to his list
		public void Add (PeerSocket peer, FileInfo fileInfo) {
			// Add FileInfo into Peer's ArrayList
			ArrayList fileList = GetPeerFileList(peer);
			fileList.Add(fileInfo);
			UpdatePeerFileList(peer, fileList);

			// Add Disk Path
			this.diskPaths.Add(fileInfo.MyDiskName, fileInfo);
		}

		/// Remove Peer's File Info from his list
		public void Remove (PeerSocket peer, FileInfo fileInfo) {
			// Remove Peer's FileInfo from ArrayList
			ArrayList fileList = (ArrayList) this.data[peer];
			if (fileList != null) fileList.Remove(fileInfo);
			UpdatePeerFileList(peer, fileList);

			// Remove Disk Path
			this.diskPaths.Remove(fileInfo.MyDiskName);
		}

		/// Remove all Peer's File List
		public void RemoveAll() {
			lock (this.data) {
				foreach (ArrayList fileList in this.data.Values) {
					fileList.Clear();
				}
				this.data.Clear();
				this.diskPaths.Clear();
			}
		}

		/// Remove All Peer's File List
		public void RemoveAll (PeerSocket peer) {
			if (this.data.ContainsKey(peer) == true) {
				ArrayList fileList = (ArrayList) this.data[peer];

				foreach (FileInfo fileInfo in fileList)
					this.diskPaths.Remove(fileInfo.MyDiskName);

				fileList.Clear();
				this.data.Remove(peer);
			}
		}

		/// Search File into List
		public FileInfo Search (PeerSocket peer, FileInfo fileInfo) {
			lock (this.data) {
				if (this.data.ContainsKey(peer) == true) {
					ArrayList fileList = (ArrayList) this.data[peer];
					int index = fileList.BinarySearch(fileInfo);
					if (index >= 0) return((FileInfo) fileList[index]);
				}
			}
			return(null);
		}

		/// Search Disk Name into List
		public FileInfo Search (string diskName) {
			FileInfo fileInfo = this.diskPaths[diskName] as FileInfo;
			return(fileInfo);
		}

		/// Get Peer's File List
		public ArrayList GetFiles (PeerSocket peer) {
			return((ArrayList) this.data[peer]);
		}

		/// Return true if File Path is found in someone's list
		public bool HasFile (string filePath) {
			return(this.diskPaths.ContainsKey(filePath));
		}

#if false
		// Return true if File Path is found in peer's list
		public bool HasFile (PeerSocket peer, string filePath) {
			ArrayList files = GetFiles(peer);
			foreach (FileInfo fileInfo in files) {
				if (fileInfo.MyDiskName == filePath)
					return(true);
			}
			return(false);
		}
#endif

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
