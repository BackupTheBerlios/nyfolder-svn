/* [ Protocol/ShareServer.cs ] NyFolder (Share Server)
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
using System.IO;
using System.Net;
using System.Threading;
using System.Collections;
using System.Net.Sockets;

using Niry;
using Niry.Utils;
using Niry.Network;

using NyFolder;
using NyFolder.Utils;
using NyFolder.Protocol;

namespace NyFolder.Protocol {
	public class ShareServer : TcpListener {
		// ============================================
		// PUBLIC Events
		// ============================================

		// ============================================
		// PROTECTED Members
		// ============================================

		// ============================================
		// PRIVATE Members
		// ============================================
		private static Hashtable acceptList = null;
		private static int numDownloads = 0;
		private Thread acceptThread = null;

		// ============================================
		// PUBLIC Constructors
		// ============================================
		public ShareServer (int port) : base(IPAddress.Loopback, port) {
			Debug.Log("Shared Server Started On: {0}", port);
			acceptList = new Hashtable();
		}

		~ShareServer() {
			foreach (Hashtable peerFiles in acceptList)
				peerFiles.Clear();
			acceptList.Clear();
			acceptList = null;
		}

		// ============================================
		// PUBLIC Methods
		// ============================================
		public new void Start() {
			base.Start();
			acceptThread = new Thread(new ThreadStart(AcceptThread));
			acceptThread.Start();
		}

		public new void Stop() {
			base.Stop();
			acceptThread.Abort();
		}

		// ============================================
		// PUBLIC STATIC Methods
		// ============================================
		public static void AddToAcceptList (PeerSocket peer, string path, string savePath) {
			IPAddress ipAddress = peer.GetRemoteIP();

			Hashtable peerFiles = acceptList[ipAddress] as Hashtable;
			if (peerFiles == null) peerFiles = new Hashtable();
			peerFiles[path] = savePath;
			acceptList[ipAddress] = peerFiles;
		}

		public static void RemoveFromAcceptList (PeerSocket peer, string path) {
			IPAddress ipAddress = peer.GetRemoteIP();

			Hashtable peerFiles = acceptList[ipAddress] as Hashtable;
			if (peerFiles != null) {
				peerFiles.Remove(path);
				acceptList[ipAddress] = peerFiles;
			}
		}

		public static string LookupFile (IPAddress ipAddress, string path) {
			Hashtable peerFiles = acceptList[ipAddress] as Hashtable;
			return((peerFiles == null) ? null : (string) peerFiles[path]);
		}

		// ============================================
		// PROTECTED (Methods) Event Handlers
		// ============================================
		protected void AcceptThread() {
			try {
				while (true) {
					Debug.Log("Shared Server: Waiting for Accept Socket");
					Socket socket = AcceptSocket();
					FileReceiver fileReceiver = new FileReceiver(socket);
					Debug.Log("Shared Server: Socket Accepted {0}", fileReceiver.GetRemoteIP().ToString());
				}
			} catch (Exception e) {
				Debug.Log("Share Server: {0}", e.Message);
			}
		}

		// ============================================
		// PRIVATE Methods
		// ============================================

		// ============================================
		// PUBLIC Properties
		// ============================================
		public static int NDownloads {
			get { return(numDownloads); }
			set { numDownloads = value; }
		}
	}
}
