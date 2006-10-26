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
		private static void SplashInit() {
			// Initialize Splash Screen
			Gdk.Pixbuf splashPixbuf = new Gdk.Pixbuf(null, "NFSplash.png");
			SplashScreen splash = new SplashScreen("NyFolder", splashPixbuf);
			splash.TextColor = new Cairo.Color(0xff, 0xff, 0xff, 1.0);
			splash.Run();

			splash.Update("Initializing Application...", 1, 11);

			InitBase(ref splash);
			InitNetwork(ref splash);
			InitApplication(ref splash);

			splash.Update("Running Application...", 11, 11);

			// Destroy Splash Screen
			splash.Dispose();
		}

		/// Initialize NyFolder Application, Paths, Proxy, Stock Icons...
		private static void InitBase (ref SplashScreen splash) {
			// Initialize NyFolder Paths
			splash.Update("Initializing NyFolder Paths...", 2, 11);
			Paths.Initialize();

			// Set Home Directory as Current Environment Path
			splash.Update("Setting Current Environment Path...", 3, 11);
			Environment.CurrentDirectory = Paths.HomeDirectory;

			// Initialize Proxy Settings
			splash.Update("Initializing Proxy Settings...", 4, 11);
			Proxy.Initialize();

			// Initialize (Gtk GUI) Stock Icons
			splash.Update("Initializing Stock Icons...", 5, 11);
			GUI.StockIcons.Initialize();
		}

		/// Initialize NyFolder P2PManager & Network Related
		private static void InitNetwork (ref SplashScreen splash) {
			// Initialize P2PManager
			splash.Update("Initializing P2P Manager...", 6, 11);
			p2pManager = P2PManager.GetInstance();

			// Initialize Command Manager
			splash.Update("Initializing Protocol Manager...", 7, 11);
			CmdManager.Initialize();
		}

		/// Initialize NyFolder Application + Plugins
		private static void InitApplication (ref SplashScreen splash) {
			// Initialize NyFolder Application
			splash.Update("Initializing NyFolder...", 8, 11);
			nyFolder = new NyFolderApp();
			nyFolder.Initialize();

			// Initialize Plugins
			splash.Update("Initializing NyFolder Plugins...", 9, 11);
			PluginManager.Initialize(nyFolder);

			// Start Plugins
			splash.Update("Starting NyFolder Plugins...", 10, 11);
			PluginManager.RunPlugins();
		}

		// ============================================
		//             APPLICATION MAIN
		// ============================================
		// Horrible Main :D
		public static int Main (string[] args) {
			try {
				// Initialize Gtk Support
				if (Gtk.Application.InitCheck("NyFolder.exe", ref args) == false) {
					PrintErrorMessage("You Don't Have Gtk Support Here...");
					return(1);
				}

				// Run Splash Screen and Initialize Components
				SplashInit();
			} catch (Exception e) {
				if (nyFolder != null) nyFolder.Quit();
				if (p2pManager != null) p2pManager.Kill();

				// Print Error Message
				PrintErrorMessage(e);
				return(1);
			}

			do {
				try {
					// Set 'No Restart' Application
					NyFolderApp.Restart = false;

					// Run NyFolder Application
					nyFolder.Run();

					// Run Gtk Main
					Gtk.Application.Run();
				} catch (NyFolderExit) {
					// This is Logout Event :D
				} catch (Exception e) {
					PrintErrorMessage(e);
					return(1);
				} finally {
					if (NyFolderApp.Restart != true) {
						if (nyFolder != null) nyFolder.Quit();
						if (p2pManager != null) p2pManager.Kill();
					}
				}
			} while (NyFolderApp.Restart == true);
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
