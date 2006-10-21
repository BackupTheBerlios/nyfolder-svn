/* [ Plugins/Talk/TalkView.cs ] NyFolder (Talk View Plugin)
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

using System;
using System.Text;
using System.Text.RegularExpressions;

using Gtk;

namespace NyFolder.Plugins.Talk {
	public class TalkView : TextView {
		private TextIter endIter;
		private TextMark endMark;
		private TextTag linkTag;
		
		private DateTime lastMessage;
		
		private bool hoveringOverLink;
		private Gdk.Cursor normalCursor;
		private Gdk.Cursor handCursor;
		
		private const string URL_REGEX = @"((\b((news|http|https|ftp|file|irc)://|mailto:|(www|ftp)\.|\S*@\S*\.)|(^|\s)/\S+/|(^|\s)~/\S+)\S*\b/?)";
		
		private static Regex regex = new Regex(URL_REGEX, RegexOptions.IgnoreCase | RegexOptions.Compiled);
		
		private static Regex emoticonRegex;
		
		private static string[,] emoticons = {
			{"face-angel", "0:-)"},
			{"face-angel", "0:)"},
			{"face-crying", ":'("},
			{"face-devil-grin", ">:-)"},
			{"face-devil-grin", ">:)"},
			{"face-devil-sad", ">:-("},
			{"face-devil-sad", ">:("},
			{"face-glasses", "B-)"},
			{"face-glasses", "B)"},
			{"face-kiss", ":-*"},
			{"face-kiss", ":*"},
			{"face-monkey", ":-(|)"},
			{"face-monkey", ":(|)"},
			{"face-plain", ":-|"},
			{"face-plain", ":|"},
			{"face-sad", ":-("},
			{"face-sad", ":("},
			{"face-smile", ":)"},
			{"face-smile-big", ":-D"},
			{"face-smile-big", ":D"},
			{"face-smirk", ":-!"},
			{"face-smirk", ":!"},
			{"face-surprise", ":-0"},
			{"face-surprise", ":0"},
			{"face-wink", ";-)"},
			{"face-wink", ";)"}
		};
		
		// ============================================
		// PUBLIC Methods
		// ============================================
		static TalkView() {
			// Create the emoticon regex
			StringBuilder regex = new StringBuilder(@"(^|(?<=[^\w]))(");
			for (int i = 0; i < emoticons.Length / 2; i++) {
				if (i != 0) {
					regex.Append("|");
				}
				string emoticon = Regex.Escape(emoticons[i, 1]);
				regex.Append(emoticon);
			}
			regex.Append(@")((?=[^\w])|$)");
			
			emoticonRegex = new Regex(regex.ToString(),
				RegexOptions.IgnoreCase | RegexOptions.Compiled);
		}
		
		public TalkView() : base(new TextBuffer(null)) {
			CursorVisible = false;
			Editable = false;
			WrapMode = WrapMode.WordChar;
			
			lastMessage = DateTime.MinValue;
			
			normalCursor = new Gdk.Cursor(Gdk.CursorType.Xterm);
			handCursor = new Gdk.Cursor(Gdk.CursorType.Hand2);
			
			TextTag tag;
			tag = new TextTag("status");
			tag.Foreground = "darkgrey";
			tag.Weight = Pango.Weight.Bold;
			Buffer.TagTable.Add(tag);
			
			tag = new TextTag("error");
			tag.Foreground = "dark red";
			Buffer.TagTable.Add(tag);
			
			tag = new TextTag("time");
			tag.Foreground = "darkgrey";
			tag.Justification = Justification.Center;
			Buffer.TagTable.Add(tag);
			
			linkTag = new TextTag("link");
			linkTag.Foreground = "blue";
			linkTag.Underline = Pango.Underline.Single;
			Buffer.TagTable.Add(linkTag);
			
			tag = new TextTag("nick-self");
			tag.Foreground = "sea green";
			tag.Weight = Pango.Weight.Bold;
			Buffer.TagTable.Add(tag);
			
			tag = new TextTag("nick-other");
			tag.Foreground = "skyblue4";
			tag.Weight = Pango.Weight.Bold;
			Buffer.TagTable.Add(tag);
			
			endIter = Buffer.GetIterAtOffset(0);
			endMark = Buffer.CreateMark("end", endIter, true);
		}

		// ============================================
		// PUBLIC Methods
		// ============================================
		public void InsertError (string error) {
			InsertText(error, "error");
		}
		
		public void InsertStatus (string status) {
			InsertText (status, "status");
		}
		
		public void InsertText (string message, string tag) {
			InsertTimestamp();
			
			Buffer.InsertWithTagsByName(ref endIter, message + "\n", tag);
			ScrollToEnd();
		}
		
		public void InsertMessage (string nick, string message, bool self) {
			InsertTimestamp();
			
			Buffer.InsertWithTagsByName(ref endIter, nick + ": ",
				self ? "nick-self" : "nick-other");
			
			int pos = 0;
			foreach (Match match in regex.Matches(message)) {
				Group group = match.Groups[1];
				
				if (pos < group.Index) {
					InsertMessageWithEmoticons(
						message.Substring(pos, group.Index - pos));
				}
				
				Buffer.InsertWithTagsByName(ref endIter,
					message.Substring(group.Index, group.Length),
					"link");
				
				pos = group.Index + group.Length;
			}
			
			InsertMessageWithEmoticons(message.Substring(pos) + "\n");
			ScrollToEnd();
		}
		
		// ============================================
		// PROTECTED Methods
		// ============================================
		protected override bool OnButtonPressEvent(Gdk.EventButton evnt) {
			if (evnt.Button != 1) {
				return base.OnButtonPressEvent(evnt);
			}
			
			int x, y;
			WindowToBufferCoords (Gtk.TextWindowType.Text, (int)evnt.X,
				(int)evnt.Y, out x, out y);
			
			TextIter iter = GetIterAtLocation(x, y);
			if (iter.HasTag(linkTag)) {
				TextIter end = iter;
				if (iter.BackwardToTagToggle(linkTag) &&
					end.ForwardToTagToggle(linkTag)) {
					return true;
				}
			}
			
			return base.OnButtonPressEvent(evnt);
		}
		
		protected override bool OnMotionNotifyEvent(Gdk.EventMotion evnt) {
			int pointerX, pointerY;
			Gdk.ModifierType pointerMask;
			
			Gdk.Window window = GetWindow(TextWindowType.Text);
			window.GetPointer(out pointerX, out pointerY, out pointerMask);
			
			int x, y;
			this.WindowToBufferCoords(TextWindowType.Widget, pointerX, pointerY,
				out x, out y);
			
			TextIter iter = GetIterAtLocation(x, y);
			bool hovering = iter.HasTag(linkTag);
			
			if (hovering != hoveringOverLink) {
				hoveringOverLink = hovering;
				if (hoveringOverLink) {
					window.Cursor = handCursor;
	        	} else {
	        		window.Cursor = normalCursor;
	        	}
			}
			
			return base.OnMotionNotifyEvent(evnt);
		}
		
		// ============================================
		// PRIVATE Methods
		// ============================================
		private void InsertMessageWithEmoticons(string message) {
			int pos = 0;
			foreach (Match match in emoticonRegex.Matches(message)) {
				if (pos < match.Index) {
					Buffer.Insert(ref endIter,
						message.Substring(pos, match.Index - pos));
				}
				
				string emoticon = match.Value;
				string iconName = GetIconName(emoticon);
				
				if (iconName != null) {
					Gdk.Pixbuf pixbuf = new Gdk.Pixbuf(null, iconName + ".png");
					
					Buffer.InsertPixbuf(ref endIter, pixbuf);
				}
				else
				{
					Buffer.Insert(ref endIter, emoticon);
				}
				
				pos = match.Index + match.Length;
			}
			Buffer.Insert(ref endIter, message.Substring(pos));
		}
		
		private string GetIconName(string emoticon) {
			for (int i = 0; i < emoticons.Length / 2; i++) {
				if (emoticons[i, 1] == emoticon) {
					return emoticons[i, 0];
				}
			}
			return null;
		}
		
		private bool InsertTimestamp() {
			TimeSpan elapsed = DateTime.Now - lastMessage;
			lastMessage = DateTime.Now;
			
			if (elapsed.TotalMinutes > 5) {
				Buffer.InsertWithTagsByName(ref endIter,
					DateTime.Now.ToString("HH:mm") + "\n", "time");
				return true;
			}
			return false;
		}
				
		private void ScrollToEnd() {
			if (Parent is ScrolledWindow) {
				ScrolledWindow scrolledWindow = Parent as ScrolledWindow;
				
				Adjustment adjustment = scrolledWindow.Vadjustment;
				bool scroll = adjustment.Value >= adjustment.Upper -
				adjustment.PageSize;
				
				if (scroll) {
					Buffer.MoveMark(endMark, endIter);
					ScrollMarkOnscreen(endMark);
				}
			}
		}
	}
}
