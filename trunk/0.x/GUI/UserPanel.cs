/* [ GUI/UserPanel.cs ] NyFolder (User Panel)
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
using System.IO;

using Niry;
using Niry.Utils;

using NyFolder;
using NyFolder.Utils;
using NyFolder.Protocol;

namespace NyFolder.GUI {
	/// User Panel
	public class UserPanel : Gtk.VBox {
		// ============================================
		// PROTECTED Members
		// ============================================
		protected Gtk.Button folderButton;
		protected Gtk.Label labelDownload;
		protected Gtk.Label labelUpload;
		protected Gtk.Label labelTFiles;
		protected Gtk.Image imageFolder;

		// ============================================
		// PRIVATE Members
		// ============================================
		private Gtk.Label labelFolderButton;
		private bool timeoutUpdateRet;
		internal uint timeoutUpdate;
		private UserInfo myInfo;

		// ============================================
		// PUBLIC Constructors
		// ============================================
		public UserPanel() : base(false, 2) {
			// Get My Info
			this.myInfo = MyInfo.GetInstance();

			// Initialize Folder Image
			this.imageFolder = new Gtk.Image();
			this.PackStart(this.imageFolder, false, false, 2);
			SetOnlineStatusIcon(false);

			// Initialize Label Folder Button
			this.labelFolderButton = new Gtk.Label(GetNameLabel());
			this.labelFolderButton.Justify = Justification.Center;
			this.labelFolderButton.UseMarkup = true;

			// Initialize Folder Button
			this.folderButton = new Gtk.Button(this.labelFolderButton);
			this.folderButton.Relief = Gtk.ReliefStyle.None;
			this.PackStart(this.folderButton, false, false, 2);

			// Vertical Separator
			this.PackStart(new HSeparator(), false, false, 2);

			// Initialize Num Download Label
			this.labelDownload = new Gtk.Label("0 Download");
			this.PackStart(this.labelDownload, false, false, 2);

			// Initialize Num Upload Label
			this.labelUpload = new Gtk.Label("0 Upload");
			this.PackStart(this.labelUpload, false, false, 2);

			// Initialize Num Files Label
			this.labelTFiles = new Gtk.Label("0 Shared Files");
			this.UpdateSharedFilesNum();
			this.PackStart(this.labelTFiles, false, false, 2);	

			// Update Labels Timeout
			timeoutUpdateRet = true;
			timeoutUpdate = GLib.Timeout.Add(10000, UpdateInfoLabels);

			// Update Download Label
			DownloadManager.Added += new BlankEventHandler(UpdateDownloadNum);
			DownloadManager.Finished += new BlankEventHandler(UpdateDownloadNum);

			// Update Upload Label
			UploadManager.Added += new BlankEventHandler(UpdateUploadNum);
			UploadManager.Finished += new BlankEventHandler(UpdateUploadNum);
		}

		~UserPanel() {
			timeoutUpdateRet = false;

			// Update Download Label
			DownloadManager.Added -= new BlankEventHandler(UpdateDownloadNum);
			DownloadManager.Finished -= new BlankEventHandler(UpdateDownloadNum);

			// Update Upload Label
			UploadManager.Added -= new BlankEventHandler(UpdateUploadNum);
			UploadManager.Finished -= new BlankEventHandler(UpdateUploadNum);
		}

		// ============================================
		// PUBLIC Methods
		// ============================================
		/// Set Online Status and Change Status Image
		public void SetOnlineStatusIcon (bool online) {
			bool secure = myInfo.SecureAuthentication;
			FolderImage = StockIcons.GetPixbuf("MyFolder" + 
											   ((online) ? "Online" : "Offline") +
											   ((secure) ? "" : "Insecure"));
		}

		/// Update Shared Files Num
		public void UpdateSharedFilesNum() {
			try {
				UserInfo userInfo = MyInfo.GetInstance();
				string sharedFolder = Paths.UserSharedDirectory(userInfo.Name);
				int numFiles = CountDirectoryFiles(sharedFolder);
				this.labelTFiles.Text = numFiles.ToString() + " Shared Files";
			} catch {}
		}

		/// Update Downloads Num
		public void UpdateDownloadNum (object sender) {
			Application.Invoke(delegate {
				try {
					int numDownload = DownloadManager.NDownloads;
					this.labelDownload.Text = numDownload.ToString() + " Download";
				} catch {}
			});
		}

		/// Update Uploads Num
		public void UpdateUploadNum (object sender) {
			Application.Invoke(delegate {
				try {
					int numUpload = UploadManager.NUploads;
					this.labelUpload.Text = numUpload.ToString() + " Upload";
				} catch {}
			});
		}

		// ============================================
		// PROTECTED (Methods) Event Handlers
		// ============================================
		protected bool UpdateInfoLabels() {
			if (timeoutUpdateRet) Application.Invoke(delegate { UpdateSharedFilesNum(); });
			return(this.timeoutUpdateRet);
		}

		// ============================================
		// PRIVATE Methods
		// ============================================
		private int CountDirectoryFiles (string path) {
			if (Directory.Exists(path) == false)
				return(0);

			DirectoryInfo dirInfo = new DirectoryInfo(path);
			int numFiles = dirInfo.GetFiles().Length;

			foreach (DirectoryInfo subDir in dirInfo.GetDirectories())
				numFiles += CountDirectoryFiles(subDir.FullName);
			return(numFiles);
		}

		private string GetNameLabel() {
			string name = this.myInfo.GetName();
			int length = name.Length;

			string label;
			if (length < 14) {
				label = "<span size='x-large'><b>" + name + "</b></span>";
			} else if (length < 17) {
				label = "<span size='large'><b>" + name + "</b></span>";
			} else if (length < 19) {
				label = "<b>" + name + "</b>";
			} else {
				string pt1 = name.Substring(0, 8);
				string pt2 = name.Substring(length - 8);
				label = "<b>" + pt1 + "..." + pt2 + "</b>";
			}

			string domain = this.myInfo.GetDomain();
			if (domain == null) return(label);

			length = domain.Length;
			if (length < 20) {
				label += "\n" + domain;
			} else {
				string pt1 = domain.Substring(0, 9);
				string pt2 = domain.Substring(length - 8);
				label += "\n" + pt1 + "..." + pt2;
			}

			return(label);
		}

		// ============================================
		// PUBLIC Properties
		// ============================================
		/// Get or Set Folder Image
		public Gdk.Pixbuf FolderImage {
			get { return(this.imageFolder.Pixbuf); }
			set { this.imageFolder.Pixbuf = value; }
		}

		/// Get Folder Button
		public Gtk.Button FolderButton {
			get { return(this.folderButton); }
		}
	}
}
