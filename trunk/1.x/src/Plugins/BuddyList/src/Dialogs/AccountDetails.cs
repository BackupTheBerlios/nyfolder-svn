/* [ Dialogs/AccountDetails.cs ] NyFolder Account Details Dialog
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

using Niry;
using Niry.Utils;
using Niry.Network;

using NyFolder;
using NyFolder.GUI;
using NyFolder.Protocol;
using NyFolder.GUI.Base;

namespace NyFolder.Plugins.BuddyList {
	/// Add Peer Dialog
	public class AccountDetails : GladeDialog {
		// ============================================
		// PRIVATE Members
		// ============================================
		[Glade.WidgetAttribute] private Gtk.CheckButton checkSecureAuth;
		[Glade.WidgetAttribute] private Gtk.Entry entryUserName;
		[Glade.WidgetAttribute] private Gtk.Entry entryPassword;
		[Glade.WidgetAttribute] private Gtk.Image image;

		// ============================================
		// PUBLIC Constructors
		// ============================================
		/// Create New "Add Peer" Dialog
		public AccountDetails() : base("dialog", "AccountDetails.glade") {
			// Initialize Dialog Image
			this.image.Pixbuf = StockIcons.GetPixbuf("Network", 96);
		}

		// ============================================
		// PRIVATE (Methods) Event Handler
		// ============================================

		// ============================================
		// PUBLIC Properties
		// ============================================
		/// Get or Set User UserName
		public string Username {
			set { entryUserName.Text = value; }
			get { return(TextUtils.IsEmpty(entryUserName.Text) ? null : entryUserName.Text); }
		}

		/// Get or Set User UserName
		public string Password {
			set { entryPassword.Text = value; }
			get { return(TextUtils.IsEmpty(entryPassword.Text) ? null : entryPassword.Text); }
		}

		/// Get or Set if User has Secure Authentication
		public bool SecureAuthentication {
			set { checkSecureAuth.Active = value; }
			get { return(checkSecureAuth.Active); }
		}
	}
}
