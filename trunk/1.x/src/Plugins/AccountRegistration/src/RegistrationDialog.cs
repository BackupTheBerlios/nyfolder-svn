/* [ Plugins/AccountReg/RegistrationDialog.cs ] NyFolder Registration Dialog
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
using System.Text;
using System.Text.RegularExpressions;

using Niry;
using Niry.Utils;
using Niry.Network;

using NyFolder;
using NyFolder.GUI;
using NyFolder.Protocol;
using NyFolder.GUI.Base;

namespace NyFolder.Plugins.AccountRegistration {
	/// Account Registration Dialog
	public class RegistrationDialog : GladeDialog {
		// ============================================
		// PRIVATE Members
		// ============================================
		[Glade.WidgetAttribute] private Gtk.Button buttonCheckAvailability;
		[Glade.WidgetAttribute] private Gtk.Entry entryPasswordCheck;
		[Glade.WidgetAttribute] private Gtk.Label labelPassMatch;
		[Glade.WidgetAttribute] private Gtk.Label labelStrength;
		[Glade.WidgetAttribute] private Gtk.Entry entryUserName;
		[Glade.WidgetAttribute] private Gtk.Entry entryPassword;
		[Glade.WidgetAttribute] private Gtk.Entry entryMail;
		[Glade.WidgetAttribute] private Gtk.Image image;

		// ============================================
		// PUBLIC Constructors
		// ============================================
		/// Create New Registration Dialog
		public RegistrationDialog() : 
			base("dialog", new Glade.XML(null, "RegistrationDialog.glade", "dialog", null))
		{
			// Initialize Dialog Image
			this.image.Pixbuf = StockIcons.GetPixbuf("Registration");

			// Initialize Events
			entryPassword.Changed += new EventHandler(OnPasswordChanged);
			entryPasswordCheck.Changed += new EventHandler(OnPasswordCheckChanged);
			buttonCheckAvailability.Clicked += new EventHandler(OnCheckAvailability);
		}

		// ============================================
		// PRIVATE (Methods) Event Handler
		// ============================================
		private void OnPasswordChanged (object sender, EventArgs args) {
			Gtk.Application.Invoke(delegate {
				labelStrength.Markup = "<b>" + PasswordType(entryPassword.Text) + "</b>";
			});
		}

		private void OnPasswordCheckChanged (object sender, EventArgs args) {
			Gtk.Application.Invoke(delegate {
				if (entryPassword.Text != entryPasswordCheck.Text) {
					labelPassMatch.Markup = "<b>Don't Match</b>";
				} else {
					labelPassMatch.Markup = "<b>Match</b>";
				}
			});
		}

		private void OnCheckAvailability (object sender, EventArgs args) {
			if (TextUtils.IsEmpty(entryUserName.Text) == true)
				return;

			Gtk.Application.Invoke(delegate {
				try {
					string resp = HttpRequest.UserNameIsAvailable(entryUserName.Text);
					GUI.Base.Dialogs.MessageError("UserName Availability", resp);
				} catch (Exception e) {
					GUI.Base.Dialogs.MessageError("UserName Availability", e.Message);
					Console.WriteLine(e.StackTrace);
				}
			});
		}

		// ============================================
		// PRIVATE Methods
		// ============================================
		private int PasswordStrength (string password) {
			int strength = 0;
			string[] patterns = new string[] {
				"#[a-z]#", "#[A-Z]#", "#[0-9]#", 
			};

			foreach (string pattern in patterns) {
				if (Regex.IsMatch(password, pattern))
					strength++;
			}

			return(strength);
		}

		private string PasswordType (string password) {
			string[] type = new string[] {
				"weak", "not weak", "acceptable", "strong"
			};
			return(type[PasswordStrength(password)]);
		}

		// ============================================
		// PUBLIC Properties
		// ============================================
		/// Get or Set User UserName
		public string Username {
			set { entryUserName.Text = value; }
			get { return(TextUtils.IsEmpty(entryUserName.Text) ? null : entryUserName.Text); }
		}

		/// Get User Password
		public string Password {
			get { return(TextUtils.IsEmpty(entryPassword.Text) ? null : entryPassword.Text); }
		}

		/// Get User EMail
		public string EMail {
			get { return(TextUtils.IsEmpty(entryMail.Text) ? null : entryMail.Text); }
		}
	}
}
