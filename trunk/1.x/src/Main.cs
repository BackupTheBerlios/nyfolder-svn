/* [ Main.cs ] NyFolder Main
 * Author: Matteo Bertozzi
 * ============================================================================
 * NyFolder is free software; you can redistribute it and/or modify
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

using Gtk;

using System;
using System.Threading;

using Niry;
using Niry.Utils;
using Niry.Network;
using Niry.GUI.Gtk2;

using NyFolder;
using NyFolder.Utils;
using NyFolder.Protocol;
using NyFolder.PluginLib;

namespace NyFolder {
	public class NyFolderMain {
		// ============================================
		// PRIVATE STATIC Members
		// ============================================
		private static P2PManager p2pManager = null;
		private static NyFolderApp nyFolder = null;

		// ============================================
		// PRIVATE STATIC Methods
		// ============================================
		/// Initialize NyFolder Application, Paths, Proxy, Stock Icons...
		private static void InitBase() {
			// Initialize NyFolder Paths
			Debug.Log("Initializing NyFolder Paths...");
			Paths.Initialize();

			// Set Home Directory as Current Environment Path
			Debug.Log("Setting Current Environment Path...");
			Environment.CurrentDirectory = Paths.HomeDirectory;

			// Initialize Proxy Settings
			Debug.Log("Initializing Proxy Settings...");
			Proxy.Initialize();

			// Initialize (Gtk GUI) Stock Icons
			Debug.Log("Initializing Stock Icons...");
			GUI.StockIcons.Initialize();
		}

		/// Initialize NyFolder P2PManager & Network Related
		private static void InitNetwork() {
			// Initialize P2PManager
			Debug.Log("Initializing P2P Manager...");
			p2pManager = P2PManager.GetInstance();

			// Initialize Command Manager
			Debug.Log("Initializing Protocol Manager...");
			CmdManager.Initialize();

			// Initialize Download Manager
			Debug.Log("Initializing Download Manager...");
			DownloadManager.Initialize();

			// Initialize Upload Manager
			Debug.Log("Initializing Upload Manager...");
			UploadManager.Initialize();
		}

		/// Initialize NyFolder Application + Plugins
		private static void InitApplication() {
			// Initialize NyFolder Application
			Debug.Log("Initializing NyFolder...");
			nyFolder = new NyFolderApp();
			nyFolder.Initialize();

			// Initialize Plugins
			Debug.Log("Initializing NyFolder Plugins...");
			PluginManager.Initialize(nyFolder);

			// Start Plugins
			Debug.Log("Starting NyFolder Plugins...");
			PluginManager.RunPlugins();
		}

		// ============================================
		//             APPLICATION MAIN
		// ============================================
		public static int Main (string[] args) {
			// Starting Application
			Debug.Log("{0} {1} Started...", Info.Name, Info.Version);

			do {
				p2pManager = null;
				nyFolder = null;

				// Initialize Gtk Support
				if (Gtk.Application.InitCheck("NyFolder.exe", ref args) == false) {
					PrintErrorMessage("You Don't Have Gtk Support Here...");
					return(1);
				}

				// Initialize Components
				InitBase();
				InitNetwork();
				InitApplication();

				try {
					// Set 'No Restart' Application
					NyFolderApp.Restart = false;

					// Run NyFolder Application
					nyFolder.Run();

					// Run Gtk Main
					Gtk.Application.Run();
				} catch (NyFolderExit) {
					// This is Logout Event :D
					NyFolderApp.Restart = true;
				} catch (Exception e) {
					PrintErrorMessage(e);
					return(1);
				} finally {
					// Uninitialize Plugins
					PluginManager.StopPlugins();

					// Clear Download/Upload Manager
					UploadManager.Clear();
					DownloadManager.Clear();

					// Destroy All P2P Connections
					if (p2pManager != null) p2pManager.Kill();

					// NyFolder Quit Application
					if (NyFolderApp.Restart == false && nyFolder != null)
						nyFolder.Quit();
				}
				Thread.Sleep(1000);
			} while (NyFolderApp.Restart == true);

			// Application Correct Termination
			Debug.Log("{0} {1} Ended...", Info.Name, Info.Version);
			return(0);
		}

		private static void PrintErrorHeader() {
			Console.WriteLine(@"    _______         ___________    .__       .___");
			Console.WriteLine(@"    \      \ ___.__.\_   _____/___ |  |    __| _/___________");
			Console.WriteLine(@"    /   |   <   |  | |    __)/  _ \|  |   / __ |/ __ \_  __ \");
			Console.WriteLine(@"   /    |    \___  | |     \(  <_> )  |__/ /_/ \  ___/|  | \/");
			Console.WriteLine(@"   \____|__  / ____| \___  / \____/|____/\____ |\___  >__|   ");
			Console.WriteLine(@"           \/\/          \/ {0} Error       \/    \/", Info.Version);
			Console.WriteLine("==================================================================");
			Console.WriteLine("Compiler: {0}", Info.Compiler);
			Console.WriteLine("OS Version:  {0}", Environment.OSVersion.Version);
			Console.WriteLine("OS Platform: {0}", Environment.OSVersion.Platform);
			Console.WriteLine("==================================================================");
		}

		private static void PrintException (Exception e) {
			Console.WriteLine("Type:     {0}", e.GetType());
			Console.WriteLine("Source:   {0}", e.Source);
			Console.WriteLine("Message:  {0}", e.Message);
			Console.WriteLine("Stack Trace:");
			Console.WriteLine(e.StackTrace);
		}

		private static void PrintErrorFooter() {
			Console.WriteLine("==================================================================");
			Console.WriteLine("Please, Report Bug(s) To Matteo Bertozzi <theo.bertozzi@gmail.com>");
		}

		private static void PrintErrorMessage (Exception e) {
			PrintErrorHeader();
			PrintException(e);
			PrintErrorFooter();
		}

		private static void PrintErrorMessage (string message) {
			PrintErrorHeader();
			Console.WriteLine(message);
			PrintErrorFooter();
		}
	}
}
