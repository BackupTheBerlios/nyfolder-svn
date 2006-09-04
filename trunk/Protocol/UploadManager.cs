/* [ Protocol/UploadManager.cs ] NyFolder (Upload Manager)
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

using Gtk;

using System;
using System.IO;
using System.Collections;

using Niry;
using Niry.Network;

using NyFolder;
using NyFolder.Utils;

namespace NyFolder.Protocol {
	public class UploadManagerException : Exception {
		public UploadManagerException (string msg) : base(msg) {}
		public UploadManagerException (string msg, Exception inner) : base(msg, inner) {}
	}

	public static class UploadManager {
		// ============================================
		// PUBLIC Events
		// ============================================
		public static event EndSendFilePartHandler EndFileUpdate = null;

		// ============================================
		// PRIVATE Members
		// ============================================
		private static Hashtable uploads = null;		// PeerSocket = ArrayList
		private static int numUploads = 0;

		// ============================================
		// PUBLIC Methods
		// ============================================
		public static void Initialize() {
			uploads = new Hashtable();
			numUploads = 0;
		}

		public static void Clear() {
			foreach (PeerSocket peer in uploads.Keys) {
				foreach (FileSender fileSender in (ArrayList) uploads[peer]) {
					fileSender.Stop();
					fileSender.EndSend -= new EndSendFilePartHandler(OnEndSendFilePart);
				}
				ArrayList fileSenderList = uploads[peer] as ArrayList;
				fileSenderList.Clear();
				fileSenderList = null;
			}
			uploads.Clear();
			uploads = null;
			numUploads = 0;
		}

		public static void Add (UserInfo userInfo, string path) {
			if (uploads == null)
				throw(new UploadManagerException("Upload Manager Not Initialized"));

			// Get File List
			PeerSocket peer = P2PManager.KnownPeers[userInfo] as PeerSocket;
			ArrayList fileSenderList = uploads[peer] as ArrayList;
			if (fileSenderList == null) fileSenderList = new ArrayList();

			// Initialize & Start File Sender
			FileSender fileSender = new FileSender(peer, path);
			fileSender.EndSend += new EndSendFilePartHandler(OnEndSendFilePart);
			fileSender.Start();

			// Update Upload List
			uploads[peer] = fileSenderList;

			// Update Num Uploads
			numUploads++;
		}

		public static void Remove (UserInfo userInfo, string path) {
			if (uploads == null)
				throw(new UploadManagerException("Upload Manager Not Initialized"));

			// Get File List
			PeerSocket peer = P2PManager.KnownPeers[userInfo] as PeerSocket;
			Remove(peer, path);
		}

		public static void Remove (PeerSocket peer, string path) {
			UserInfo userInfo = peer.Info as UserInfo;

			ArrayList fileSenderList = uploads[peer] as ArrayList;
			if (fileSenderList == null)
				throw(new UploadManagerException(userInfo.Name + " File '" + path + "' Not Found"));

			int filePos;
			if ((filePos = fileSenderList.IndexOf(path)) < 0)
				throw(new UploadManagerException(userInfo.Name + " File '" + path + "' Not Found"));

			// Stop File Sender Transfer
			FileSender fileSender = fileSenderList[filePos] as FileSender;
			fileSender.Stop();
			fileSender.EndSend -= new EndSendFilePartHandler(OnEndSendFilePart);

			// Remove File & Update Upload List
			fileSenderList.RemoveAt(filePos);
			uploads[peer] = fileSenderList;

			// Update Num Uploads
			numUploads--;
		}

		// ============================================
		// PROTECTED (Methods) Event Handlers
		// ============================================
		private static void OnEndSendFilePart (object sender, EndSendFilePartArgs args) {
			FileSender fileSender = sender as FileSender;

			// Remove File
			Remove(fileSender.Peer, fileSender.FileName);

			if (EndFileUpdate != null) EndFileUpdate(sender, args);
		}

		// ============================================
		// PRIVATE Methods
		// ============================================

		// ============================================
		// PUBLIC Properties
		// ============================================
		public static int NUploads {
			get { return(numUploads); }
		}
	}
}
