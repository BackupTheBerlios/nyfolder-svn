/* [ Protocol/FileSender.cs ] NyFolder Protocol (File Sender)
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
using System.Text;
using System.Threading;
using System.Net.Sockets;

using Niry;
using Niry.Utils;
using Niry.Network;

using NyFolder;
using NyFolder.Utils;
using NyFolder.Protocol;

namespace NyFolder.Protocol {
	public delegate void EndSendFilePartHandler (object sender, EndSendFilePartArgs args);

	public class EndSendFilePartArgs : EventArgs {
		private bool status;
		private string message;

		public EndSendFilePartArgs (bool status) {
			this.status = status;
			this.message = null;
		}

		public EndSendFilePartArgs (bool status, string message) {
			this.status = status;
			this.message = message;
		}

		public bool Status {
			get { return(this.status); }
		}

		public string Message {
			get { return(this.message); }
		}
	}

	public class FileSender {
		// ============================================
		// PUBLIC Consts
		// ============================================
		public const int ChunkSize = 8192;	// 8192

		// ============================================
		// PUBLIC Events
		// ============================================
		public event EndSendFilePartHandler EndSend = null;

		// ============================================
		// PRIVATE Members
		// ============================================
		private StreamWriter streamWriter = null;
		private TcpClient tcpClient = null;
		private string fileName = null;
		private PeerSocket peer = null;
		private string b64file = null;
		private long b64fileSize = 0;
		private long fileSize = 0;
		private Thread thread;

		// ============================================
		// PUBLIC Constructors
		// ============================================
		public FileSender (PeerSocket peer, string fileName) {
			this.peer = peer;

			InitializeTcpClient(peer);
			InitializeAndReadFile(fileName, fileName);
		}

		public FileSender (PeerSocket peer, string fileName, string displayName) {
			this.peer = peer;

			InitializeTcpClient(peer);
			InitializeAndReadFile(fileName, displayName);
		}

		// ============================================
		// PUBLIC Methods
		// ============================================
		public void Start() {
			thread = new Thread(new ThreadStart(ThreadSendFile));
			thread.Start();
		}

		public void Stop() {
			thread.Abort();
		}

		// ============================================
		// PRIVATE Methods
		// ============================================
		private void ThreadSendFile() {
			try {
				// Start Sending FileName
				streamWriter.Write(this.fileName + "\n");

				// Send All File
				while (b64file.Length > 0) {
					int size = (b64file.Length > ChunkSize) ? ChunkSize : b64file.Length;
					string toSend = b64file.Substring(0, size);
					b64file = b64file.Remove(0, size);

					streamWriter.Write(toSend);
				}

				// Send Event... End File Ok
				if (EndSend != null) EndSend(this, new EndSendFilePartArgs(true));
			} catch (ThreadAbortException e) {
				// Send Thread Abort Error
				if (EndSend != null) EndSend(this, new EndSendFilePartArgs(false, e.Message));
			} catch (Exception e) {
				// Send Thread Error
				if (EndSend != null) EndSend(this, new EndSendFilePartArgs(false, e.Message));
			} finally {
				this.streamWriter.Close();
				this.tcpClient.Close();
			}
		}

		private void InitializeTcpClient (PeerSocket peer) {
			// Get User Info
			UserInfo userInfo = peer.Info as UserInfo;

			// Initialize TcpClient
			this.tcpClient = new TcpClient(peer.GetRemoteIP().ToString(), userInfo.Port + 1);

			// Initialize Stream Writer
			this.streamWriter = new StreamWriter(tcpClient.GetStream());
		}

		private void InitializeAndReadFile (string fileName, string displayName) {
			// Initialize FileName
			this.fileName = displayName;

			// Read All File
			byte[] fileBytes = FileUtils.ReadEntireFile(fileName);
			this.fileSize = fileBytes.Length;

			// Transform Into Base64
			this.b64file = Convert.ToBase64String(fileBytes);
			this.b64fileSize = this.b64file.Length;
		}

		// ============================================
		// PUBLIC Properties
		// ============================================
		public PeerSocket Peer {
			get { return(this.peer); }
		}

		public string FileName {
			get { return(this.fileName); }
		}

		public long FileSize {
			get { return(this.fileSize); }
		}

		public int SendedPercent {
			get {
				long sendedLength = b64fileSize - b64file.Length;
				return((int) (((double) sendedLength / (double) b64fileSize) * 100));
			}
		}
	}
}
