/* [ Plugins/ImagePreview/ImagePreview.cs ] NyFolder (ImagePreview Plugin)
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

using Niry;
using Niry.Utils;

using NyFolder;
using NyFolder.GUI;
using NyFolder.Utils;
using NyFolder.Protocol;

namespace NyFolder.Plugins.ImagePreview {
	public class ImagePreview : Plugin {
		// ============================================
		// PUBLIC Events
		// ============================================

		// ============================================
		// PROTECTED Members
		// ============================================
		protected INyFolder nyFolder = null;

		// ============================================
		// PRIVATE Members
		// ============================================
		private NotebookViewer notebookViewer = null;

		// ============================================
		// PUBLIC Constructors
		// ============================================
		public ImagePreview() {
		}

		~ImagePreview() {
		}

		// ============================================
		// PUBLIC Methods
		// ============================================
		public override void Initialize (INyFolder nyFolder) {
			this.nyFolder = nyFolder;
			this.nyFolder.MainWindowStarted += new BlankEventHandler(OnMainWindowStarted);
			this.nyFolder.QuittingApplication += new BlankEventHandler(OnMainAppQuit);
		}

		// ============================================
		// PROTECTED (Methods) Event Handlers
		// ============================================
		protected void OnMainWindowStarted (object sender) {
			// Initialize Notebook Viewer Events
			this.notebookViewer = this.nyFolder.Window.NotebookViewer;
			this.notebookViewer.FileAdded += new ObjectEventHandler(OnFileAdded);
		}

		protected void OnMainAppQuit (object sender) {
			// Notebook Viewer Events
			this.notebookViewer.FileAdded -= new ObjectEventHandler(OnFileAdded);
			this.notebookViewer = null;
		}

		protected void OnFileAdded (object sender, object storeIter) {
			FolderViewer folderViewer = sender as FolderViewer;
			Gtk.TreeIter iter = (Gtk.TreeIter) storeIter;
			FolderStore store = folderViewer.Store;

#if false
"wmf", "ani", "bmp", "gif", "ico", "jpg", "jpeg", "pcx", "png", "pnm", "ras", "tga", "tiff", "wbmp", "xbm", "xpm", "svg"
#endif

			// Request Image Thumb if it's Image
			// Check Extension First...
			// if (Extension is NULL) Try To Request
			Console.WriteLine("File Added To: {0}", folderViewer.UserInfo.Name);
			Console.WriteLine(" - Name: {0}", store.GetFileName(iter));
			Console.WriteLine(" - Ext: {0}", Path.GetExtension(store.GetFilePath(iter)));
		}

		// ============================================
		// PRIVATE Methods
		// ============================================

		// ============================================
		// PUBLIC Properties
		// ============================================
	}
}
