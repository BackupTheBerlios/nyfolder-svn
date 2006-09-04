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
using Niry.Network;

using NyFolder;
using NyFolder.Utils;

namespace NyFolder.Protocol {
	public class DownloadManagerException : Exception {
		public DownloadManagerException (string msg) : base(msg) {}
		public DownloadManagerException (string msg, Exception inner) : base(msg, inner) {}
	}

	public static class DownloadManager {
		// ============================================
		// PUBLIC Events
		// ============================================

		// ============================================
		// PRIVATE Members
		// ============================================
		private static Hashtable recvFileList = null;
		private static Hashtable acceptList = null;

		// ============================================
		// PUBLIC Methods
		// ============================================
		public static void Initialize() {
			recvFileList = new Hashtable();
			acceptList = new Hashtable();
		}

		public static void Clear() {
			foreach (FileReceiver fileReceiver in recvFileList)
				fileReceiver.Save();
			recvFileList.Clear();
			recvFileList = null;

			foreach (Hashtable peerTable in acceptList)
				peerTable.Clear();
			acceptList.Clear();
			acceptList = null;
		}

		public static void AddToAcceptList (PeerSocket peer, string path, string savePath) {
			Hashtable peerList = acceptList[peer] as Hashtable;
			if (peerList == null) peerList = new Hashtable();

			peerList[path] = savePath;
			acceptList[peer] = peerList;
		}

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
		}

		public static void GetFilePart (PeerSocket peer, XmlRequest xml) {
			FileReceiver fileRecv = LookupFileReceiver(peer, xml);

			if (fileRecv != null) {
				fileRecv.Append(xml);
			} else {
				throw(new DownloadManagerException("What file is this ?"));
			}
		}

		public static void SaveFile (PeerSocket peer, XmlRequest xml) {
			FileReceiver fileRecv = LookupFileReceiver(peer, xml);

			if (fileRecv != null) {
				fileRecv.Save();
				RemoveFileReceiver(peer, fileRecv.FileName);
			} else {
				throw(new DownloadManagerException("What file is this ?"));
			}			
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
			if (peerList == null) peerList = new Hashtable();
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


		// ============================================
		// PUBLIC Properties
		// ============================================
		public static int NDownloads {
			get { return(recvFileList.Count); }
		}
	}
}
