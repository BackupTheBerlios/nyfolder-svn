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
		private FileProgressStore store;
		private Gtk.TreeView treeView;

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

			// Initialize TreeView
			this.store = new FileProgressStore();
			this.treeView = new Gtk.TreeView(store);
			this.scrolledWindow.Add(this.treeView);

			TreeViewColumn col;

			// UserInfo
			col = treeView.AppendColumn("User", new CellRendererText(), "text", 0);
			col.Resizable = true;
			col.Spacing = 2;

			// FileName
			col = treeView.AppendColumn("FileName", new CellRendererText(), "text", 1);
			col.Resizable = true;
			col.Spacing = 2;

			// Progress
			col = treeView.AppendColumn("Progress", new CellRendererProgress(), "value", 2);
			col.Resizable = true;
			col.Expand = true;
			col.Spacing = 2;

			// FileSize
			col = treeView.AppendColumn("Size", new CellRendererText(), "text", 3);
			col.Resizable = true;
			col.Spacing = 2;

			this.ShowAll();
		}

		// ============================================
		// PUBLIC Methods
		// ============================================
		public void Clear() {
			store.Clear();
		}

		public void Add (FileSender fileSender) {
			store.Add(fileSender);
			this.ShowAll();
		}

		public void Add (FileReceiver fileReceiver) {
			store.Add(fileReceiver);
			this.ShowAll();
		}

		public void Remove (FileSender fileSender) {
			store.Remove(fileSender);
			this.ShowAll();
		}

		public void Remove (FileReceiver fileReceiver) {
			store.Remove(fileReceiver);
			this.ShowAll();
		}

		public void Update (FileSender fileSender) {
			store.Update(fileSender);
			this.ShowAll();
		}

		public void Update (FileReceiver fileReceiver) {
			store.Update(fileReceiver);
			this.ShowAll();
		}

		// ============================================
		// PROTECTED (Methods) Event Handlers
		// ============================================

		// ============================================
		// PRIVATE Methods
		// ============================================

		// ============================================
		// PUBLIC Properties
		// ============================================
		public FileProgressStore Store {
			get { return(this.store); }
		}

		public TreeSelection Selection {
			get { return(this.treeView.Selection); }
		}
	}
}
