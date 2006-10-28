/* [ GUI/Dialogs/ProxySettings.cs ] NyFolder Proxy Settings Dialog
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
using Niry.Network;

using NyFolder;
using NyFolder.Protocol;
using NyFolder.GUI.Base;

namespace NyFolder.GUI.Dialogs {
	/// Proxy Settings Dialog
	public class ProxySettings : GladeDialog {
		// ============================================
		// PRIVATE GLADE Members
		// ============================================
		[Glade.WidgetAttribute] private Gtk.Image image;
		[Glade.WidgetAttribute] private Gtk.VBox vbox;

		// ============================================
		// PRIVATE Members
		// ============================================
		private Niry.GUI.Gtk2.ProxySettings proxy;

		// ============================================
		// PUBLIC Constructors
		// ============================================
		/// Create New Proxy Settings Dialog
		public ProxySettings() : base("dialog", "ProxySettingsDialog.glade") {
			// Add Proxy Settings Box
			proxy = new Niry.GUI.Gtk2.ProxySettings();
			this.vbox.PackStart(proxy, true, true, 2);
			proxy.ShowAll();

			// Initialize Dialog Image
			this.image.Pixbuf = StockIcons.GetPixbuf("Proxy", 48);
		}

		// ============================================
		// PUBLIC Properties
		// ============================================
		/// Get or Set if Proxy is Enabled
		public bool EnableProxy {
			set { proxy.EnableProxy = value; }
			get { return(proxy.EnableProxy); }
		}

		/// Get or Set if Proxy uses Auth
		public bool UseProxyAuth {
			set { proxy.UseProxyAuth = value; }
			get { return(proxy.UseProxyAuth); }
		}

		/// Get or Set Proxy Hostname
		public string Host {
			set { proxy.Host = value; }
			get { return(proxy.Host); }
		}

		/// Get or Set Proxy Port
		public int Port {
			set { proxy.Port = value; }
			get { return(proxy.Port); }
		}

		/// Get or Set Proxy Auth Username
		public string Username {
			set { proxy.Username = value; }
			get { return(proxy.Username); }
		}

		/// Get or Set Proxy Auth Password
		public string Password {
			set { proxy.Password = value; }
			get { return(proxy.Password); }
		}
	}
}
