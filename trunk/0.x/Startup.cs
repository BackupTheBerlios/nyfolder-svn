/* [ Startup.cs ] NyFolder (Windows Startup)
 * Author: Matteo Bertozzi
 * ============================================================================
 * This program (NyFolder) is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
 */

using System;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;

namespace NyFolder {
	public class Startup {
		public static int Main (string[] args) {
			try {
				string path = AppDomain.CurrentDomain.BaseDirectory;
				path = Path.Combine(path, "NyFolder.exe");

				Process proc = new Process();
				proc.EnableRaisingEvents = false;
				proc.StartInfo.FileName = "mono";
				proc.StartInfo.Arguments = path;
				proc.Start();
			} catch {
				MessageBox.Show("Mono Not Found!\nSet environment PATH variable for Mono.\nhttp://www.mono-project.com/", "Mono Not Found");
				return(1);
			}
			return(0);
		}
	}
}
