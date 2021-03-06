/* [ Protocol/FileInfo.cs ] NyFolder Protocol File Info
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
using Niry.Network;

using NyFolder;
using NyFolder.Utils;
using NyFolder.Protocol;

namespace NyFolder.Protocol {
	/// Abstract File Info
	public abstract class FileInfo : IComparable {
		// ============================================
		// PUBLIC Events
		// ============================================

		// ============================================
		// PROTECTED Members
		// ============================================

		// ============================================
		// PRIVATE Members
		// ============================================
		private string diskName = null;
		private string dispName = null;
		private PeerSocket peer = null;
		private long fileSize = 0;
		private ulong id;

		// ============================================
		// PUBLIC Constructors
		// ============================================
		/// Create New File Info only with Id (used for compare)
		public FileInfo (ulong id) : this(id, null, null, null) {
		}

		/// Create New File Info with id and its owner
		public FileInfo (ulong id, PeerSocket peer) : this(id, peer, null, null)
		{
		}

		/// Create New File Info with id, its owner and its name
		public FileInfo (ulong id, PeerSocket peer,
						 string diskName, string dispName)
		{
			this.id = id;
			this.peer = peer;
			if (this.peer != null) {
				this.peer.Disconnecting += new PeerEventHandler(OnPeerDisconnect);
			}
			this.diskName = diskName;
			this.dispName = dispName;
		}

		~FileInfo() {
			if (this.peer != null) {
				this.peer.Disconnecting -= new PeerEventHandler(OnPeerDisconnect);
			}
		}

		// ============================================
		// PUBLIC Methods
		// ============================================
		/// Abort Method
		public abstract void Abort();

		/// Compare Files Id
		public int CompareTo (object obj) {
			FileInfo fileInfo = (FileInfo) obj;
			return((int) (this.id - fileInfo.Id));
		}

		// ============================================
		// PRIVATE (Methods) Event Handlers
		// ============================================
		private void OnPeerDisconnect (object sender, PeerEventArgs args) {
			Abort();
		}

		// ============================================
		// PRIVATE Methods
		// ============================================

		// ============================================
		// PUBLIC Properties
		// ============================================
		/// Get The File Id
		public ulong Id {
			get { return(this.id); }
			set { this.id = value; }
		}

		/// Get The File Size
		public long Size {
			get { return(this.fileSize); }
			protected set { this.fileSize = value; }
		}

		/// Get The Peer (File Owner)
		public PeerSocket Peer {
			get { return(this.peer); }
		}

		/// Get The Displayed Name
		public string Name {
			get { return(this.dispName); }
			protected set { this.dispName = value; }
		}

		/// Get The Name (On My Disk)
		public string MyDiskName {
			get { return(this.diskName); }
			protected set { this.diskName = value; }
		}
	}
}
