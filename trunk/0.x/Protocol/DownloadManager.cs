/* [ Protocol/DownloadManager.cs ] NyFolder (Download Manager)
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
using Niry.Utils;
using Niry.Network;

using NyFolder;
using NyFolder.Utils;
using NyFolder.Protocol;

namespace NyFolder.Protocol {
	public class DownloadManagerException : Exception {
		public DownloadManagerException (string msg) : base(msg) {}
		public DownloadManagerException (string msg, Exception inner) : base(msg, inner) {}
	}

	/// Download Manager
	public static class DownloadManager {
		// ============================================
		// PUBLIC Events
		// ============================================
		public static event BlankEventHandler Finished = null;
		public static event BlankEventHandler Received = null;
		public static event BlankEventHandler Added = null;

		// ============================================
		// PRIVATE Members
		// ============================================
		private static Hashtable recvFileList = null;
		private static Hashtable acceptList = null;
		private static int numDownloads = 0;

		// ============================================
		// PUBLIC Methods
		// ============================================
		/// Initialize Download Manager
		public static void Initialize() {
			recvFileList = Hashtable.Synchronized(new Hashtable());
			acceptList = Hashtable.Synchronized(new Hashtable());
		}

		/// Uninitialize Download Manager
		public static void Clear() {
			foreach (Hashtable peerTable in recvFileList.Values) {
				foreach (FileReceiver fileReceiver in peerTable.Values)
					fileReceiver.Save();
				peerTable.Clear();
			}
			recvFileList.Clear();
			recvFileList = null;

			foreach (Hashtable peerTable in acceptList.Values)
				peerTable.Clear();
			acceptList.Clear();
			acceptList = null;
		}

		/// Add File To Accept List
		public static void AddToAcceptList (PeerSocket peer, string path, string savePath) {
			Hashtable peerList = acceptList[peer] as Hashtable;

			// Initialize Peer List
			if (peerList == null)
				peerList = Hashtable.Synchronized(new Hashtable());

			peerList[path] = savePath;
			acceptList[peer] = peerList;
		}

		/// Remove file from Accept list and prepare to Receve it
		public static void InitFile (PeerSocket peer, XmlRequest xml) {
			FileReceiver fileRecv = LookupFileReceiver(peer, xml);

			string path = (string) xml.Attributes["name"];
			Hashtable peerList = acceptList[peer] as Hashtable;
			string name = (string) peerList[path];
			peerList.Remove(path);
			acceptList[peer] = peerList;

			if (fileRecv == null) {
				fileRecv = new FileReceiver(peer, xml, name);
				AddFileReceiver(peer, path, fileRecv);
			}

			numDownloads++;

			// Start New File Added
			if (Added != null) Added(fileRecv);
		}

		/// Get File Part
		public static void GetFilePart (PeerSocket peer, XmlRequest xml) {
			FileReceiver fileRecv = LookupFileReceiver(peer, xml);

			if (fileRecv != null) {
				fileRecv.Append(xml);

				// Send Received Part Event
				if (Received != null) Received(fileRecv);
			} else {
				string fileName = (string) xml.Attributes["name"];
				UserInfo userInfo = peer.Info as UserInfo;

				string message = "What file is this ?" +
								 "\nUser: " + userInfo.Name +
								 "\nFileName: " + fileName;
				throw(new DownloadManagerException(message));
			}
		}

		/// File is Ended, Save it
		public static void SaveFile (PeerSocket peer, XmlRequest xml) {
			FileReceiver fileRecv = LookupFileReceiver(peer, xml);

			if (fileRecv != null) {
				Remove(fileRecv);
			} else {
				string fileName = (string) xml.Attributes["name"];
				UserInfo userInfo = peer.Info as UserInfo;

				string message = "<b>What file is this ?</b>" +
								 "\n<b>User:</b> " + userInfo.Name +
								 "\n<b>FileName:</b> " + fileName;
				throw(new DownloadManagerException(message));
			}			
		}

		/// Remove file from Download Manager
		public static void Remove (PeerSocket peer, XmlRequest xml) {
			FileReceiver fileRecv = LookupFileReceiver(peer, xml);
			if (fileRecv != null) Remove(fileRecv);
		}

		/// Remove file from Download Manager
		public static void Remove (FileReceiver fileReceiver) {
			fileReceiver.Save();
			RemoveFileReceiver(fileReceiver.Peer, fileReceiver.FileName);
			numDownloads--;

			// Start Finished File Event
			if (Finished != null) Finished(fileReceiver);
		}

		/// Abort file Download
		public static void Abort (FileReceiver fileReceiver) {
			SendFileAbort(fileReceiver.Peer, 
						  fileReceiver.FileName, fileReceiver.FileSize);

			// Remove File
			Remove(fileReceiver);
		}

		// ============================================
		// PRIVATE (Methods) Event Handlers
		// ============================================
		private static FileReceiver LookupFileReceiver (PeerSocket peer, XmlRequest xml) {
			string fileName = (string) xml.Attributes["name"];
			Hashtable peerList = recvFileList[peer] as Hashtable;
			if (peerList == null) return(null);
			FileReceiver fileReceiver = peerList[fileName] as FileReceiver;
			return(fileReceiver);
		}

		private static void AddFileReceiver (PeerSocket peer, string name, FileReceiver fr) {
			Hashtable peerList = recvFileList[peer] as Hashtable;

			// Initialize PeerList
			if (peerList == null)
				peerList = Hashtable.Synchronized(new Hashtable());
			peerList[name] = fr;
			recvFileList[peer] = peerList;
		}

		private static void RemoveFileReceiver (PeerSocket peer, string fileName) {
			Hashtable peerList = recvFileList[peer] as Hashtable;
			peerList.Remove(fileName);
			recvFileList[peer] = peerList;	
		}

		// ============================================
		// PRIVATE Methods
		// ============================================
		private static void SendFileAbort (PeerSocket peer, string fname, long fsize) {
			// XmlRequest
			XmlRequest xmlRequest = new XmlRequest();
			xmlRequest.FirstTag = "recv-abort";
			xmlRequest.Attributes.Add("what", "file");
			xmlRequest.Attributes.Add("name", fname);

			// Send To Peer
			peer.Send(xmlRequest.GenerateXml());
		}

		// ============================================
		// PUBLIC Properties
		// ============================================
		/// Get Number of Downloads
		public static int NDownloads {
			get { return(numDownloads); }
		}

		/// Get Receiving File List
		public static Hashtable ReceivingFileList {
			get { return(recvFileList); }
		}
	}
}
