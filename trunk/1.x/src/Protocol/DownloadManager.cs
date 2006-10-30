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
using Niry.Network;

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
		private static FileList acceptList = null;
		private static FileList recvList = null;

		private static int numDownloads = 0;

		// ============================================
		// PUBLIC (Init/Clear) Methods
		// ============================================
		/// Initialize Download Manager
		public static void Initialize() {
			// Create Thread-Safe File Lists Instances
			acceptList = new FileList();
			recvList = new FileList();
		}

		/// Abort All The Download in the List
		public static void Clear() {
			acceptList.RemoveAll();
			recvList.RemoveAll();
		}

		// ============================================
		// PUBLIC Methods
		// ============================================

		// Download Steps:
		//  - Add To Accept List
		//  - Move File From Accept List to Recv List and Init for Download
		//  - [LOOP] Get File's Part
		//  - Finish/Abort
		//  - Remove File From Recv List
		public static void Accept  (PeerSocket peer, uint id, 
									string path, string saveAs) 
		{
			FileReceiver fileRecv = new FileReceiver(id, peer, path, saveAs);
			acceptList.Add(peer, fileRecv);
		}

		public static void InitDownload (PeerSocket peer, uint id, XmlRequest xml) {
			FileReceiver fileRecv = new FileReceiver(id);
			fileRecv = (FileReceiver) acceptList.Search(peer, fileRecv);
			acceptList.Remove(peer, fileRecv);
			recvList.Add(peer, fileRecv);
			fileRecv.Init(xml);
		}

		public static void GetFilePart (PeerSocket peer, uint id, XmlRequest xml) {
			FileReceiver fileRecv = new FileReceiver(id);
			fileRecv = (FileReceiver) recvList.Search(peer, fileRecv);
			fileRecv.AddPart(xml);
		}

		public static void AbortDownload (PeerSocket peer, uint id) {
			FileReceiver fileRecv = new FileReceiver(id);
			if ((fileRecv = (FileReceiver) acceptList.Search(peer, fileRecv)) != null) {
				acceptList.Remove(peer, fileRecv);
			} else {
				recvList.Remove(peer, fileRecv);
			}
			fileRecv.Abort();			
		}

		public static void FinishedDownload (PeerSocket peer, uint id) {
			FileReceiver fileRecv = new FileReceiver(id);
			fileRecv = (FileReceiver) recvList.Search(peer, fileRecv);
			fileRecv.Save();
			recvList.Remove(peer, fileRecv);
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

		/// Get The Accept List
		public static FileList AcceptList {
			get { return(acceptList); }
		}
		
		/// Get The Receiving List
		public static FileList ReceivingList {
			get { return(recvList); }
		}
	}
}
