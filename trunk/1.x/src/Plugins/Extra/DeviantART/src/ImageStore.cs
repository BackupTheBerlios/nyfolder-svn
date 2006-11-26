/* [ DeviantART/ImageStore.cs ] DeviantART (Image Store)
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
using NyFolder.GUI.Base;

namespace NyFolder.Plugins.DeviantART {
	public class ImageStore : Gtk.ListStore {
		// ============================================
		// PUBLIC CONST Members
		// ============================================
		public const int COL_PATH = 0;
		public const int COL_NAME = 1;
		public const int COL_PIXBUF = 2;

		// ============================================
		// PUBLIC Events
		// ============================================

		// ============================================
		// PROTECTED Members
		// ============================================

		// ============================================
		// PRIVATE Members
		// ============================================
		private Gtk.TreeIter fIter;
		private string fPath;

		// ============================================
		// PUBLIC Constructors
		// ============================================
		public ImageStore() : base(typeof(string), 
									typeof(string),
									typeof(Gdk.Pixbuf))
		{
			fIter = Gtk.TreeIter.Zero;
			SetSortColumnId(COL_NAME, SortType.Ascending);
			DefaultSortFunc = new TreeIterCompareFunc(StoreSortFunc);
		}

		// ============================================
		// PUBLIC Methods
		// ============================================
		public void Add (string path, string name, Gdk.Pixbuf pixbuf) {
			AppendValues(path, name, pixbuf);
		}

		/// Remove File or Directory
		public void Remove (string path) {
			Gtk.TreeIter iter = GetIter(path);
			this.Remove(ref iter);
		}

		/// Return TreeIter relative to specified path
		public Gtk.TreeIter GetIter (string path) {
			this.fPath = path;
			this.fIter = Gtk.TreeIter.Zero;
			this.Foreach(GetIterForeach);
			this.fPath = null;
			return(this.fIter);
		}

		/// Return File Path string stored at TreePath position
		public string GetFilePath (TreePath path) {
			TreeIter iter;
			GetIter(out iter, path);
			return((string) GetValue(iter, COL_PATH));
		}

		/// Return File Path string stored at TreeIter position
		public string GetFilePath (TreeIter iter) {
			return((string) GetValue(iter, COL_PATH));
		}

		/// Return File Name string stored at TreePath position
		public string GetFileName (TreePath path) {
			TreeIter iter;
			GetIter(out iter, path);
			return((string) GetValue(iter, COL_NAME));
		}

		/// Return File Name string stored at TreeIter position
		public string GetFileName (TreeIter iter) {
			return((string) GetValue(iter, COL_NAME));
		}

		/// Return File Icon Pixbuf stored at TreePath position
		public Gdk.Pixbuf GetPixbuf (TreePath path) {
			TreeIter iter;
			GetIter(out iter, path);
			return((Gdk.Pixbuf) GetValue(iter, COL_PIXBUF));
		}

		/// Return File Icon Pixbuf stored at TreeIter position
		public Gdk.Pixbuf GetPixbuf (TreeIter iter) {
			return((Gdk.Pixbuf) GetValue(iter, COL_PIXBUF));
		}

		/// Set New Icon Pixbuf for File at TreeIter Positions
		public void SetPixbuf (TreeIter iter, Gdk.Pixbuf pixbuf) {
			SetValue(iter, COL_PIXBUF, pixbuf);
		}

		// ============================================
		// PROTECTED Methods
		// ============================================

		// ============================================
		// PRIVATE Methods
		// ============================================
		private int StoreSortFunc (TreeModel model, TreeIter a, TreeIter b) {
			// Sort Folders Before Files
			string a_name = (string) model.GetValue(a, COL_NAME);
			string b_name = (string) model.GetValue(b, COL_NAME);
	
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
	}
}
