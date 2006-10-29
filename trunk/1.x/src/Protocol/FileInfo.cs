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
		private string originalName;
		private PeerSocket peer;
		private long fileSize;
		private uint id;

		// ============================================
		// PUBLIC Constructors
		// ============================================
		public FileInfo (uint id) : this(id, null, null) {
		}

		public FileInfo (uint id, PeerSocket peer) : this(id, peer, null) {
		}

		public FileInfo (uint id, PeerSocket peer, string originalName) {
			this.id = id;
			this.peer = peer;
			this.originalName = originalName;
		}

		// ============================================
		// PUBLIC Methods
		// ============================================
		public abstract void Abort();

		/// Compare Files Id
		public int CompareTo (object obj) {
			FileInfo fileInfo = (FileInfo) obj;
			return((int) (this.id - fileInfo.Id));
		}

		// ============================================
		// PROTECTED (Methods) Event Handlers
		// ============================================

		// ============================================
		// PRIVATE Methods
		// ============================================

		// ============================================
		// PUBLIC Properties
		// ============================================
		/// Get The File Id
		public uint Id {
			get { return(this.id); }
		}

		/// Get The Peer (File Owner)
		public PeerSocket Peer {
			get { return(this.peer); }
		}

		/// Get The File Size
		public long FileSize {
			get { return(this.fileSize); }
			protected set { this.fileSize = value; }
		}

		// Get The Original File Name
		public string OriginalName {
			get { return(this.originalName); }
			protected set { this.originalName = value; }
		}
	}
}
