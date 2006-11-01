/* [ Protocol/FileReceiver.cs ] NyFolder File Receiver
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

using Niry;
using Niry.Utils;
using Niry.Network;

using NyFolder;
using NyFolder.Utils;
using NyFolder.Protocol;

namespace NyFolder.Protocol {
	/// File Receiver
	public class FileReceiver : Protocol.FileInfo {
		// ============================================
		// PUBLIC Events
		// ============================================

		// ============================================
		// PRIVATE Members
		// ============================================
		private BinaryWriter binaryWriter = null;
		private bool saveOnExit = true;
		private string saveName = null;
		private long fileSaved = 0;

		// ============================================
		// PUBLIC Constructors
		// ============================================
		/// Create New File Receiver (Used Only for Compare)
		public FileReceiver (uint id) : base(id) {
		}

		/// Create New File Receiver
		public FileReceiver (uint id, PeerSocket peer, string fileName) : 
			base(id, peer, fileName)
		{
			this.saveName = null;
		}

		/// Create New File Receiver
		public FileReceiver (uint id, PeerSocket peer, 
							 string fileName, string saveAs) :
			base(id, peer, fileName)
		{
			this.saveName = saveAs;
		}

		/// File Receiver Distructor
		~FileReceiver() {
			if (saveOnExit == true) Save();
		}

		// ============================================
		// PUBLIC Methods
		// ============================================
		/// Initialize File
		public void Init (XmlRequest xml) {
			Size = long.Parse((string) xml.Attributes["size"]);
			OriginalName = (string) xml.Attributes["name"];

			// Create Null File and Open Binary Stream
			FileStream stream = FileUtils.CreateNullFile(SaveName, Size);
			binaryWriter = new BinaryWriter(stream);
		}

		/// Append Data to The File
		public void AddPart (XmlRequest xml) {
			int part = int.Parse((string) xml.Attributes["part"]);
			byte[] data = Convert.FromBase64String(xml.BodyText);
			fileSaved += data.Length;

			// Seek to Offset & Write Data
			binaryWriter.Seek((int)(part * FileSender.ChunkSize), SeekOrigin.Begin);
			binaryWriter.Write(data, 0, data.Length);
		}

		/// Abort The Recv Operation
		public override void Abort() {
			Abort(null);
		}
		
		public void Abort (string msgerror) {
			Save();
			saveOnExit = false;
			AbortRecvFile(msgerror);
		}

		/// Save File
		public void Save() {
			binaryWriter.Flush();
			binaryWriter.Close();
			binaryWriter = null;
		}

		// ============================================
		// PRIVATE Methods
		// ============================================
		/// <recv-abort what='file' id='10' />
		/// <recv-abort what='file' id='10'>Error Message</abort>
		private void AbortRecvFile (string msgerror) {
			// XmlRequest
			XmlRequest xmlRequest = new XmlRequest();
			xmlRequest.FirstTag = "recv-abort";
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
		public string SaveName {
			get { return((saveName == null) ? OriginalName : saveName); }
		}

		/// Get File Saved Size
		public long SavedSize {
			get { return(this.fileSaved); }
		}

		/// Get Received Percentage
		public int ReceivedPercent {
			get { return((int) ((SavedSize / (double) Size) * 100)); }
		}
	}
}
