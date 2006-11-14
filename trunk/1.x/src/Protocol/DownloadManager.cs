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
using System.Collections;

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
		/// Raised When Download has Received Part
		public static event BlankEventHandler ReceivedPart = null;
		/// Raised When Download is Finished
		public static event BlankEventHandler Finished = null;
		/// Raised When Download is Aborted
		public static event BlankEventHandler Aborted = null;
		/// Raised When New Download is Added to Accept List
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

			numDownloads = 0;
		}

		/// Abort All The Download in the List
		public static void Clear() {
			acceptList.RemoveAll();
			recvList.RemoveAll();

			numDownloads = 0;
		}

		// ============================================
		// PUBLIC Methods
		// ============================================
		/// Add New Download To The Accept List
		public static void Accept  (PeerSocket peer, uint id, 
									string path, string saveAs) 
		{
			FileReceiver fileRecv = new FileReceiver(id, peer, path, saveAs);
			fileRecv.EndAbort += new BlankEventHandler(OnEndAbort);

			acceptList.Add(peer, fileRecv);

			// Update Num Downloads
			numDownloads++;

			// Raise Added Event
			if (Added != null) Added(fileRecv);
		}

		/// Move Download From Accept To Receiving List and Initialize it
		public static void InitDownload (PeerSocket peer, uint id, XmlRequest xml) {
			FileReceiver fileRecv = new FileReceiver(id);
			if ((fileRecv = (FileReceiver) acceptList.Search(peer, fileRecv)) == null) {
				ArrayList files = acceptList.GetFiles(peer);

				string name = (string) xml.Attributes["name"];
				foreach (FileReceiver fRecv in files) {
					if (fRecv.OriginalName == name) {
						fileRecv = fRecv;
						break;
					}
				}
			}
			acceptList.Remove(peer, fileRecv);
			fileRecv.Id = id;
			recvList.Add(peer, fileRecv);
			fileRecv.Init(xml);
		}

		/// Append New Data To Download
		public static void GetFilePart (PeerSocket peer, uint id, XmlRequest xml) {
			FileReceiver fileRecv = new FileReceiver(id);
			fileRecv = (FileReceiver) recvList.Search(peer, fileRecv);
			if (fileRecv == null) return;

			// Add Part
			fileRecv.AddPart(xml);

			// Raise Received Event
			if (ReceivedPart != null) ReceivedPart(fileRecv);
		}

		/// Abort Download and Remove it From Receiving or Accepted List
		public static void AbortDownload (PeerSocket peer, uint id) {
			FileReceiver fileRecv = new FileReceiver(id);
			if ((fileRecv = (FileReceiver) acceptList.Search(peer, fileRecv)) != null) {
				acceptList.Remove(peer, fileRecv);
			} else {
				fileRecv = (FileReceiver) recvList.Search(peer, new FileReceiver(id));
				if (fileRecv == null) return;
				recvList.Remove(peer, fileRecv);
			}

			AbortDownload(fileRecv);
		}

		/// Save and Remove Download From Receiving List
		public static void FinishedDownload (PeerSocket peer, uint id) {
			FileReceiver fileRecv = new FileReceiver(id);
			fileRecv = (FileReceiver) recvList.Search(peer, fileRecv);
			fileRecv.Save();
			fileRecv.EndAbort -= new BlankEventHandler(OnEndAbort);
			recvList.Remove(peer, fileRecv);

			// Update Num Downloads
			numDownloads--;

			// Raise Finished Event
			if (Finished != null) Finished(fileRecv);
		}

		// ============================================
		// PRIVATE (Methods) Event Handlers
		// ============================================
		private static void OnEndAbort (object sender) {
			FileReceiver fileRecv = sender as FileReceiver;
			AbortDownload(fileRecv);
		}

		// ============================================
		// PRIVATE Methods
		// ============================================
		private static void AbortDownload (FileReceiver fileRecv) {
			// Abort Download
			fileRecv.EndAbort -= new BlankEventHandler(OnEndAbort);
			fileRecv.Abort();

			// Update Num Downloads
			numDownloads--;

			// Raise Aborted Event
			if (Aborted != null) Aborted(fileRecv);
		}

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
