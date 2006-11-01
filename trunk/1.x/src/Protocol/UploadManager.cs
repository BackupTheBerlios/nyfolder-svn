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
using Niry.Network;

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
		private static FileList acceptList = null;
		private static FileList uploadList = null;
		private static int numUploads = 0;
		private static uint fileId = 1;		// ID Zero is Reserved

		// ============================================
		// PUBLIC (Init/Clear) Methods
		// ============================================
		/// Initialize Upload Manager
		public static void Initialize() {
			// Create Thread-Safe File Lists Instances
			acceptList = new FileList();
			uploadList = new FileList();
		}

		/// Abort All The Uploads in the List
		public static void Clear() {
			acceptList.RemoveAll();
			uploadList.RemoveAll();
		}

		// ============================================
		// PUBLIC Methods
		// ============================================
		public static void Add (PeerSocket peer, string path) {
			// Create New File Sender && Update File ID
			FileSender fileSender = new FileSender(fileId++, peer, path);
			fileSender.SendedPart += new BlankEventHandler(OnSendedPart);
			fileSender.EndSend += new ExceptionEventHandler(OnEndSend);

			// Add to Accept List
			acceptList.Add(peer, fileSender);

			// Send Accept Message
			fileSender.Ask();

			// Raise Upload Added Event
			if (Added != null) Added(fileSender);
		}

		/// Start File Transfer (Accepted File)
		public static void Send (PeerSocket peer, uint id) {
			FileSender fileSender = new FileSender(id);
			fileSender = (FileSender) acceptList.Search(peer, fileSender);

			// Remove From Accept and Add To The Upload List
			acceptList.Remove(peer, fileSender);
			uploadList.Add(peer, fileSender);

			// Start The File Sender
			fileSender.Start();

			// Update Num Uploads
			numUploads++;
		}

		/// Start File Transfer (Send By Name)
		public static void Send (PeerSocket peer, string path) {
			// Create New File Sender && Update File ID
			FileSender fileSender = new FileSender(fileId++, peer, path);
			fileSender.SendedPart += new BlankEventHandler(OnSendedPart);
			fileSender.EndSend += new ExceptionEventHandler(OnEndSend);

			uploadList.Add(peer, fileSender);

			// Start The File Sender
			fileSender.Start();

			// Update Num Uploads
			numUploads++;

			// Raise Upload Added Event
			if (Added != null) Added(fileSender);
		}

		/// Abort File Upload
		public static void Abort (PeerSocket peer, uint id) {
			Debug.Log("[ST] Upload Abort");
			FileSender fileSender = new FileSender(id);
			if ((fileSender = (FileSender) acceptList.Search(peer, fileSender)) != null) {
				Debug.Log(" - Remove From Accept List");
				RemoveFromAcceptList(fileSender);
			} else {
				Debug.Log(" - Remove From Recv List");
				fileSender = (FileSender) uploadList.Search(peer, new FileSender(id));
				Remove(fileSender);
			}
			if (fileSender == null) return;
			fileSender.Abort();

			// Update Num Uploads
			numUploads--;

			// Raise Aborted Event
			Debug.Log(" - Start Event");
			if (Aborted != null) Aborted(fileSender);
			Debug.Log("[ED] Upload Abort");
		}

		// ============================================
		// PRIVATE (Methods) Event Handlers
		// ============================================
		private static void OnSendedPart (object sender) {
			// Raise Sended Part Event
			if (SendedPart != null) SendedPart(sender);
		}

		private static void OnEndSend (object sender, Exception e) {
			FileSender fileSender = sender as FileSender;

			// Update Num Uploads
			numUploads--;
			
			// Upload Finished/Aborted, Remove it
			Remove(fileSender);

			if (e == null) {
				// Raise Finished Event
				if (Finished != null) Finished(fileSender);
			} else {
				// Raise Aborted Event
				if (Aborted != null) Aborted(fileSender);
			}
		}

		// ============================================
		// PRIVATE Methods
		// ============================================
		private static void Remove (FileSender fileSender) {
			if (fileSender != null) {
				fileSender.SendedPart -= new BlankEventHandler(OnSendedPart);
				fileSender.EndSend -= new ExceptionEventHandler(OnEndSend);
				uploadList.Remove(fileSender.Peer, fileSender);			
			}
		}

		private static void RemoveFromAcceptList (FileSender fileSender) {
			if (fileSender != null) {
				fileSender.SendedPart -= new BlankEventHandler(OnSendedPart);
				fileSender.EndSend -= new ExceptionEventHandler(OnEndSend);
				acceptList.Remove(fileSender.Peer, fileSender);
			}
		}

		// ============================================
		// PUBLIC Properties
		// ============================================
		/// Get Number of Uploads
		public static int NUploads {
			get { return(numUploads); }
		}
	}
}
