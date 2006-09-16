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

using NyFolder;
using NyFolder.Utils;
using NyFolder.Protocol;

namespace NyFolder.Plugins.DownloadManager.GUI {
	public class FileProgressViewer : Gtk.Frame {
		// ============================================
		// PUBLIC Events
		// ============================================

		// ============================================
		// PROTECTED Members
		// ============================================

		// ============================================
		// PRIVATE Members
		// ============================================
		private Gtk.ScrolledWindow scrolledWindow;
		private Gtk.VBox vbox;

		// ============================================
		// PUBLIC Constructors
		// ============================================
		public FileProgressViewer (string title) {
			Gtk.Label label = new Gtk.Label("<span size='large'><b>" + title + "</b></span>");
			label.UseMarkup = true;

			// Initialize Frame
			this.LabelWidget = label;
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

#if false
			// Add 1
			FileProgressObject widget = new FileProgressObject();
			this.vbox.PackStart(widget, false, false, 2);
			widget.SetName("Prova.txt", "Theo");
			widget.SetTransferInfo(10, 100, 10);

			// Add 1
			widget = new FileProgressObject();
			this.vbox.PackStart(widget, false, false, 2);
			widget.SetName("Ciao.jpg", "Neo");
			widget.SetTransferInfo(70000, 1000000, 70);
#endif
			this.ShowAll();
		}

		// ============================================
		// PUBLIC Methods
		// ============================================
		public void Clear() {
			this.vbox.Forall(new Callback(this.Remove));
		}

		public void Add (FileSender fileSender) {
			FileProgressObject widget = new FileProgressObject(fileSender);
			this.vbox.PackStart(widget, false, false, 2);
			this.ShowAll();
		}

		public void Add (FileReceiver fileReceiver) {
			FileProgressObject widget = new FileProgressObject(fileReceiver);
			this.vbox.PackStart(widget, false, false, 2);
			this.ShowAll();
		}

		public void Remove (FileSender fileSender) {
			FileProgressObject widget = Lookup(fileSender);
			if (widget != null) this.vbox.Remove(widget);
			Console.WriteLine("ToDo: Remove File Sender");
		}

		public void Remove (FileReceiver fileReceiver) {
			FileProgressObject widget = Lookup(fileReceiver);
			if (widget != null) this.vbox.Remove(widget);
			Console.WriteLine("ToDo: Remove File Receiver");
		}

		public void Update (FileSender fileSender) {
			FileProgressObject widget = Lookup(fileSender);
			if (widget != null) widget.Update();
			Console.WriteLine("ToDo: Update File Sender");
		}

		public void Update (FileReceiver fileReceiver) {
			FileProgressObject widget = Lookup(fileReceiver);
			if (widget != null) widget.Update();
			Console.WriteLine("ToDo: Update File Receiver");
		}

		// ============================================
		// PROTECTED (Methods) Event Handlers
		// ============================================
		protected FileProgressObject Lookup (object fileInfo) {
			return(null);
		}

		// ============================================
		// PRIVATE Methods
		// ============================================

		// ============================================
		// PUBLIC Properties
		// ============================================
	}
}
