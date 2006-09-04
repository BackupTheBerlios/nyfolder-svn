/* [ Plugins/Talk/TalkOutput.cs ] NyFolder (Talk Editor Output)
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
using Niry.GUI.Gtk2;

using NyFolder;
using NyFolder.GUI;
using NyFolder.Utils;
using NyFolder.Protocol;

namespace NyFolder.Plugins.Talk {
	public class TalkOutput : Gtk.ScrolledWindow {
		protected Gtk.TextView textView;

		public TalkOutput() : base(null, null) {
			ShadowType = Gtk.ShadowType.None;
			HscrollbarPolicy = Gtk.PolicyType.Automatic;
			VscrollbarPolicy = Gtk.PolicyType.Automatic;

			// Init Text View
			this.textView = new Gtk.TextView();
			Add(this.textView);
		}

		public void Prepend (string text) {
			textView.Buffer.Text = text + textView.Buffer.Text;
		}

		public void Append (string text) {
			textView.Buffer.Text += text;
		}
	}
}
