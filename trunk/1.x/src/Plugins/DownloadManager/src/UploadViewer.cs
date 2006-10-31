/* [ Plugins/DownloadManager/UploadViewer.cs ] NyFolder Upload Viewer
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
	public class UploadViewer : FrameViewer {
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
		public UploadViewer() {
			UploadManager.SendedPart += new BlankEventHandler(OnSendedPart);
			UploadManager.Finished += new BlankEventHandler(OnFinished);
			UploadManager.Aborted += new BlankEventHandler(OnAborted);
			UploadManager.Added += new BlankEventHandler(OnAdded);
		}

		~UploadViewer() {
			UploadManager.SendedPart -= new BlankEventHandler(OnSendedPart);
			UploadManager.Finished -= new BlankEventHandler(OnFinished);
			UploadManager.Aborted -= new BlankEventHandler(OnAborted);
			UploadManager.Added -= new BlankEventHandler(OnAdded);
		}

		// ============================================
		// PUBLIC Methods
		// ============================================

		// ============================================
		// PROTECTED (Methods) Event Handlers
		// ============================================
		private void OnAdded (object sender) {
		Gtk.Application.Invoke(delegate {
			FileSender fileSender = sender as FileSender;

			FileProgressObject obj = new FileProgressObject();

			// Setup Image
			string ext = FileUtils.GetExtension(fileSender.DisplayedName);
			obj.Image = StockIcons.GetFileIconPixbuf(TextUtils.UpFirstChar(ext));

			// Set Transfer Info
			SetTransferInfo(obj, fileSender);
			
			// Set Info
			UserInfo userInfo = fileSender.Peer.Info as UserInfo;
			obj.SetName(fileSender.DisplayedName, userInfo.Name);

			// Add Update
			progressObjects.Add(fileSender, obj);
			vbox.PackStart(obj, false, false, 2);
			ShowAll();
		});
		}

		private void OnAborted (object sender) {
		Gtk.Application.Invoke(delegate {
			FileSender fileSender = sender as FileSender;
			FileProgressObject obj = (FileProgressObject) progressObjects[fileSender];
			SetTransferInfo(obj, fileSender);
			obj.Info += " <b>ABORTED</b>";
		});
		}

		private void OnFinished (object sender) {
		Gtk.Application.Invoke(delegate {
			FileSender fileSender = sender as FileSender;
			FileProgressObject obj = (FileProgressObject) progressObjects[fileSender];
			obj.ProgressBar.Visible = false;
			obj.Info = "<b>(Finished)</b>";
		});
		}

		private void OnSendedPart (object sender) {
		Gtk.Application.Invoke(delegate {
			FileSender fileSender = sender as FileSender;
			FileProgressObject obj = (FileProgressObject) progressObjects[fileSender];
			if (obj == null) Console.WriteLine("Sended Part NULL");
			SetTransferInfo(obj, fileSender);
		});
		}

		// ============================================
		// PRIVATE Methods
		// ============================================
		private void SetTransferInfo (FileProgressObject obj, FileSender fs) {
			obj.SetTransferInfo(fs.SendedSize, fs.Size, fs.SendedPercent);
		}

		// ============================================
		// PUBLIC Properties
		// ============================================
	}
}
