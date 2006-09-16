/* [ Plugins/DownloadManager/GUI/Glue.cs ] NyFolder (Plugin)
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

using Niry;
using Niry.Utils;
using Niry.Network;

using NyFolder;
using NyFolder.Utils;
using NyFolder.Protocol;

namespace NyFolder.Plugins.DownloadManager.GUI {
	public class Glue {
		// ============================================
		// PRIVATE Members
		// ============================================
		private GUI.Window window;

		// ============================================
		// PUBLIC Constructors
		// ============================================
		public Glue (GUI.Window window) {
			this.window = window;
			this.window.RemoveEvent += new BlankEventHandler(OnRemove);

			// Download Manager
			Protocol.DownloadManager.Added += new BlankEventHandler(AddDownload);
			Protocol.DownloadManager.Finished += new BlankEventHandler(DelDownload);
			Protocol.DownloadManager.Received += new BlankEventHandler(UpdateDownload);

			// Upload Manager
			UploadManager.Added += new BlankEventHandler(AddUpload);
			UploadManager.Finished += new BlankEventHandler(DelUpload);
			UploadManager.SendedPart += new BlankEventHandler(UpdateUpload);
		}

		~Glue() {
			// Download Manager
			Protocol.DownloadManager.Added -= new BlankEventHandler(AddDownload);
			Protocol.DownloadManager.Finished -= new BlankEventHandler(DelDownload);
			Protocol.DownloadManager.Received -= new BlankEventHandler(UpdateDownload);

			// Upload Manager
			UploadManager.Added -= new BlankEventHandler(AddUpload);
			UploadManager.Finished -= new BlankEventHandler(DelUpload);
			UploadManager.SendedPart -= new BlankEventHandler(UpdateUpload);
		}

		private void AddDownload (object sender) {
			FileReceiver fileReceiver = sender as FileReceiver;
			window.DownloadsViewer.Add(fileReceiver);
		}

		private void DelDownload (object sender) {
			FileReceiver fileReceiver = sender as FileReceiver;
			window.DownloadsViewer.Remove(fileReceiver);
		}

		private void UpdateDownload (object sender) {
			FileReceiver fileReceiver = sender as FileReceiver;
			window.DownloadsViewer.Update(fileReceiver);
		}

		private void AddUpload (object sender) {
			FileSender fileSender = sender as FileSender;
			window.UploadsViewer.Add(fileSender);
		}

		private void DelUpload (object sender) {
			FileSender fileSender = sender as FileSender;
			window.UploadsViewer.Remove(fileSender);
		}


		private void UpdateUpload (object sender) {
			FileSender fileSender = sender as FileSender;
			window.UploadsViewer.Update(fileSender);
		}

		private void OnRemove (object sender) {
			Gtk.Application.Invoke(delegate {
				RemoveDownloads();
				RemoveUploads();
			});
		}

		private void RemoveDownloads() {
			foreach (TreePath path in window.DownloadsViewer.Selection.GetSelectedRows()) {
				TreeIter iter;
				window.DownloadsViewer.Store.GetIter(out iter, path);
				FileReceiver fr = (FileReceiver) window.DownloadsViewer.Store.Lookup(iter);
				if (fr != null) {
					Protocol.DownloadManager.Abort(fr);
					window.DownloadsViewer.Store.Remove(fr);
				}
			}
		}

		private void RemoveUploads() {
			foreach (TreePath path in window.UploadsViewer.Selection.GetSelectedRows()) {
				TreeIter iter;
				window.UploadsViewer.Store.GetIter(out iter, path);

				FileSender fs = (FileSender) window.UploadsViewer.Store.Lookup(iter);
				if (fs != null) {
					Protocol.UploadManager.Abort(fs);
					window.UploadsViewer.Store.Remove(fs);
				}
			}
		}
	}
}

