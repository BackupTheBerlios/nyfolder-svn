/* [ Plugins/Search/GUI/TopBar.cs ] NyFolder (Search Window)
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
	public class TopBar : Gtk.HBox {
		// ============================================
		// PUBLIC Events
		// ============================================

		// ============================================
		// PROTECTED Members
		// ============================================
		protected Gtk.ComboBox comboSearchType;
		protected Gtk.ComboBox comboSearchUser;
		protected Gtk.Entry entrySearch;

		// ============================================
		// PRIVATE Members
		// ============================================
		private Gtk.Image imageLogo;
		private Gtk.Table table;

		// ============================================
		// PUBLIC Constructors
		// ============================================
		public TopBar() : base(false, 2) {
			// Initialize Search Logo
			this.imageLogo = StockIcons.GetImage("Search");
			this.imageLogo.Xalign = 0.1f;
			this.imageLogo.Yalign = 0.1f;
			this.imageLogo.Xpad = 2;
			this.PackStart(this.imageLogo, false, false, 2);

			// Initialize Top VBox
			this.table = new Gtk.Table(3, 2, false);
			this.table.ColumnSpacing = 6;
			this.table.RowSpacing = 2;
			this.PackStart(this.table, true, true, 2);

			Gtk.Label label;

			// Initialize Search Label
			label = new Gtk.Label("<b>Search:</b>");
			label.UseMarkup = true;
			label.Xalign = 1.0f;
			this.table.Attach(label, 0, 1, 0, 1);

			// Initialize Entry Search
			this.entrySearch = new Gtk.Entry();
			this.table.Attach(this.entrySearch, 1, 2, 0, 1);

			// Initialize Search By
			label = new Gtk.Label("<b>By:</b>");
			label.UseMarkup = true;
			label.Xalign = 1.0f;
			this.table.Attach(label, 0, 1, 1, 2);

			// Initialize Combo Search Type
			this.comboSearchType = ComboBox.NewText();
			AddSearchType();
			this.comboSearchType.Active = 0;
			this.table.Attach(this.comboSearchType, 1, 2, 1, 2);

			// Initialize Search By
			label = new Gtk.Label("<b>User:</b>");
			label.UseMarkup = true;
			label.Xalign = 1.0f;
			table.Attach(label, 0, 1, 2, 3);

			// Initialize Combo Search Type
			this.comboSearchUser = ComboBox.NewText();
			AddSearchUsers();
			this.comboSearchUser.Active = 0;
			this.table.Attach(this.comboSearchUser, 1, 2, 2, 3);

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
		private void AddSearchType() {
			this.comboSearchType.AppendText("Name");
			this.comboSearchType.AppendText("Extension");
			this.comboSearchType.AppendText("Tag");
		}

		private void AddSearchUsers() {
			this.comboSearchUser.AppendText("All");
		}

		// ============================================
		// PUBLIC Properties
		// ============================================
	}
}
