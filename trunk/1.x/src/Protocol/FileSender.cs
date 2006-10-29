/* [ Protocol/FileSender.cs ] NyFolder File Sender
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
	/// File Sender
	public class FileSender : FileInfo {
		// ============================================
		// PUBLIC Events
		// ============================================

		// ============================================
		// PUBLIC Consts
		// ============================================
		public const uint ChunkSize = 5120;	// 1024 it's ok

		// ============================================
		// PROTECTED Members
		// ============================================

		// ============================================
		// PRIVATE Members
		// ============================================

		// ============================================
		// PUBLIC Constructors
		// ============================================
		public FileSender (uint id) : base(id) {
		}

		public FileSender (uint id, PeerSocket peer, string fileName) :
			base(id, peer, fileName)
		{
		}

		public FileSender (uint id, PeerSocket peer, 
						   string fileName, string displayName) :
			base(id, peer, fileName)
		{
		}

		// ============================================
		// PUBLIC Methods
		// ============================================
		public override void Abort() {
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
	}
}
