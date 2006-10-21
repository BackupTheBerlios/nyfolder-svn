/* [ /.cs ] NyFolder ()
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

using NyFolder;
using NyFolder.GUI;
using NyFolder.Utils;
using NyFolder.Protocol;

namespace NyFolder.Plugins.DownloadManager.GUI {
	public class FileProgressObject : Gtk.EventBox {
		// ============================================
		// PUBLIC Events
		// ============================================
		public event BlankEventHandler Delete = null;

		// ============================================
		// PROTECTED Members
		// ============================================
		protected Gtk.ProgressBar progressBar;
		protected Gtk.Button deleteButton;
		protected Gtk.Label labelStatus;
		protected Gtk.Image imageLogo;
		protected Gtk.Label labelName;

		// ============================================
		// PRIVATE Members
		// ============================================
		private FileReceiver fileReceiver = null;
		private FileSender fileSender = null;
		private Gtk.VBox vbox;
		private Gtk.HBox hbox;

		// ============================================
		// PUBLIC Constructors
		// ============================================	
		public FileProgressObject (FileSender snd) {
			this.fileSender = snd;
			InitializeObject(false);

			// Initialize Info
			UpdateFileSender(this.fileSender);
		}

		public FileProgressObject (FileReceiver recv) {
			this.fileReceiver = recv;
			InitializeObject(true);

			// Initialize Info
			UpdateFileReceiver(this.fileReceiver);
		}

		// ============================================
		// PUBLIC Methods
		// ============================================
		public void SetName (string fileName, string userName) {
			FileName = "<b>" + fileName + "</b> (" + userName + ")";
		}

		public void SetTransferInfo (long current, long total, int percent) {
			string currentStr = FileUtils.GetSizeString(current);
			string totalStr = FileUtils.GetSizeString(total);
			string percentStr = percent.ToString() + "%";
			Info = currentStr + " of " + totalStr + " (" +  percentStr + ")";

			this.progressBar.Fraction = (double) percent / (double) 100.0f;
		}

		public void Update() {
			if (this.fileSender != null) {
				UpdateFileSender(this.fileSender);
			} else if (this.fileReceiver != null) {
				UpdateFileReceiver(this.fileReceiver);
			}
		}

		// ============================================
		// PRIVATE (Methods) Event Handlers
		// ============================================
		private void OnDeleteClicked (object sender, EventArgs args) {
			if (Delete != null) Delete(this);
		}

		// ============================================
		// PROTECTED Methods
		// ============================================
		private void UpdateFileSender (FileSender snd) {
			UserInfo userInfo = snd.Peer.Info as UserInfo;
			SetName(snd.FileName, userInfo.Name);
			SetTransferInfo(snd.FileSendedSize, snd.FileSize, snd.SendedPercent);
		}

		private void UpdateFileReceiver (FileReceiver recv) {
			UserInfo userInfo = recv.Peer.Info as UserInfo;
			SetName(recv.FileName, userInfo.Name);
			SetTransferInfo(recv.FileSavedSize, recv.FileSize, recv.ReceivedPercent);
		}

		// ============================================
		// PRIVATE Methods
		// ============================================
		private void InitializeObject (bool download) {
			this.hbox = new Gtk.HBox(false, 2);
			this.Add(this.hbox);

			// Initialize Image Logo
			this.imageLogo = StockIcons.GetImage(((download == true) ? "Download": "Upload"));
			this.hbox.PackStart(this.imageLogo, false, false, 2);

			// Initialize Delete Button
			this.deleteButton = new Gtk.Button(StockIcons.GetImage("DlTrash"));
			this.deleteButton.Relief = ReliefStyle.None;
			this.deleteButton.Clicked += new EventHandler(OnDeleteClicked);
			this.hbox.PackEnd(this.deleteButton, false, false, 2);

			// Initialize VBox
			this.vbox = new Gtk.VBox(false, 2);
			this.hbox.PackStart(this.vbox, true, true, 2);

			// Initialize Name
			this.labelName = new Gtk.Label();
			this.labelName.UseMarkup = true;
			this.labelName.Xalign = 0.0f;
			this.vbox.PackStart(this.labelName, false, false, 2);

			// Initialize Status "5.8Mb of 8.1Mb (at 316.1Kb/s)"
			this.labelStatus = new Gtk.Label();
			this.labelStatus.UseMarkup = true;
			this.labelStatus.Xalign = 0.0f;
			this.vbox.PackStart(this.labelStatus, false, false, 2);		

			// Initialize ProgressBar
			this.progressBar = new Gtk.ProgressBar();
			this.vbox.PackStart(this.progressBar, false, false, 2);

			this.vbox.PackStart(new Gtk.HSeparator(), false, false, 2);

			this.ShowAll();
		}		

		// ============================================
		// PUBLIC Properties
		// ============================================
		public Gdk.Pixbuf Image {
			get { return(this.imageLogo.Pixbuf); }
			set { this.imageLogo.Pixbuf = value; }
		}

		public string FileName {
			get { return(this.labelName.Text); }
			set { this.labelName.Markup = value; }
		}

		public string Info {
			get { return(this.labelStatus.Text); }
			set { this.labelStatus.Markup = value; }
		}

		public Gtk.ProgressBar ProgressBar {
			get { return(this.progressBar); }
		}

		public object FileInfo {
			get { 
				if (this.fileSender != null)
					return(this.fileSender);
				if (this.fileReceiver != null)
					return(this.fileReceiver);
				return(null);
			}
		}
	}
}
