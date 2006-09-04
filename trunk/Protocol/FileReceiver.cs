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
	public class FileReceiver {
		// ============================================
		// PUBLIC Events
		// ============================================

		// ============================================
		// PROTECTED Members
		// ============================================

		// ============================================
		// PRIVATE Members
		// ============================================
		private StreamReader streamReader;
		private Socket socket;

		// ============================================
		// PUBLIC Constructors
		// ============================================
		public FileReceiver (Socket socket) {
			this.socket = socket;
			this.streamReader = new StreamReader(new NetworkStream(this.socket));

			Thread thread = new Thread(new ThreadStart(Receive));
			thread.Start();
		}

		// ============================================
		// PUBLIC Methods
		// ============================================
		public IPAddress GetRemoteIP() {
			if (this.socket != null) {
				IPEndPoint ipEndPoint = (IPEndPoint) this.socket.RemoteEndPoint;
				return(IPAddress.Parse(ipEndPoint.Address.ToString()));
			}
			return(null);
		}

		// ============================================
		// PROTECTED (Methods) Event Handlers
		// ============================================
		protected void Receive() {
			ShareServer.NDownloads++;

			string fileName = streamReader.ReadLine();
			string saveAs = ShareServer.LookupFile(GetRemoteIP(), fileName);

			if (saveAs != null) {
				string b64file = streamReader.ReadToEnd();
				byte[] data = Convert.FromBase64String(b64file);
				
				FileStream stream = File.Create(saveAs);
				stream.Write(data, 0, data.Length);
				stream.Close();
			}

			// Close Connection
			this.streamReader.Close();
			this.socket.Close();

			ShareServer.NDownloads--;
		}

		// ============================================
		// PRIVATE Methods
		// ============================================

		// ============================================
		// PUBLIC Properties
		// ============================================
	}
}
