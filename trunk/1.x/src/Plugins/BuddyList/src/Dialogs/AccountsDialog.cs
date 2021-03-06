/* [ Dialogs/AccountsDialog.cs ] NyFolder Account Details Dialog
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
using Glade;

using System;

using Niry;
using Niry.Utils;
using Niry.Network;

using NyFolder;
using NyFolder.Protocol;
using NyFolder.GUI.Base;

namespace NyFolder.Plugins.BuddyList {
	/// Add Peer Dialog
	public class AccountsDialog : GladeDialog {
		// ============================================
		// PRIVATE Members
		// ============================================
		[Glade.WidgetAttribute] private Gtk.TreeView treeView;
		[Glade.WidgetAttribute] private Gtk.Button buttonEdit;
		[Glade.WidgetAttribute] private Gtk.Button buttonDelete;

		// ============================================
		// PUBLIC Constructors
		// ============================================
		/// Create New "Add Peer" Dialog
		public AccountsDialog() : base("dialog", new XML(null, "AccountsDialog.glade", "dialog", null))
		{
			TreeStore store = new TreeStore(typeof(string), typeof(bool));

			store.AppendValues("Demo 0", true);
			store.AppendValues("Demo 1", false);
			store.AppendValues("Demo 1", false);

			treeView.Model = store;

			treeView.AppendColumn("Demo", new CellRendererText(), "text", 0);
			treeView.AppendColumn("Data", new CellRendererToggle(), "active", 1);
		}

		// ============================================
		// PRIVATE (Methods) Event Handler
		// ============================================

		// ============================================
		// PUBLIC Properties
		// ============================================
	}
}
