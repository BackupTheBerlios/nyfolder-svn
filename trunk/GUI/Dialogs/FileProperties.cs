/* [ GUI/Dialogs/FileProperties.cs ] NyFolder (File Properties Dialog)
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

namespace NyFolder.GUI.Dialogs {
	public class FileProperties {
		[Glade.WidgetAttribute]
		private Gtk.Dialog dialog;
		[Glade.WidgetAttribute]
		private Gtk.Image image;

		public FileProperties (string path) {
			XML xml = new XML(null, "FilePropertiesDialog.glade", "dialog", null);
			xml.Autoconnect(this);

			this.image.Pixbuf = StockIcons.GetPixbuf("Directory");
			this.dialog.ShowAll();
		}

		public ResponseType Run() {
			return((ResponseType) dialog.Run());
		}

		public void Destroy() {
			dialog.Destroy();
		}
	}
}
