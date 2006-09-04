/* [ Utils/Debug.cs ] - NyFolder Debug Utils
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
using System.IO;
using System.Text;

namespace NyFolder.Utils {
	public class Debug : IDisposable {
		// PRIVATE Members
		private StringBuilder logBuffer = null;
		private TextWriter logStream = null;
		private string logfile = null;

		// PUBLIC Event
		public event EventHandler Message = null;

		// PRIVATE Members (Singleton Vars)
		private static Debug debug = null;

		// PUBLIC GetInstance() -> Singleton
		public static Debug GetInstance() {
			if (debug == null)
				debug = new Debug();
			return(debug);
		}

		public static Debug GetInstance (string logfile) {
			if (debug == null)
				debug = new Debug(logfile);
			return(debug);
		}

		public void Dispose() {
			this.End();
			if (logfile != null) this.logStream.Close();
		}

		// PRIVATE Constructors
		private Debug() {
			this.logfile = null;
			this.logStream = Console.Error;
			this.Start();
		}

		private Debug (string logfile) {
			this.logfile = logfile;
			this.logStream = new StreamWriter(this.logfile);
			this.Start();
		}

		// PUBLIC Members
		public void Write (string str) {
			logBuffer = new StringBuilder();
			logBuffer.AppendFormat("{0} - ", System.DateTime.Now);
			logBuffer.Append(str);

			// Write on Stream
			logStream.WriteLine(logBuffer.ToString());
			if (Message != null) Message(this, null);
		}

		public void Write (string str, params object[] objs) {
			logBuffer = new StringBuilder();
			logBuffer.AppendFormat("{0} - ", System.DateTime.Now);
			logBuffer.AppendFormat(str, objs);

			// Write on Stream
			logStream.WriteLine(logBuffer.ToString());
			if (Message != null) Message(this, null);
		}


		// PUBLIC Static Members
		public static void Log (string str) {
			if (debug != null) {
				debug.Write(str);
			} else {
				Console.Write("{0} - ", System.DateTime.Now);
				Console.WriteLine(str);
			}
		}

		public static void Log (string str, params object[] objs) {
			if (debug != null) {
				debug.Write(str, objs);
			} else {
				Console.Write("{0} - ", System.DateTime.Now);
				Console.WriteLine(str, objs);
			}
		}

		// PRIVATE Members
		private void Start() {
			Write("Start Debug: {0} {1}", Info.Name, Info.Version);
		}

		public void End() {
			Write("End Debug...");
			logStream.WriteLine();
		}

		// PUBLIC Properties
		public string Buffer {
			get { return(this.logBuffer.ToString()); }
		}
	}
}
