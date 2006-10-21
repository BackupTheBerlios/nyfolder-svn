/* [ Plugins/DownloadManager/GUI/FileProgressViewer.cs ] NyFolder (Plugin)
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
using System.Collections;

using Niry;

using NyFolder;
using NyFolder.Utils;
using NyFolder.Protocol;

namespace NyFolder.Plugins.DownloadManager.GUI {
	public class FileProgressViewer : Gtk.Frame {
		// ============================================
		// PUBLIC Events
		// ============================================
		public event BlankEventHandler Delete = null;

		// ============================================
		// PROTECTED Members
		// ============================================
		protected Hashtable progressObjects = null;

		// ============================================
		// PRIVATE Members
		// ============================================
		private Gtk.ScrolledWindow scrolledWindow;
		private Gtk.VBox vbox;

		// ============================================
		// PUBLIC Constructors
		// ============================================
		public FileProgressViewer() {
			// Initialize Progress Objects Hashtable
			this.progressObjects = new Hashtable();

			// Initialize Frame
			this.Shadow = Gtk.ShadowType.None;
			this.ShadowType = Gtk.ShadowType.None;

			// Initialize Scrolled Window
			this.scrolledWindow = new Gtk.ScrolledWindow(null, null);
			this.Add(this.scrolledWindow);
			this.scrolledWindow.ShadowType = Gtk.ShadowType.None;
			this.scrolledWindow.HscrollbarPolicy = Gtk.PolicyType.Automatic;
			this.scrolledWindow.VscrollbarPolicy = Gtk.PolicyType.Always;

			// Initialize VBox
			this.vbox = new Gtk.VBox(false, 2);
			this.scrolledWindow.AddWithViewport(this.vbox);

			this.ShowAll();
		}

		// ============================================
		// PUBLIC Methods
		// ============================================
		public void Clear() {
			this.vbox.Forall(new Callback(this.Remove));
			this.progressObjects.Clear();
		}

		public void Add (FileSender fileSender) {
			FileProgressObject widget = AddFileProgress(fileSender);
			this.vbox.PackStart(widget, false, false, 2);
			this.ShowAll();
		}

		public void Add (FileReceiver fileReceiver) {
			FileProgressObject widget = AddFileProgress(fileReceiver);
			this.vbox.PackStart(widget, false, false, 2);
			this.ShowAll();
		}

		public void Remove (FileSender fileSender) {
			FileProgressObject widget = Lookup(fileSender);
			if (widget != null) {
				this.vbox.Remove(widget);
				DelFileProgress(widget, fileSender);
			}
		}

		public void Remove (FileReceiver fileReceiver) {
			FileProgressObject widget = Lookup(fileReceiver);
			if (widget != null) {
				this.vbox.Remove(widget);
				DelFileProgress(widget, fileReceiver);
			}
		}

		public void Update (FileSender fileSender) {
			FileProgressObject widget = Lookup(fileSender);
			if (widget != null) widget.Update();
		}

		public void Update (FileReceiver fileReceiver) {
			FileProgressObject widget = Lookup(fileReceiver);
			if (widget != null) widget.Update();
		}

		// ============================================
		// PROTECTED (Methods) Event Handlers
		// ============================================
		protected FileProgressObject AddFileProgress (FileSender fileSender) {
			FileProgressObject widget = new FileProgressObject(fileSender);
			widget.Delete += new BlankEventHandler(OnDeleteClicked);
			this.progressObjects.Add(fileSender, widget);
			return(widget);
		}

		protected FileProgressObject AddFileProgress (FileReceiver fileReceiver) {
			FileProgressObject widget = new FileProgressObject(fileReceiver);
			widget.Delete += new BlankEventHandler(OnDeleteClicked);
			this.progressObjects.Add(fileReceiver, widget);
			return(widget);
		}

		protected void DelFileProgress (FileProgressObject widget, object fileInfo) {
			widget.Delete -= new BlankEventHandler(OnDeleteClicked);
			this.progressObjects.Remove(fileInfo);
		}

		protected FileProgressObject Lookup (object fileInfo) {
			return((FileProgressObject) this.progressObjects[fileInfo]);
		}

		// ============================================
		// PRIVATE Methods
		// ============================================
		private void OnDeleteClicked (object sender) {
			if (Delete != null) Delete(sender);
		}

		// ============================================
		// PUBLIC Properties
		// ============================================
	}
}
