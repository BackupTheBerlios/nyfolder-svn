/* [ Plugins/DownloadManager/FrameViewer.cs ] NyFolder Frame Viewer
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
	public abstract class FrameViewer : Gtk.Frame {
		// ============================================
		// PUBLIC Events
		// ============================================

		// ============================================
		// PROTECTED Members
		// ============================================
		protected Hashtable progressObjects = null;
		protected Gtk.VBox vbox;

		// ============================================
		// PRIVATE Members
		// ============================================
		private Gtk.ScrolledWindow scrolledWindow;

		// ============================================
		// PUBLIC Constructors
		// ============================================
		public FrameViewer() {
			// Initialize Progress Objects Hashtable
			this.progressObjects = Hashtable.Synchronized(new Hashtable());

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

		// ============================================
		// PROTECTED (Methods) Event Handlers
		// ============================================

		// ============================================
		// PRIVATE Methods
		// ============================================

		// ============================================
		// PUBLIC Properties
		// ============================================
	}
}
