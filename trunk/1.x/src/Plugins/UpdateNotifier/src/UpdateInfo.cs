/* [ Plugins/UpdateNotifer/UpdateInfo.cs ] NyFolder Update Informations Window
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

//using NyFolder;
//using NyFolder.Utils;
//using NyFolder.Protocol;

namespace NyFolder.Plugins.UpdateNotifier {
	public class UpdateInfo : Gtk.Dialog {
		// ============================================
		// PUBLIC Events
		// ============================================

		// ============================================
		// PROTECTED Members
		// ============================================
		protected Gtk.Label version = null;
		protected Gtk.Label title = null;
		protected Gtk.Label infos = null;

		// ============================================
		// PRIVATE Members
		// ============================================
		private Gtk.VBox vbox = null;

		// ============================================
		// PUBLIC Constructors
		// ============================================
		public UpdateInfo() : base("Update Informations", null, DialogFlags.Modal) {
			Gtk.HBox hbox = new Gtk.HBox(false, 4);
			this.VBox.PackStart(hbox, true, true, 4);

			// Add Close Button
			AddButton(Gtk.Stock.Close, ResponseType.Close);

			// Update Logo
			Gtk.Image updateLogo = StockIcons.GetImage("UpdateNotifier");
			updateLogo.Yalign = 0;
			updateLogo.Xpad = 2;
			hbox.PackStart(updateLogo, false, false, 2);

			// Update Informations
			this.vbox = new Gtk.VBox(false, 2);
			hbox.PackStart(this.vbox, true, true, 2);

			// Title
			this.title = new Gtk.Label("<span size='x-large'><b>Update Informations</b></span>");
			this.title.UseMarkup = true;
			this.vbox.PackStart(this.title, false, false, 2);

			// Version
			this.version = new Gtk.Label();
			this.version.Xalign = 0;
			this.version.UseMarkup = true;
			this.vbox.PackStart(this.version, false, false, 2);

			// Infos
			this.infos = new Gtk.Label();
			this.infos.Xalign = 0;
			this.infos.Yalign = 0;
			this.vbox.PackStart(this.infos, true, true, 2);

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
		public string Version {
			set { this.version.Markup = "NyFolder <b>" + value + "</b>"; }
		}

		public string Infos {
			set { this.infos.Markup = value; }
		}
	}
}
