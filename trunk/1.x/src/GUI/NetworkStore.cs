/* [ GUI/NetworkStore.cs ] NyFolder (Network Store)
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
using NyFolder.Protocol;
using NyFolder.GUI.Base;

namespace NyFolder.GUI {
	/// Network Store
	public class NetworkStore : Gtk.ListStore {
		// ============================================
		// PUBLIC CONST Members
		// ============================================
		public const int COL_USER_INFO = 0;
		public const int COL_NAME = 1;
		public const int COL_PIXBUF = 2;

		// ============================================
		// PRIVATE Members
		// ============================================
		private UserInfo rmUser = null;

		// ============================================
		// PUBLIC Constructors
		// ============================================
		/// Create New Network Store
		public NetworkStore() : base(typeof(UserInfo), 
									 typeof(string),
									 typeof(Gdk.Pixbuf))
		{
#if false
			// Removed because Generate a Thread error, that throw Exception
			SetSortColumnId(COL_NAME, SortType.Ascending);
			DefaultSortFunc = new TreeIterCompareFunc(StoreSortFunc);
#endif
		}

		// ============================================
		// PUBLIC Methods
		// ============================================
		/// Add New Peer
		public void Add (UserInfo userInfo) {
			Remove(userInfo);

			// Setup Pixbuf
			Gdk.Pixbuf pixbuf;
			if (userInfo.SecureAuthentication == true) {
				if (userInfo.IsOnline == true) {
					pixbuf = StockIcons.GetPixbuf("Network", 74);
				} else {
					pixbuf = StockIcons.GetPixbuf("NetworkOffline", 74);
				}
			} else {
				pixbuf = StockIcons.GetPixbuf("NetworkInsecure", 74);
			}

			lock (this) {
				AppendValues(userInfo, userInfo.GetName(), pixbuf);
			}
		}

		/// Add a List of Users
		public void Add (UserInfo[] users) {
			if (users != null) {
				lock (this) {
					foreach (UserInfo userInfo in users)
						Add(userInfo);
				}
			}
		}

		/// Remove Peer
		public void Remove (UserInfo userInfo) {
			this.rmUser = userInfo;
			this.Foreach(RemoveForeach);
			this.rmUser = null;
		}

		/// Remove a List of Users
		public void Remove (UserInfo[] users) {
			if (users != null) {
				lock (this) {
					foreach (UserInfo userInfo in users)
						Remove(userInfo);
				}
			}
		}

		/// Remove All The Offline User In The Store
		public void RemoveAllOffline() {
			UserInfo[] users = GetOfflineUsers();
			if (users != null) Remove(users);
		}

		/// Return User Info Array Contains all The Stored Offline Users
		public UserInfo[] GetOfflineUsers() {
			ArrayList offlineUsers = new ArrayList();

			lock (this) {
				foreach (object[] row in this) {
					UserInfo userInfo = row[COL_USER_INFO] as UserInfo;
					if (userInfo.IsOnline == false)
						offlineUsers.Add(userInfo);
				}
			}

			return((UserInfo[]) offlineUsers.ToArray(typeof(UserInfo)));
		}

		/// Return Peer UserInfo at TreePath position.
		public UserInfo GetUserInfo (TreePath path) {
			TreeIter iter;
			GetIter(out iter, path);
			return((UserInfo) GetValue(iter, COL_USER_INFO));
		}

		/// Return Peer UserInfo at TreeIter position.
		public UserInfo GetUserInfo (TreeIter iter) {
			return((UserInfo) GetValue(iter, COL_USER_INFO));
		}

		/// Return Peer Name at TreePath position.
		public string GetName (TreePath path) {
			TreeIter iter;
			GetIter(out iter, path);
			return((string) GetValue(iter, COL_NAME));
		}

		/// Return Peer Name at TreeIter position.
		public string GetName (TreeIter iter) {
			return((string) GetValue(iter, COL_NAME));
		}

		/// Return Pixbuf Icon stored at TreePath position. 
		public Gdk.Pixbuf GetPixbuf (TreePath path) {
			TreeIter iter;
			GetIter(out iter, path);
			return((Gdk.Pixbuf) GetValue(iter, COL_PIXBUF));
		}

		/// Return Pixbuf Icon stored at TreeIter position. 
		public Gdk.Pixbuf GetPixbuf (TreeIter iter) {
			return((Gdk.Pixbuf) GetValue(iter, COL_PIXBUF));
		}

		// ============================================
		// PRIVATE Methods
		// ============================================
#if false
		// Removed because Generate a Thread error, that throw Exception
		private int StoreSortFunc (TreeModel model, TreeIter a, TreeIter b) {
			string a_name = (string) model.GetValue(a, COL_NAME);
			string b_name = (string) model.GetValue(b, COL_NAME);
			return(String.Compare(a_name, b_name));
		}
#endif

		private bool RemoveForeach (TreeModel model, TreePath path, TreeIter iter) {
			lock (this.rmUser) {
				//if (GetUserInfo(iter) == this.rmUser) {
				if (GetUserInfo(iter).CompareTo(this.rmUser) == 0) {
					this.Remove(ref iter);
					return(true);
				}
				return(false);
			}
		}
	}
}
