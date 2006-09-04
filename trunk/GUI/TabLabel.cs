/* [ GUI/TabLabel.cs ] NyFolder (Folder TabLabel)
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

namespace NyFolder.GUI {
	public class TabLabel : Gtk.HBox {
		private Gtk.Button button = null;
		private Gtk.Label title = null;
		private Gtk.Image icon = null;
		
		public TabLabel (Gtk.Label label) : base (false, 2) {
			this.title = label;
			this.icon = null;
			this.InitTabLabel();
		}

		public TabLabel (string label) : base (false, 2) {
			this.title = new Gtk.Label(label);
			this.title.UseMarkup = true;
			this.title.Xpad = 2;
			this.icon = null;
			this.InitTabLabel();
		}

		public TabLabel (Gtk.Label label, Gtk.Image icon) : base (false, 2) {
			this.title = label;
			this.icon = icon;
			this.InitTabLabel();
		}
		
		public TabLabel (string label, Gtk.Image icon) : base (false, 2) {
			this.title = new Gtk.Label(label);
			this.title.UseMarkup = true;
			this.title.Xpad = 2;
			this.icon = icon;
			this.InitTabLabel();
		}
		
		private void InitTabLabel () {
			// Close Tab Button
			this.InitCloseTabButton();

			// Tab Title Label
			this.PackEnd(this.title, true, true, 0);

			// Tab Icon
			if (this.icon != null)
				this.PackStart(this.icon, false, false, 2);	

			// Show All
			this.ClearFlag(Gtk.WidgetFlags.CanFocus);
			this.ShowAll();
		}

		private void InitCloseTabButton () {
			this.button = new Gtk.Button();
			this.button.Add(StockIcons.GetImage("Close"));
			this.button.Relief = Gtk.ReliefStyle.None;
			this.button.SetSizeRequest(18, 18);
			this.PackEnd(this.button, false, false, 2);
		}
		
		public void AddImage (Gtk.Image image) {
			this.icon = image;
			this.PackStart(image, false, false, 2);
		}

		// ====================
		//  PUBLIC Properties
		// ====================
		public Gtk.Label Label {
			get { return(this.title); }
			set { this.title = value; }
		}
		
		public Gtk.Image Icon {
			get { return(this.icon); }
			set { this.icon = value; }
		}
		
		public Gtk.Button Button {
			get { return(this.button); }
		}
	}
}
