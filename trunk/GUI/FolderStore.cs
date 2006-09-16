/* [ GUI/FolderStore.cs ] NyFolder (Folder Store)
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

namespace NyFolder.GUI {
	public class FolderStore : Gtk.ListStore {
		// ============================================
		// PUBLIC CONST Members
		// ============================================
		public const int COL_PATH = 0;
		public const int COL_NAME = 1;
		public const int COL_PIXBUF = 2;
		public const int COL_IS_DIRECTORY = 3;

		// ============================================
		// PUBLIC Events
		// ============================================
		public event ObjectEventHandler DirectoryAdded = null;
		public event ObjectEventHandler FileAdded = null;

		// ============================================
		// PROTECTED Members
		// ============================================
		protected bool showHiddenFile;

		// ============================================
		// PRIVATE Members
		// ============================================
		private Gtk.TreeIter fIter;
		private string fPath;

		// ============================================
		// PUBLIC Constructors
		// ============================================
		public FolderStore() : base(typeof(string), 
									typeof(string),
									typeof(Gdk.Pixbuf),
									typeof(bool))
		{
			showHiddenFile = true;
			fIter = Gtk.TreeIter.Zero;
			SetSortColumnId(COL_NAME, SortType.Ascending);
			DefaultSortFunc = new TreeIterCompareFunc(StoreSortFunc);
		}

		// ============================================
		// PUBLIC Methods
		// ============================================
		public void Add (string path) {
			if (Directory.Exists(path) == true) {
				AddDirectory(path);
			} else if (File.Exists(path) == true) {
				AddFile(path);
			}
		}

		public void AddFile (string path) {
			FileInfo fileInfo = new FileInfo(path);
			string ext = GetIconTypeName(fileInfo);
			Gdk.Pixbuf pixbuf = StockIcons.GetFileIconPixbuf(ext);
			
			Gtk.TreeIter iter;
			iter = this.AppendValues(path, fileInfo.Name, pixbuf, false);

			// File Added Event
			if (FileAdded != null) FileAdded(this, iter);
		}

		public void AddDirectory (string path) {
			DirectoryInfo dirInfo = new DirectoryInfo(path);
			Gdk.Pixbuf pixbuf = StockIcons.GetPixbuf("Directory");

			Gtk.TreeIter iter;
			iter = this.AppendValues(path, dirInfo.Name, pixbuf, true);

			// Directory Added Event
			if (DirectoryAdded != null) DirectoryAdded(this, iter);
		}

		public void Fill (string path) {
			// Now Go Through The Directory and Extract All The File Information
			if (Directory.Exists(path) == false)
				return;

			// Get Root Directory
			DirectoryInfo rootDirectory = new DirectoryInfo(path);

			// Get SubDirectory
			foreach (DirectoryInfo dir in rootDirectory.GetDirectories()) {
				if (this.showHiddenFile == true || !dir.Name.StartsWith("."))
					AddDirectory(dir.FullName);
			}

			// Get Files
			foreach (FileInfo file in rootDirectory.GetFiles()) {
				if (this.showHiddenFile == true || !file.Name.StartsWith("."))
					AddFile(file.FullName);
			}
		}

		public void Remove (string path) {
			Gtk.TreeIter iter = GetIter(path);
			this.Remove(ref iter);
		}

		public Gtk.TreeIter GetIter (string path) {
			this.fPath = path;
			this.fIter = Gtk.TreeIter.Zero;
			this.Foreach(GetIterForeach);
			this.fPath = null;
			return(this.fIter);
		}

		public string GetFilePath (TreePath path) {
			TreeIter iter;
			GetIter(out iter, path);
			return((string) GetValue(iter, COL_PATH));
		}

		public string GetFilePath (TreeIter iter) {
			return((string) GetValue(iter, COL_PATH));
		}

		public string GetFileName (TreePath path) {
			TreeIter iter;
			GetIter(out iter, path);
			return((string) GetValue(iter, COL_NAME));
		}

		public string GetFileName (TreeIter iter) {
			return((string) GetValue(iter, COL_NAME));
		}

		public Gdk.Pixbuf GetPixbuf (TreePath path) {
			TreeIter iter;
			GetIter(out iter, path);
			return((Gdk.Pixbuf) GetValue(iter, COL_PIXBUF));
		}

		public Gdk.Pixbuf GetPixbuf (TreeIter iter) {
			return((Gdk.Pixbuf) GetValue(iter, COL_PIXBUF));
		}

		public bool GetIsDirectory (TreePath path) {
			TreeIter iter;
			GetIter(out iter, path);
			return((bool) GetValue(iter, COL_IS_DIRECTORY));
		}

		public bool GetIsDirectory (TreeIter iter) {
			return((bool) GetValue(iter, COL_IS_DIRECTORY));
		}

		public void SetPixbuf (TreeIter iter, Gdk.Pixbuf pixbuf) {
			SetValue(iter, COL_PIXBUF, pixbuf);
		}

		// ============================================
		// PROTECTED Methods
		// ============================================
		protected string GetIconTypeName (FileInfo fileInfo) {
			string ext = fileInfo.Extension;
			if (ext != null && ext != "") {
				ext = ext.Remove(0, 1);
				char[] extchr = ext.ToCharArray();
				extchr[0] = Char.ToUpper(extchr[0]);
				ext = new String(extchr);
			} else {
				ext = null;
			}
			return(ext);
		}

		// ============================================
		// PRIVATE Methods
		// ============================================
		private int StoreSortFunc (TreeModel model, TreeIter a, TreeIter b) {
			// Sort Folders Before Files
			bool a_is_dir = (bool) model.GetValue(a, COL_IS_DIRECTORY);
			bool b_is_dir = (bool) model.GetValue(b, COL_IS_DIRECTORY);
			string a_name = (string) model.GetValue(a, COL_NAME);
			string b_name = (string) model.GetValue(b, COL_NAME);
	
			if (!a_is_dir && b_is_dir) {
				return(1);
			} else if (a_is_dir && !b_is_dir) {
				return(-1);
			}
	
			return(String.Compare(a_name, b_name));
		}

		private bool GetIterForeach (TreeModel model, TreePath path, TreeIter iter) {
			lock (this.fPath) {
				if (GetFilePath(iter) == this.fPath) {
					this.fIter = iter;
					return(true);
				}
				return(false);
			}
		}

		// ============================================
		// PUBLIC Properties
		// ============================================
		public bool ShowHiddenFile {
			get { return(this.showHiddenFile); }
			set { this.showHiddenFile = value; }
		}
	}
}
