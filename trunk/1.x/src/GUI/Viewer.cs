/* [ GUI/Viewer.cs ] NyFolder (Abstract Viewer)
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

namespace NyFolder.GUI {
	/// Abstract Viewer
	public abstract class Viewer : Gtk.ScrolledWindow {
		// ============================================
		// PROTECTED Members
		// ============================================
		/// Icon Viewer
		protected Gtk.IconView iconView;

		// ============================================
		// PUBLIC Constructors
		// ============================================
		/// Create New Viewer
		public Viewer() {
			// Initialize Scrolled Window
			BorderWidth = 0;
			ShadowType = ShadowType.EtchedIn;
			SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
		}

		// ============================================
		// PUBLIC Properties
		// ============================================
		/// Return The Selected Items
		public TreePath[] SelectedItems {
			get { return(this.iconView.SelectedItems); }
		}
	}
}
