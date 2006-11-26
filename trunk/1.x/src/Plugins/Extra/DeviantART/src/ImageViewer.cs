/* [ DeviantART/ImageViewer.cs ] DeviantART (Image Viewer)
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
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

using Niry;
using Niry.Utils;
using Niry.Network;
using Niry.GUI.Gtk2;

using NyFolder;
using NyFolder.Utils;
using NyFolder.Protocol;
using NyFolder.GUI.Base;

namespace NyFolder.Plugins.DeviantART {
	/// User Shared Folder Viewer
	public class ImageViewer : Gtk.ScrolledWindow {
		// ============================================
		// PUBLIC Events
		// ============================================

		// ============================================
		// PROTECTED Members
		// ============================================
		protected Gtk.IconView iconView;
		protected ImageStore store;

		// ============================================
		// PRIVATE Members
		// ============================================

		// ============================================
		// PUBLIC Constructors
		// ============================================
		/// Create New Folder Viewer
		public ImageViewer() {
			// Initialize Scrolled Window
			BorderWidth = 0;
			ShadowType = ShadowType.EtchedIn;
			SetPolicy(PolicyType.Automatic, PolicyType.Automatic);

			// Initialize Folder Store
			this.store = new ImageStore();

			// Initialize Icon View
			iconView = new IconView(store);
			iconView.TextColumn = ImageStore.COL_NAME;
			iconView.PixbufColumn = ImageStore.COL_PIXBUF;
			iconView.SelectionMode = SelectionMode.Multiple;

			// Initialize Icon View Events
			iconView.ButtonPressEvent += new ButtonPressEventHandler(OnItemClicked);

			// Add IconView to ScrolledWindow
			Add(iconView);
		}

		~ImageViewer() {
			// Icon View Events
			iconView.ButtonPressEvent -= new ButtonPressEventHandler(OnItemClicked);
		}

		// ============================================
		// PUBLIC Methods
		// ============================================
		public void Add (string path, string name, Gdk.Pixbuf pixbuf) {
			store.Add(path, name, pixbuf);
		}

		// ============================================
		// PROTECTED (Methods) Event Handlers
		// ============================================
		protected void OnItemClicked (object sender, ButtonPressEventArgs args) {
			if (iconView.SelectedItems.Length <= 0)
				return;

			if (args.Event.Device.Source != Gdk.InputSource.Mouse)
				return;

			if (args.Event.Button == 3) {
				Console.WriteLine("TODO: Item Right Clicked");
			}
		}
		
		// ============================================
		// PROTECTED (Methods) Menu Event Handlers
		// ============================================

		// ============================================
		// PRIVATE Methods
		// ============================================

		// ============================================
		// PUBLIC Properties
		// ============================================
		/// Return The Selected Items
		public TreePath[] SelectedItems {
			get { return(this.iconView.SelectedItems); }
		}
	}
}
