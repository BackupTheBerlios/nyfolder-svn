/* [ Plugins/Talk/Talk.cs ] NyFolder (Talk Plugin)
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
using System.Threading;
using System.Collections;

using Niry;
using Niry.Utils;
using Niry.Network;
using Niry.GUI.Gtk2;

using NyFolder;
using NyFolder.GUI;
using NyFolder.Utils;
using NyFolder.Protocol;
using NyFolder.GUI.Glue;

namespace NyFolder.Plugins.Talk {
	public class TalkFrame : Gtk.VPaned {
		// ============================================
		// PUBLIC Events
		// ============================================
		public event StringEventHandler Message = null;

		// ============================================
		// PROTECTED Members
		// ============================================
		protected Gtk.TextView inputView = null;
		protected TalkView talkView = null;
		protected UserInfo userInfo = null;

		// ============================================
		// PRIVATE Members
		// ============================================
		private Gtk.ScrolledWindow scrolledWindowOut;
		private Gtk.ScrolledWindow scrolledWindowIn;

		// ============================================
		// PUBLIC Constructors
		// ============================================
		public TalkFrame (UserInfo userInfo) {
			// Initialize User Info
			this.userInfo = userInfo;

			// VPaned Position
			this.Position = 120;

			Gtk.Frame frame;

			// Output Text View
			this.talkView = new TalkView();

			// Output Scrolled Window
			this.scrolledWindowOut = new Gtk.ScrolledWindow(null, null);
			this.scrolledWindowOut.ShadowType = Gtk.ShadowType.None;
			this.scrolledWindowOut.HscrollbarPolicy = Gtk.PolicyType.Automatic;
			this.scrolledWindowOut.VscrollbarPolicy = Gtk.PolicyType.Automatic;
			this.scrolledWindowOut.Add(this.talkView);

			// Output Frame
			frame = new Gtk.Frame();
			frame.Shadow = ShadowType.EtchedOut;
			frame.ShadowType = ShadowType.EtchedOut;
			frame.Add(this.scrolledWindowOut);
			this.Add1(frame);

			// Input Text View
			this.inputView = new Gtk.TextView();
			this.inputView.HeightRequest = 25;
			this.inputView.KeyReleaseEvent += new KeyReleaseEventHandler(OnKeyReleaseEvent);

			// Input Scrolled Window
			this.scrolledWindowIn = new Gtk.ScrolledWindow(null, null);
			this.scrolledWindowIn.ShadowType = Gtk.ShadowType.None;
			this.scrolledWindowIn.HscrollbarPolicy = Gtk.PolicyType.Automatic;
			this.scrolledWindowIn.VscrollbarPolicy = Gtk.PolicyType.Automatic;
			this.scrolledWindowIn.Add(this.inputView);

			// Input Frame
			frame = new Gtk.Frame();
			frame.Shadow = ShadowType.EtchedOut;
			frame.ShadowType = ShadowType.EtchedOut;
			frame.Add(this.scrolledWindowIn);
			this.Add2(frame);

			// Show All
			this.ShowAll();
		}

		// ============================================
		// PUBLIC Methods
		// ============================================
		public void InsertError (string error) {
			this.talkView.InsertError(error);
		}
		
		public void InsertStatus (string status) {
			this.talkView.InsertStatus(status);
		}

		public void InsertMessage (UserInfo userInfo, string message) {
			this.talkView.InsertMessage(userInfo.Name, message, 
										(userInfo == this.userInfo));
		}

		// ============================================
		// PROTECTED (Methods) Event Handlers
		// ============================================

		// ============================================
		// PRIVATE (Methods) Event Handlers
		// ============================================
		private void OnKeyReleaseEvent (object o, Gtk.KeyReleaseEventArgs args) {
			if (args.Event.Key == Gdk.Key.Return && 
				(args.Event.State == Gdk.ModifierType.None || 
				 args.Event.State == Gdk.ModifierType.Mod2Mask))
			{
				int msgLength = this.inputView.Buffer.Text.Length;
				if (msgLength > 1) {
					string msg = inputView.Buffer.Text.Substring(0, msgLength-1);
					// Send Event
					if (Message != null) Message(this, msg);
				}
				this.inputView.Buffer.Clear();
			}
		}

		// ============================================
		// PUBLIC Properties
		// ============================================
		public UserInfo UserInfo {
			get { return(this.userInfo); } 
		}
	}
}
