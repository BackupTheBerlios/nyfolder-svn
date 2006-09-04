/* [ Plugins/Talk/TalkEditor.cs ] NyFolder (Talk Editor)
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
	public delegate void MessageEventHandler (object sender, string message);

	public class TalkEditor : Gtk.VBox {
		// ============================================
		// PUBLIC Events
		// ============================================
		public event MessageEventHandler Message = null;

		// ============================================
		// PROTECTED Members
		// ============================================
		protected Gtk.ScrolledWindow scrolledWindowIn;
		protected Gtk.TextView textViewIn;
		protected TalkOutput talkOutput;
		protected Gtk.VBox vbox;

		// ============================================
		// PRIVATE Members
		// ============================================
		private PeerSocket peer;

		// ============================================
		// PUBLIC Constructors
		// ============================================
		public TalkEditor (PeerSocket peer) : base(false, 2) {
			this.peer = peer;

			// Init Output Box Scrolled Window
			this.talkOutput = new TalkOutput();
			this.PackStart(this.scrolledWindowOut, true, true, 0);

			// Init Output Box Scrolled Window
			this.scrolledWindowIn = new Gtk.ScrolledWindow(null, null);
			this.scrolledWindowIn.ShadowType = Gtk.ShadowType.None;
			this.scrolledWindowIn.HscrollbarPolicy = Gtk.PolicyType.Automatic;
			this.scrolledWindowIn.VscrollbarPolicy = Gtk.PolicyType.Automatic;
			this.PackStart(this.scrolledWindowIn, false, false, 0);

			// Init Text View
			this.textViewIn = new Gtk.TextView();
			this.textViewIn.KeyReleaseEvent += new KeyReleaseEventHandler(OnKeyReleaseEvent);
			this.scrolledWindowIn.Add(this.textViewIn);

			this.ShowAll();
		}

		// ============================================
		// PUBLIC Methods
		// ============================================
		public void AddMessage (string message) {
			talkOutput.Prepend("Me: " + message + "\n");
		}

		public void AddMessage (PeerSocket peer, string message) {
			UserInfo userInfo = peer.Info as UserInfo;
			talkOutput.Prepend(userInfo.Name + ": " + message + "\n");
		}

		// ============================================
		// PROTECTED (Methods) Event Handlers
		// ============================================
		private void OnKeyReleaseEvent (object o, Gtk.KeyReleaseEventArgs args) {
			if (args.Event.Key == Gdk.Key.Return && 
				(args.Event.State == Gdk.ModifierType.None || 
				 args.Event.State == Gdk.ModifierType.Mod2Mask))
			{
				int msgLength = this.textViewIn.Buffer.Text.Length;
				if (msgLength > 1) {
					string msg = textViewIn.Buffer.Text.Substring(0, msgLength-1);
					// Send Event
					if (Message != null) Message(this, msg);
				}
				this.textViewIn.Buffer.Clear();
			}
		}

		// ============================================
		// PRIVATE Methods
		// ============================================

		// ============================================
		// PUBLIC Properties
		// ============================================
		public PeerSocket Peer {
			get { return(this.peer); }
		}
	}
}
