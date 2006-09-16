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
		public const uint ChunkSize = 5120;	// 1024 it's ok

		// ============================================
		// PUBLIC Events
		// ============================================
		public EndSendFilePartHandler EndSend = null;
		public BlankEventHandler SendedPart = null;
		
		// ============================================
		// PRIVATE Members
		// ============================================
		private int sendedPercent = 0;
		private long sendedSize = 0;
		private string realFileName;
		private byte[] fileContent;
		private bool ended = false;
		private long fileSize = 0;
		private PeerSocket peer;
		private string fileName;
		private Thread thread;

		public FileSender (PeerSocket peer, string fileName) {
			this.peer = peer;
			this.fileContent = null;

			// Initialize
			this.fileName = fileName;
			this.realFileName = fileName;

			// Initialize Thread :)
			thread = null;
		}

		public FileSender (PeerSocket peer, string path, string displayName) {
			this.peer = peer;
			this.fileContent = null;

			// Initialize & Read Entire File
			this.fileName = displayName;
			this.realFileName = path;

			// Initialize Thread :)
			thread = null;
		}

		// ============================================
		// PUBLIC Methods
		// ============================================
		public void Start() {
			thread = new Thread(new ThreadStart(StartSendingFile));
			thread.Name = "Send File " + fileName;
			thread.Start();
		}

		public void Stop() {
			if (ended == true) return;
			if (thread.IsAlive) {
				thread.Abort();
			}
		}

		// ============================================
		// PRIVATE Methods
		// ============================================
		private void StartSendingFile() {
			try {
				// Initialize & Read Entire File
				this.fileContent = FileUtils.ReadEntireFile(realFileName);
				this.fileSize = this.fileContent.Length;

				// Send File Start
				SendFileStart();

				// Wait One Seconds first then send Body
				Thread.Sleep(1000);

				uint npart = 0;
				while (fileContent != null) {
					long length = ChunkSize;
					if (fileContent.Length < ChunkSize)
						length = fileContent.Length;

					// Copy First `ChunkSize` byte and Send It
					byte[] buffer = new byte[length];
					Array.Copy(fileContent, buffer, length);
					
					// Send File Part Event
					SendFilePart(buffer, npart++);

					// Sended File Part
					this.sendedSize = fileSize - fileContent.Length;
					this.sendedPercent = (int) (((double) sendedSize / (double) fileSize) * 100);
					if (SendedPart != null) SendedPart(this);

					// Remove Sended Part From File
					if (fileContent.Length > ChunkSize) {
						// Remove First `ChunkSize` byte
						buffer = fileContent;
						length = buffer.Length - ChunkSize;
						fileContent = new byte[length];
						Array.Copy(buffer, ChunkSize, fileContent, 0, length);
					} else {
						fileContent = null;
					}
				}

				// Wait One Seconds first then send End
				Thread.Sleep(1000);

				// Send File End
				SendFileEnd(null);
				ended = true;

				// Send Event... End File Ok
				if (EndSend != null)
					EndSend(this, new EndSendFilePartArgs(true));
			} catch (ThreadAbortException) {
				ended = true;
			} catch (Exception e) {
				if (ended == true) return;

				SendFileEnd(e.Message);
				ended = true;

				// Send Thread Error
				if (EndSend != null)
					EndSend(this, new EndSendFilePartArgs(false, e.Message));
			}
		}

		private void SendFilePart (byte[] data, uint npart) {
			// Base64 Convert
			string b64data = Convert.ToBase64String(data);

			// XmlRequest
			XmlRequest xmlRequest = new XmlRequest();
			xmlRequest.FirstTag = "snd";
			xmlRequest.BodyText = b64data;
			xmlRequest.Attributes.Add("what", "file");
			xmlRequest.Attributes.Add("name", this.fileName);
			xmlRequest.Attributes.Add("size", this.fileSize);
			xmlRequest.Attributes.Add("part", npart);

			// Add md5sum
			xmlRequest.AddAttributeMd5Sum();

			// Send To Peer
			if (this.peer != null) this.peer.Send(xmlRequest.GenerateXml());
		}

		private void SendFileStart() {
			// XmlRequest
			XmlRequest xmlRequest = new XmlRequest();
			xmlRequest.FirstTag = "snd-start";
			xmlRequest.Attributes.Add("what", "file");
			xmlRequest.Attributes.Add("name", this.fileName);
			xmlRequest.Attributes.Add("size", this.fileSize);

			// Send To Peer
			if (this.peer != null) this.peer.Send(xmlRequest.GenerateXml());
		}

		private void SendFileEnd (string msgerror) {
			// XmlRequest
			XmlRequest xmlRequest = new XmlRequest();
			xmlRequest.FirstTag = "snd-end";
			if (msgerror != null)
				xmlRequest.BodyText = "Sending Error: " + msgerror;
			xmlRequest.Attributes.Add("what", "file");
			xmlRequest.Attributes.Add("name", this.fileName);
			xmlRequest.Attributes.Add("size", this.fileSize);

			// Send To Peer
			if (this.peer != null) this.peer.Send(xmlRequest.GenerateXml());
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

		public long FileSendedSize {
			get { return(this.sendedSize); }
		}

		public int SendedPercent {
			get { return(this.sendedPercent); }
		}
	}
}
