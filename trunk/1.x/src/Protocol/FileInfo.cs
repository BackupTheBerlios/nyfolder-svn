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
	public class FileInfo : IComparable {
		// ============================================
		// PUBLIC Events
		// ============================================

		// ============================================
		// PROTECTED Members
		// ============================================
		private string originalName;
		private string selectedName;
		private PeerSocket peer;

		// ============================================
		// PRIVATE Members
		// ============================================
		private uint id;

		// ============================================
		// PUBLIC Constructors
		// ============================================
		public FileInfo (uint id, PeerSocket peer) {
			this.id = id;
			this.peer = peer;
		}

		public FileInfo (uint id, PeerSocket peer, 
						 string originalName, string selectedName)
		{
			this.id = id;
			this.peer = peer;
			this.originalName = originalName;
			this.selectedName = selectedName;
		}

		// ============================================
		// PUBLIC Methods
		// ============================================
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

		// Get The Original File Name
		public string OriginalName {
			get { return(this.originalName); }
		}

		// Get The Selected File Name
		public string SelectedName {
			get { return(this.selectedName); }
		}
	}
}