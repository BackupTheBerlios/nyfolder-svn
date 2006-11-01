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
using System.Collections;

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
		/// Remove Finished or Aborted
		public void Clear() {
			lock (progressObjects) {
				Hashtable rm = new Hashtable();				
				foreach (FileSender fileSender in progressObjects.Keys) {
					FileProgressObject obj = (FileProgressObject) progressObjects[fileSender];
					if (obj.Finished == true) {
						obj.Delete -= new BlankEventHandler(OnButtonDelete);
						rm.Add(fileSender, obj);
					}
				}

				foreach (FileSender fileSender in rm.Keys) {
					progressObjects.Remove(fileSender);
					vbox.Remove((FileProgressObject) rm[fileSender]);
				}
				rm.Clear();
				rm = null;
			}
		}

		// ============================================
		// PRIVATE (Methods) Event Handlers
		// ============================================
		private void OnAdded (object sender) {
		Gtk.Application.Invoke(delegate {
			FileSender fileSender = sender as FileSender;

			FileProgressObject obj = new FileProgressObject();
			obj.Delete += new BlankEventHandler(OnButtonDelete);

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
			obj.ShowAll();
		});
		}

		private void OnAborted (object sender) {
		Gtk.Application.Invoke(delegate {
			FileSender fileSender = sender as FileSender;
			FileProgressObject obj = (FileProgressObject) progressObjects[fileSender];
			SetTransferInfo(obj, fileSender);
			obj.Info += " <b>ABORTED</b>";
			obj.Finished = true;
		});
		}

		private void OnFinished (object sender) {
		Gtk.Application.Invoke(delegate {
			FileSender fileSender = sender as FileSender;
			FileProgressObject obj = (FileProgressObject) progressObjects[fileSender];
			obj.ProgressBar.Visible = false;
			obj.Info = "<b>(Finished)</b>";
			obj.Finished = true;
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

		private void OnButtonDelete (object sender) {
		Gtk.Application.Invoke(delegate {
			FileProgressObject obj = sender as FileProgressObject;
			obj.Delete -= new BlankEventHandler(OnButtonDelete);

			FileSender fileSender = null;
			lock (progressObjects) {
				foreach (FileSender fileSnd in progressObjects.Keys) {
					if (progressObjects[fileSnd] == obj) {
						fileSender = fileSnd;
						break;
					}
				}

				if (obj.Finished != true)
					fileSender.Abort();

				progressObjects.Remove(fileSender);
				vbox.Remove(obj);
			}
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
