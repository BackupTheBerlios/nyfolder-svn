/* [ Plugins/DownloadManager/GUI/FileProgressStore.cs ] NyFolder (Plugin)
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
using System.Collections;

using Niry;
using Niry.Utils;

using NyFolder;
using NyFolder.Utils;
using NyFolder.Protocol;

namespace NyFolder.Plugins.DownloadManager.GUI {
	public class FileProgressStore : Gtk.ListStore {
		private Hashtable iters = null;

		// ============================================
		// PUBLIC Constructors
		// ============================================
		public FileProgressStore() : 
			base(typeof(string),
				 typeof(string), 
				 typeof(int),
				 typeof(string), 
				 typeof(object))
		{
			// 0. User Name
			// 1. File name
			// 2. Percent
			// 3. File Size
			// 4. Object
			iters = new Hashtable();
		}

		// ============================================
		// PUBLIC Methods
		// ============================================
		public void Add (FileSender fs) {
			if (iters.ContainsKey(fs) == false) {
				Gtk.TreeIter iter;

				UserInfo userInfo = fs.Peer.Info as UserInfo;
				string fileSize = FileUtils.GetSizeString(fs.FileSize);
				iter = AppendValues(userInfo.Name, fs.FileName, 
									fs.SendedPercent, fileSize, fs);
				iters.Add(fs, iter);
			}
		}

		public void Add (FileReceiver fr) {
			if (iters.ContainsKey(fr) == false) {
				Gtk.TreeIter iter;

				UserInfo userInfo = fr.Peer.Info as UserInfo;
				string fileSize = FileUtils.GetSizeString(fr.FileSize);
				iter = AppendValues(userInfo.Name, fr.FileName, 
									fr.ReceivedPercent, fileSize, fr);
				iters.Add(fr, iter);
			}
		}

		public void Remove (FileSender fs) {
			if (iters.ContainsKey(fs) == true) {
				Gtk.TreeIter iter = (Gtk.TreeIter) iters[fs] ;
				Remove(ref iter);
				iters.Remove(fs);
			}
		}

		public void Remove (FileReceiver fr) {
			if (iters.ContainsKey(fr) == true) {
				Gtk.TreeIter iter = (Gtk.TreeIter) iters[fr] ;
				Remove(ref iter);
				iters.Remove(fr);
			}
		}

		public void Update (FileSender fs) {
			lock (iters) {
				if (iters.ContainsKey(fs) == true) {
					Gtk.TreeIter iter = (Gtk.TreeIter) iters[fs] ;
					SetValue(iter, 2, fs.SendedPercent);
				}
			}
		}

		public void Update (FileReceiver fr) {
			lock (iters) {
				if (iters.ContainsKey(fr) == true) {
					Gtk.TreeIter iter = (Gtk.TreeIter) iters[fr] ;
					SetValue(iter, 2, fr.ReceivedPercent);
				}
			}
		}

		public object Lookup (TreeIter iter) {
			return(GetValue(iter, 4));
		}
	}
}
