/* [ Protocol/FileReceiver.cs ] NyFolder (Share Server File Receiver)
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
using System.Text;
using System.Threading;
using System.Collections;

using Niry;
using Niry.Utils;
using Niry.Network;

using NyFolder;
using NyFolder.Utils;
using NyFolder.Protocol;

namespace NyFolder.Protocol {
	/// File Receiver
	public class FileReceiver {
		// ============================================
		// PRIVATE Members
		// ============================================
		private BinaryWriter binaryWriter;
		private PeerSocket peer;
		private string fileName;
		private long fileSaved;
		private long fileSize;

		/// Create New File Receiver
		public FileReceiver (PeerSocket peer, XmlRequest xml, string name) {
			this.peer = peer;
			this.fileSaved = 0;
			fileName = (string) xml.Attributes["name"];
			fileSize = Int32.Parse((string) xml.Attributes["size"]);

			// Create File Stream
			FileStream stream = FileUtils.CreateNullFile(name, fileSize);
			binaryWriter = new BinaryWriter(stream);
		}

		~FileReceiver() {
			Save();
		}

		// ============================================
		// PUBLIC Methods
		// ============================================
		/// Add File Part
		public void Append (XmlRequest xml) {
			int part = int.Parse((string) xml.Attributes["part"]);
			byte[] data = Convert.FromBase64String(xml.BodyText);
			fileSaved += data.Length;

			// Seek to Offset
			binaryWriter.Seek((int)(part * FileSender.ChunkSize), SeekOrigin.Begin);
			binaryWriter.Write(data, 0, data.Length);
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

		// ============================================
		// PUBLIC Properties
		// ============================================
		/// Get Peer Sender
		public PeerSocket Peer {
			get { return(this.peer); }
		}

		/// Get File Name
		public string FileName {
			get { return(this.fileName); }
		}

		/// Get File Size
		public long FileSize {
			get { return(this.fileSize); }
		}

		/// Get File Saved Size
		public long FileSavedSize {
			get { return(this.fileSaved); }
		}

		/// Get Received Percentage
		public int ReceivedPercent {
			get { return((int) (((double) fileSaved / (double) fileSize) * 100)); }
		}
	}
}
