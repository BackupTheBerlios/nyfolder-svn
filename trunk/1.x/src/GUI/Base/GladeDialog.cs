/* [ GUI/Base/GladeDialog.cs ] NyFolder Abstract Glade Window Class
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

namespace NyFolder.GUI.Base {
	/// Abstract Glade Dialog Class
	public abstract class GladeDialog : GladeWindow {
		// ============================================
		// PUBLIC Constructors
		// ============================================
		/// Initialize Glade Dialog with Window Name & Glade XML File
		public GladeDialog (string name, string gladeFile) :
			base(name, gladeFile)
		{
		}

		/// Initialize Glade Dialog with Window Name & Glade XML Struct
		public GladeDialog (string name, Glade.XML glade) : base(name, glade) {
		}

		// ============================================
		// PUBLIC Methods
		// ============================================
		/// Run The Dialog
		public virtual ResponseType Run() {
			return((ResponseType) Dialog.Run());
		}

		// ============================================
		// PUBLIC Properties
		// ============================================
		/// Get The Dialog
		public Gtk.Dialog Dialog {
			get { return((Gtk.Dialog) Window); }
		}
	}
}
