/* [ BuddyList/DbTable.cs ] NyFolder Abstract DB Table
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
using System.Data;
using System.Collections;

using Niry;
using Niry.Utils;
using Niry.Database;

using NyFolder;
using NyFolder.Utils;
using NyFolder.Protocol;

namespace NyFolder.Plugins.BuddyList {
	public abstract class DbTable : IDisposable {
		// ============================================
		// PUBLIC Const
		// ============================================
		public const string dbName = "users.db";

		// ============================================
		// PROTECTED Members
		// ============================================
		protected SQLite db = null;

		// ============================================
		// PUBLIC Constructors
		// ============================================
		public DbTable (string table) {
			// SQLite File Path
			string dbPath = Path.Combine(Utils.Paths.ConfigDirectory, dbName);

			// Connect To SQLite DB and Open Connection
			db = new SQLite(dbPath);
			db.Open();

			// Create Table if doesn't exists.
			if (db.TableExists(table) == false)
				CreateAccountsTable();
		}

		// ============================================
		// PUBLIC Methods
		// ============================================
		public void Dispose() {
			db.Dispose();
			db = null;
		}

		// ============================================
		// PROTECTED Methods
		// ============================================
		protected abstract void CreateAccountsTable();
	}
}
