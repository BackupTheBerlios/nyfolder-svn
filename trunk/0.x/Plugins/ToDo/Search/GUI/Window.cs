/* [ Plugins/Search/GUI/Window.cs ] NyFolder (Search Window)
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
using NyFolder.GUI;
using NyFolder.Utils;
using NyFolder.Protocol;

namespace NyFolder.Plugins.Search.GUI {
	public class Window : Gtk.Window {
		// ============================================
		// PUBLIC Events
		// ============================================

		// ============================================
		// PROTECTED Members
		// ============================================
		protected Gtk.HButtonBox hbuttonBox;
		protected Gtk.VBox vbox;

		// ============================================
		// PRIVATE Members
		// ============================================
		private ScrolledWindow swResultViewer;
		private ResultViewer resultViewer;
		private TopBar topbar;

		// ============================================
		// PUBLIC Constructors
		// ============================================
		public Window() : base("Search...") {
			// Initialize Window Options
			this.SetDefaultSize(320, 250);

			// Initialize VBox
			this.vbox = new Gtk.VBox(false, 2);
			this.Add(this.vbox);

			// Initialize TopBar
			this.topbar = new TopBar();
			this.vbox.PackStart(this.topbar, false, false, 2);

			// Initialize Search Results Label
			Gtk.Label label = new Gtk.Label("<b>Search Results:</b>");
			label.UseMarkup = true;
			label.Xalign = 0.0f;
			label.Xpad = 5;
			this.vbox.PackStart(label, false, false, 2);

			// Initialize Search Results Viewer
			this.swResultViewer = new Gtk.ScrolledWindow(null, null);
			this.swResultViewer.ShadowType = Gtk.ShadowType.None;
			this.swResultViewer.HscrollbarPolicy = Gtk.PolicyType.Automatic;
			this.swResultViewer.VscrollbarPolicy = Gtk.PolicyType.Always;
			this.vbox.PackStart(this.swResultViewer, true, true, 2);

			// Initialize Search Results Viewer
			this.resultViewer = new ResultViewer();
			this.swResultViewer.Add(this.resultViewer);

			// Initialize Button Box
			this.vbox.PackStart(new HSeparator(), false, false, 2);
			this.hbuttonBox = new Gtk.HButtonBox();
			this.hbuttonBox.Layout = ButtonBoxStyle.End;
			this.vbox.PackStart(this.hbuttonBox, false, true, 2);

			Gtk.Button button;

			// Find
			button = new Gtk.Button(Gtk.Stock.Find);
			this.hbuttonBox.PackEnd(button, false, false, 2);

			ShowAll();
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
