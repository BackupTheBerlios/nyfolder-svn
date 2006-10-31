/* [ Plugins/DownloadManager/DownloadViewer.cs ] NyFolder Download Viewer
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

using Niry;
using Niry.Utils;

using NyFolder;
using NyFolder.GUI;
using NyFolder.Utils;
using NyFolder.Protocol;
using NyFolder.GUI.Base;

namespace NyFolder.Plugins.DownloadManager {
	public class DownloadViewer : FrameViewer {
		// ============================================
		// PUBLIC Events
		// ============================================

		// ============================================
		// PROTECTED Members
		// ============================================

		// ============================================
		// PRIVATE Members
		// ============================================

		// ============================================
		// PUBLIC Constructors
		// ============================================
		public DownloadViewer() {
			Protocol.DownloadManager.ReceivedPart += new BlankEventHandler(OnReceivedPart);
			Protocol.DownloadManager.Finished += new BlankEventHandler(OnFinished);
			Protocol.DownloadManager.Aborted += new BlankEventHandler(OnAborted);
			Protocol.DownloadManager.Added += new BlankEventHandler(OnAdded);
		}

		~DownloadViewer() {
			Protocol.DownloadManager.ReceivedPart -= new BlankEventHandler(OnReceivedPart);
			Protocol.DownloadManager.Finished -= new BlankEventHandler(OnFinished);
			Protocol.DownloadManager.Aborted -= new BlankEventHandler(OnAborted);
			Protocol.DownloadManager.Added -= new BlankEventHandler(OnAdded);
		}

		// ============================================
		// PUBLIC Methods
		// ============================================

		// ============================================
		// PRIVATE (Methods) Event Handlers
		// ============================================
		private void OnAdded (object sender) {
		Gtk.Application.Invoke(delegate {
			FileReceiver fileRecv = sender as FileReceiver;

			FileProgressObject obj = new FileProgressObject();

			// Setup Image
			string ext = FileUtils.GetExtension(fileRecv.SaveName);
			obj.Image = StockIcons.GetFileIconPixbuf(TextUtils.UpFirstChar(ext));

			// Set Transfer Info
			SetTransferInfo(obj, fileRecv);
			
			// Set Info
			UserInfo userInfo = fileRecv.Peer.Info as UserInfo;
			obj.SetName(fileRecv.OriginalName, userInfo.Name);

			// Add Update
			progressObjects.Add(fileRecv, obj);
			vbox.PackStart(obj, false, false, 2);
			ShowAll();
		});
		}

		private void OnAborted (object sender) {
		Gtk.Application.Invoke(delegate {
			FileReceiver fileRecv = sender as FileReceiver;
			FileProgressObject obj = (FileProgressObject) progressObjects[fileRecv];
			SetTransferInfo(obj, fileRecv);
			obj.Info += " <b>ABORTED</b>";
		});
		}

		private void OnFinished (object sender) {
		Gtk.Application.Invoke(delegate {
			FileReceiver fileRecv = sender as FileReceiver;
			FileProgressObject obj = (FileProgressObject) progressObjects[fileRecv];
			obj.ProgressBar.Visible = false;
			obj.Info = "<b>(Finished)</b>";
		});
		}

		private void OnReceivedPart (object sender) {
		Gtk.Application.Invoke(delegate {
			FileReceiver fileRecv = sender as FileReceiver;
			FileProgressObject obj = (FileProgressObject) progressObjects[fileRecv];
			SetTransferInfo(obj, fileRecv);
		});
		}

		// ============================================
		// PRIVATE Methods
		// ============================================
		private void SetTransferInfo (FileProgressObject obj, FileReceiver fr) {
			int percent = fr.ReceivedPercent;
			if (percent < 0) percent = 0;
			obj.SetTransferInfo(fr.SavedSize, fr.Size, percent);
		}

		// ============================================
		// PUBLIC Properties
		// ============================================
	}
}