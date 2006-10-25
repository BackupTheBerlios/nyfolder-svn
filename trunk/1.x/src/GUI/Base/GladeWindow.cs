/* [ GUI/Base/GladeWindow.cs ] NyFolder Abstract Glade Window Class
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
	/// Abstract Glade Window Class
	public abstract class GladeWindow {
		// ============================================
		// PRIVATE Members
		// ============================================
		private Gtk.Window window;
		private Glade.XML glade;

		// ============================================
		// PUBLIC Constructors
		// ============================================
		/// Initialize Glade Window with Window Name & Glade XML File
		public GladeWindow (string name, string gladeFile) :
			this(name, new Glade.XML(null, gladeFile, name, null))
		{
		}

		/// Initialize Glade Window with Window Name & Glade XML Struct
		public GladeWindow (string name, Glade.XML glade) {
			this.glade = glade;
			this.window = (Gtk.Window) glade.GetWidget(name);
			this.glade.Autoconnect(this);
		}

		// ============================================
		// PUBLIC Methods
		// ============================================
		/// Destroy Glade Window
		public virtual void Destroy() {
			Window.Destroy();
		}

		// ============================================
		// PROTECTED Properties
		// ============================================
		/// Get Glade XML Struct
		protected Glade.XML Glade {
			get { return(this.glade); }
		}

		// ============================================
		// PUBLIC Properties
		// ============================================
		/// Get Gtk Window (Extract From Glade)
		public Gtk.Window Window {
			get { return(this.window); }
		}
	}
}
