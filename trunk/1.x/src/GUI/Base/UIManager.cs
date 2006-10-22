/* [ GUI/Base/UIManager.cs ] NyFolder Abstract UIManager Class
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

namespace NyFolder.GUI.Base {
	public abstract class UIManager : Gtk.UIManager {
		// ============================================
		// PUBLIC Events
		// ============================================
		/// Event Raised When Menu Item is Clicked
		public event EventHandler Activated = null;

		// ============================================
		// PROTECTED Members
		// ============================================

		// ============================================
		// PRIVATE Members
		// ============================================
		private ActionGroup actionGroup;

		// ============================================
		// PUBLIC Constructors
		// ============================================
		public UIManager (string groupName) {
			this.actionGroup = new ActionGroup(groupName);
			InsertActionGroup(actionGroup, 0);
		}

		// ============================================
		// PUBLIC Methods
		// ============================================
		/// Update Menu with new Action Entries
		public void AddMenus (string ui, ActionEntry[] entries) {
			AddUiFromString(ui);
			actionGroup.Add(entries);
			EnsureUpdate();
		}

		/// Update Menu with new Toggled Action Entries
		public void AddMenus (string ui, ToggleActionEntry[] entries) {
			AddUiFromString(ui);
			actionGroup.Add(entries);
			EnsureUpdate();
		}

		/// Update Menu with new Radio Action Entries
		public void AddMenus (string ui, RadioActionEntry[] entries) {
			AddUiFromString(ui);
			actionGroup.Add(entries);
			EnsureUpdate();
		}

		/// Set Menu Widget Sensitive
		public void SetSensitive (string path, bool sensitive) {
			Widget widget = GetWidget(path);
			if (widget != null) widget.Sensitive = sensitive;
		}

		// ============================================
		// PRIVATE (Methods) Event Handlers
		// ============================================
		private void ActionActivated (object sender, EventArgs args) {
			if (Activated != null) Activated(sender, args);				
		}

		// ============================================
		// PRIVATE Methods
		// ============================================

		// ============================================
		// PUBLIC Properties
		// ============================================
		/// Get Menu Group Action
		public ActionGroup ActionGroup {
			get { return(this.actionGroup); }
		}
	}
}
