/* [ GUI/StockIcons.cs ] NyFolder (Stock Icons)
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
using System.Reflection;
using System.Collections;

using Gtk;

using Niry;
using Niry.GUI.Gtk2;

namespace NyFolder.GUI {
	public static class StockIcons {
		private static readonly string[] stock_icons = {
			"Download",
			"Send",
			"StockMyFolder",
			"StockNetwork",
			"Proxy"
		};

		private static readonly string[] stock_images_names = {
			"Close",
			"Channel",
			"Directory",
			"Download",
			"Lock",
			"Network",
			"NetworkInsecure",
			"Proxy",
			"InsecureAuth",
			"SecureAuth",
			"Send",

			"MyFolder",
			"MyFolderOffline",
			"MyFolderOfflineInsecure",
			"MyFolderOnline",
			"MyFolderOnlineInsecure",

			// NyFolder Logos
			"NyFolderIcon",
			"NyFolderLogo",
			"NyFolderSmall",

			// File Types
			"FileTypeAc3",
			"FileTypeAiff",
			"FileTypeArj",
			"FileTypeAu",
			"FileTypeBak",
			"FileTypeBin",
			"FileTypeBmp",
			"FileTypeBz2",
			"FileTypeC",
			"FileTypeClass",
			"FileTypeCss",
			"FileTypeDeb",
			"FileTypeDia",
			"FileTypeDoc",
			"FileTypeDvi",
			"FileTypeEps",
			"FileTypeExe",
			"FileTypeGeneric",
			"FileTypeGif",
			"FileTypeGz",
			"FileTypeH",
			"FileTypeJar",
			"FileTypeJava",
			"FileTypeJpg",
			"FileTypeMidi",
			"FileTypeMod",
			"FileTypeMp3",
			"FileTypeOgg",
			"FileTypePbm",
			"FileTypePdf",
			"FileTypePgp",
			"FileTypePhp",
			"FileTypePl",
			"FileTypePng",
			"FileTypePpt",
			"FileTypePs",
			"FileTypePsd",
			"FileTypePy",
			"FileTypePyc",
			"FileTypeRar",
			"FileTypeRb",
			"FileTypeRm",
			"FileTypeRpm",
			"FileTypeRtf",
			"FileTypeSh",
			"FileTypeSql",
			"FileTypeSvg",
			"FileTypeTar",
			"FileTypeTbz",
			"FileTypeTex",
			"FileTypeTga",
			"FileTypeTgz",
			"FileTypeWav",
			"FileTypeXls",
			"FileTypeXml",
			"FileTypeXwd",
			"FileTypeZip"
		};

		// [name] = Gtk.Image
		private static Hashtable stock_images = new Hashtable();

		// ============================================
		// PUBLIC Methods
		// ============================================
		public static void Initialize () {
			Gtk.IconFactory factory = new Gtk.IconFactory();
			factory.AddDefault();
			
			// Stock Icons
			foreach (string name in stock_icons) {
				Gdk.Pixbuf pixbuf = new Gdk.Pixbuf(null, name + ".png");
				Gtk.IconSet iconset = new Gtk.IconSet(pixbuf);				

				factory.Add(name, iconset);
			}

			// Stock Images
			foreach (string name in stock_images_names) {
				stock_images.Add(name, new Gdk.Pixbuf(null, name + ".png"));
			}
		}
		
		public static void AddToStock (string name, Gdk.Pixbuf pixbuf) {
			Gtk.IconFactory factory = new Gtk.IconFactory();
			factory.AddDefault();
			Gtk.IconSet iconset = new Gtk.IconSet(pixbuf);
			factory.Add(name, iconset);
		}

		public static void AddToStockImages (string name, Gdk.Pixbuf pixbuf) {
			stock_images.Add(name, pixbuf);
		}

		public static Gdk.Pixbuf GetPixbuf (string name) {
			return((Gdk.Pixbuf) stock_images[name]);
		}

		public static Gdk.Pixbuf GetPixbuf (string name, int size) {
			return(ImageUtils.Resize((Gdk.Pixbuf) stock_images[name], size, size));
		}

		public static Gdk.Pixbuf GetPixbuf (string name, int width, int height) {
			return(ImageUtils.Resize((Gdk.Pixbuf) stock_images[name], width, height));
		}

		public static Gtk.Image GetImage (string name) {
			return(new Gtk.Image((Gdk.Pixbuf) stock_images[name]));
		}

		public static Gtk.Image GetImage (string name, int size) {
			return(new Gtk.Image(GetPixbuf(name, size, size)));
		}

		public static Gtk.Image GetImage (string name, int width, int height) {
			return(new Gtk.Image(GetPixbuf(name, width, height)));
		}

		public static Gdk.Pixbuf GetFileIconPixbuf (string ext) {
			string type = "FileType" + ext;
			if (ext == null || IsPresent(type) == false) 
				return(GetPixbuf("FileTypeGeneric"));
			return(GetPixbuf(type));
		}

		public static Gtk.Image GetFileIconImage (string ext) {
			string type = "FileType" + ext;
			if (ext == null || IsPresent(type) == false) 
				return(GetImage("FileTypeGeneric"));
			return(GetImage(type));
		}

		public static bool IsPresent (string name) {
			return(stock_images.ContainsKey(name));
		}
	}
}
