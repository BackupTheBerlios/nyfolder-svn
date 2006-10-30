/* [ GUI/Base/Dialogs.cs ] NyFolder GUI Base Dialogs
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
using System.IO;
using System.Text;

using NyFolder;
using NyFolder.Utils;

namespace NyFolder.GUI.Base {
	/// Utility Dialogs
	public static class Dialogs {
		// ============================================
		// PUBLIC Methods
		// ============================================
		/// Simple Message Error Dialog
		public static void MessageError (string title, string message) {
			MessageDialog dialog;

			dialog = new MessageDialog (null, DialogFlags.Modal, MessageType.Error,
										ButtonsType.Close, true, 
										"<span size='x-large'><b>{0}</b></span>\n\n{1}",
										title, message);
			dialog.Run();
			dialog.Destroy();
		}

		/// Simple Yes/No Question Dialog
		public static bool QuestionDialog (string title, string message) {
			MessageDialog dialog;
			dialog = new MessageDialog(null, DialogFlags.Modal, MessageType.Question, 
										ButtonsType.YesNo, true, 
										"<span size='x-large'><b>{0}</b></span>\n\n{1}",
										title, message);
			bool response = (ResponseType) dialog.Run() == ResponseType.Yes;
			dialog.Destroy();
			return(response);
		}

		/// Save File Dialog
		public static string SaveFile (string path, string fileName) {
			FileChooserDialog dialog = new FileChooserDialog("Save File", null,
															 FileChooserAction.Save);
			dialog.AddButton(Stock.Cancel, ResponseType.Cancel);
			dialog.AddButton(Stock.Save, ResponseType.Ok);

			dialog.SelectMultiple = false;
			dialog.SetCurrentFolder(path);

			FileInfo fileInfo = new FileInfo(fileName);
			dialog.CurrentName = fileInfo.Name;

			string filePath = null;
			do {
				if ((ResponseType) dialog.Run() != ResponseType.Ok) {
					filePath = null;
					break;
				}

				if (File.Exists(dialog.Filename) == false) {
					filePath = dialog.Filename;
					break;
				}

				if (ReplaceExistingFile(dialog.Filename) == true) {
					filePath = dialog.Filename;
					break;
				}
			} while (true);
			dialog.Destroy();
			return(filePath);
		}

		/// Replace Existing File Dialog
		public static bool ReplaceExistingFile (string fileName) {
			FileInfo fileInfo = new FileInfo(fileName);

			StringBuilder msg = new StringBuilder();
			msg.AppendFormat("A file named \"{0}\" already exists.\n", fileInfo.Name);
			msg.Append("Do you want to replace it ?\n\n");
			msg.AppendFormat("The file already exists in \"{0}\".\n", fileInfo.DirectoryName);
			msg.Append("Replacing it will overwrite its contents.");

			return(QuestionDialog("Replace File", msg.ToString()));
		}

		// ============================================
		// PRIVATE Methods
		// ============================================

	}
}
