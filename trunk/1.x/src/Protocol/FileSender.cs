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
using System.IO;
using System.Threading;

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
		/// Event Raised When Send is Ended
		public ExceptionEventHandler EndSend = null;
		/// Event Raised When Part is Sended
		public BlankEventHandler SendedPart = null;

		// ============================================
		// PUBLIC Consts
		// ============================================
		/// Sended Block Size (Less Then 8K Please)
		public const uint ChunkSize = 5120;	// 5120 it's ok

		// ============================================
		// PRIVATE Members
		// ============================================
		private string displayedName = null;
		private int sendedPercent = 0;
		private Thread thread = null;
		private bool endSend = false;
		private long sendedSize = 0;

		// ============================================
		// PUBLIC Constructors
		// ============================================
		/// Create New File Sender (Used Only for Compare)
		public FileSender (uint id) : base(id) {
		}

		/// Create New File Sender
		public FileSender (uint id, PeerSocket peer, string path) :
			base(id, peer, path)
		{
			System.IO.FileInfo fileInfo = new System.IO.FileInfo(path);

			string sharedFolder = Paths.UserSharedDirectory(MyInfo.Name);
			if (OriginalName.StartsWith(sharedFolder) == true) {				
				this.displayedName = OriginalName.Substring(sharedFolder.Length);
			} else {
				this.displayedName = fileInfo.Name;
			}
			Size = fileInfo.Length;
		}

		/// File Sender Distructor
		~FileSender() {
			Abort();
		}

		// ============================================
		// PUBLIC Methods
		// ============================================
		/// Send Ask Message to User (Do You Want This?)
		public void Ask() {
			// XmlRequest
			XmlRequest xmlRequest = new XmlRequest();
			xmlRequest.FirstTag = "ask";
			xmlRequest.Attributes.Add("what", "file");
			xmlRequest.Attributes.Add("id", Id);
			xmlRequest.Attributes.Add("size", Size);
			xmlRequest.Attributes.Add("name", DisplayedName);

			// Send To Peer
			if (Peer != null) Peer.Send(xmlRequest.GenerateXml());
		}

		/// Abort Sending Operation
		public override void Abort() {
			if (endSend == false && thread.IsAlive == true) {
				thread.Abort();
			}
		}

		/// Abort Sending Operation
		public void Abort (Exception e) {
			// Send Abort Message + Error
			endSend = true;
			AbortSendFile(e.Message);

			// Raise End Send Event
			if (EndSend != null) EndSend(this, e);

			// Abort Thread
			if (thread.IsAlive == true) thread.Abort();
		}

		/// Start Sending File
		public void Start() {
			thread = new Thread(new ThreadStart(StartSendingFile));
			thread.Name = "Send File " + DisplayedName;
			thread.Start();
		}

		// ============================================
		// PRIVATE (Thread) Methods
		// ============================================
		private void StartSendingFile() {
			try {
				SendFileStart();
				Thread.Sleep(1500);
				if (Size < 1048576) {
					// Less Then 1M
					SendSmallFileParts();
				} else {
					SendBigFileParts();
				}
				Thread.Sleep(1500);
				SendFileEnd();
				endSend = true;

				// Raise End Send Event
				if (EndSend != null) EndSend(this, null);
			} catch (ThreadAbortException) {
				if (endSend == true) return;

				// Send Abort Message
				endSend = true;
				AbortSendFile();

				// Raise End Send Event
				if (EndSend != null) EndSend(this, null);
			} catch (Exception e) {
				if (endSend == true) return;

				// Send Abort Message + Error
				endSend = true;
				AbortSendFile(e.Message);

				// Raise End Send Event
				if (EndSend != null) EndSend(this, e);
			}
		}

		/// Split and Send "Small File" In Parts of ChunkSize
		private void SendSmallFileParts() {
			// Initialize & Read Entire File
			byte[] fileContent = FileUtils.ReadEntireFile(OriginalName);
			Size = fileContent.Length;

			uint part = 0;
			while (fileContent != null) {
				long length = ChunkSize;
				if (fileContent.Length < ChunkSize)
					length = fileContent.Length;
				
				// Copy first 'ChunkSize` block and Send it
				byte[] buffer = new byte[length];
				Array.Copy(fileContent, buffer, length);

				// Send File Part
				SendFilePart(buffer, part++);

				// Sended File Part
				sendedSize = Size - fileContent.Length;
				sendedPercent = (int) ((sendedSize / (double) Size) * 100);

				// Raise Sended Part Event
				if (SendedPart != null) SendedPart(this);
				Thread.Sleep(40);

				// Remove Sended Part From File
				if (fileContent.Length > ChunkSize) {
					buffer = fileContent;
					length = buffer.Length - ChunkSize;
					fileContent = new byte[length];
					Array.Copy(buffer, ChunkSize, fileContent, 0, length);
				} else {
					fileContent = null;
				}
			}
		}

		/// Split and Send "Big File" In Parts of ChunkSize
		private void SendBigFileParts() {
			BufferedStream stream = null;

			try {
				uint totalParts = (uint) Size / FileSender.ChunkSize;

				stream = new BufferedStream(File.OpenRead(OriginalName));
				byte[] buffer = new byte[FileSender.ChunkSize];

				for (uint part=0; part < totalParts; part++) {
					stream.Seek((int)(part * FileSender.ChunkSize), SeekOrigin.Begin);
					stream.Read(buffer, 0, (int) FileSender.ChunkSize);

					// Send File Part
					SendFilePart(buffer, part);

					// Sended File Part
					sendedSize = part * FileSender.ChunkSize;
					sendedPercent = (int) ((sendedSize / (double) Size) * 100);

					// Raise Sended Part Event
					if (SendedPart != null) SendedPart(this);
					Thread.Sleep(40);
				}

				// Send Last Block
				if ((Size % FileSender.ChunkSize) != 0) {
					int startPos = (int) (totalParts * FileSender.ChunkSize);
					stream.Seek(startPos, SeekOrigin.Begin);

					byte[] lastBuffer = new byte[(int) Size - startPos];
					stream.Read(lastBuffer, 0, lastBuffer.Length);

					// Send File Part
					SendFilePart(lastBuffer, totalParts);

					// Sended File Part
					sendedSize = Size;
					sendedPercent = 100;

					// Raise Sended Part Event
					if (SendedPart != null) SendedPart(this);
				}
			} catch (Exception e) {
				Abort(e);
			} finally {
				if (stream != null) {
					stream.Close();
					stream = null;
				}
			}
		}

		// ============================================
		// PRIVATE Methods
		// ============================================
		/// <snd what='file' id='10' part='0'>...</snd>
		private void SendFilePart (byte[] data, uint npart) {
			// Base64 Convert
			string b64data = Convert.ToBase64String(data);

			// XmlRequest
			XmlRequest xmlRequest = new XmlRequest();
			xmlRequest.FirstTag = "snd";
			xmlRequest.BodyText = b64data;
			xmlRequest.Attributes.Add("what", "file");
			xmlRequest.Attributes.Add("id", Id);
			xmlRequest.Attributes.Add("part", npart);

			// Add md5sum
			xmlRequest.AddAttributeMd5Sum();

			// Send To Peer
			if (Peer != null) Peer.Send(xmlRequest.GenerateXml());
		}

		/// <snd-start what='file' id='10' name='/pippo.txt' size='1024' />  
		private void SendFileStart() {
			// XmlRequest
			XmlRequest xmlRequest = new XmlRequest();
			xmlRequest.FirstTag = "snd-start";
			xmlRequest.Attributes.Add("what", "file");
			xmlRequest.Attributes.Add("id", Id);
			xmlRequest.Attributes.Add("size", Size);
			xmlRequest.Attributes.Add("name", DisplayedName);

			// Send To Peer
			if (Peer != null) Peer.Send(xmlRequest.GenerateXml());
		}

		/// <snd-end what='file' id='10 />
		private void SendFileEnd() {
			// XmlRequest
			XmlRequest xmlRequest = new XmlRequest();
			xmlRequest.FirstTag = "snd-end";
			xmlRequest.Attributes.Add("what", "file");
			xmlRequest.Attributes.Add("id", Id);

			// Send To Peer
			if (Peer != null) Peer.Send(xmlRequest.GenerateXml());
		}

		/// <snd-abort what='file' id='10' />
		private void AbortSendFile() {
			AbortSendFile(null);
		}

		/// <snd-abort what='file' id='10'>Error Message</abort>
		private void AbortSendFile (string msgerror) {
			// XmlRequest
			XmlRequest xmlRequest = new XmlRequest();
			xmlRequest.FirstTag = "snd-abort";
			if (msgerror != null)
				xmlRequest.BodyText = "Sending Error: " + msgerror;
			xmlRequest.Attributes.Add("what", "file");
			xmlRequest.Attributes.Add("id", Id);

			// Send To Peer
			if (Peer != null) Peer.Send(xmlRequest.GenerateXml());
		}

		// ============================================
		// PUBLIC Properties
		// ============================================
		/// Get Displayed File Name
		public string DisplayedName {
			get {
				return((displayedName == null) ? OriginalName : displayedName);
			}
		}

		/// Get The File Sended Size
		public long SendedSize {
			get { return(this.sendedSize); }
		}

		/// Get The File Sended Percent
		public int SendedPercent {
			get { return(this.sendedPercent); }
		}
	}
}
