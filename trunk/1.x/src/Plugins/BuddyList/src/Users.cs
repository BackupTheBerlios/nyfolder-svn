/* [ UsersManager/Users.cs ] NyFolder Users DB
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
	public class Users : IUsers, IDisposable {
		// ============================================
		// PUBLIC Const
		// ============================================
		public const string dbName = "users.db";

		// ============================================
		// PRIVATE Members
		// ============================================
		private SQLite sqlite = null;

		// ============================================
		// PUBLIC Constructors
		// ============================================
		/// Open Db Connection With Accounts (Users) Database
		public Users() {
			// SQLite File Path
			string dbPath = Path.Combine(Utils.Paths.ConfigDirectory, dbName);

			// Connect To SQLite DB and Open Connection
			this.sqlite = new SQLite(dbPath);
			this.sqlite.Open();

			if (this.sqlite.TableExists("users") == false) {
				CreateAccountsTable();
			}
		}

		// ============================================
		// PUBLIC Methods
		// ============================================
		/// Close a Connection With Accounts (Uses) Database
		public void Dispose() {
			this.sqlite.Dispose();
			this.sqlite = null;
		}

		public void Remove (string username) {
			string sql = "DELETE from users WHERE username=@Name;";
			Hashtable sqlParams = new Hashtable();
			sqlParams.Add("@Name", username);
			sqlite.ExecuteNonQuery(sql, sqlParams);
		}

		/// Return The Id Of The Inserted User
		public int Insert (string username, bool accept) {
			string sql = "INSERT INTO `users` (username, accept) VALUES (@Name, @Accept);";
			Hashtable sqlParams = new Hashtable();
			sqlParams.Add("@Name", username);
			sqlParams.Add("@Accept", (accept == true) ? 1 : 0);
			return(sqlite.ExecuteNonQueryGetID(sql, sqlParams));
		}

		public void SetAccept (string username, bool accept) {
			string sql = "UPDATE `users` SET accept=@Accept WHERE username=@Name;";
			Hashtable sqlParams = new Hashtable();
			sqlParams.Add("@Name", username);
			sqlParams.Add("@Accept", (accept == true) ? 1 : 0);
			sqlite.ExecuteNonQuery(sql, sqlParams);
		}

		public bool GetAccept (string username) {
			string sql = "SELECT accept FROM users WHERE username=@Name;";
			Hashtable sqlParams = new Hashtable();
			sqlParams.Add("@Name", username);
			return(sqlite.ExecuteReadInt(sql, sqlParams) == 1 ? true : false);
		}

		/// Return The Id of The Specified User
		public int GetUserId (string username) {
			string sql = "SELECT id FROM users WHERE username=@Name;";
			Hashtable sqlParams = new Hashtable();
			sqlParams.Add("@Name", username);
			return(sqlite.ExecuteReadInt(sql, sqlParams));
		}

		/// Return a String Array That Contains all The Accounts UserName
		public string[] GetAllAcceptedUsers() {
			string sql = "SELECT username FROM users WHERE accept=@Accept;";
			Hashtable sqlParams = new Hashtable();
			sqlParams.Add("@Accept", 1);
			return(sqlite.ExecuteReadStrings(sql, sqlParams));
		}

		// ============================================
		// PRIVATE Methods
		// ============================================
		// +----------+------------------+
		// | id       | INTEGER          |
		// +----------+------------------+
		// | username | VARCHAR(32 + 24) |
		// +----------+------------------+
		// | accept   | INTEGER          |
		// +----------+------------------+
		private void CreateAccountsTable() {
			string sql = "CREATE TABLE users (" +
							"id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL," +
							"username VARCHAR(56) NOT NULL UNIQUE," +
							"accept INTEGER NOT NULL" +
						 ");";
			sqlite.ExecuteNonQuery(sql);
		}

		// ============================================
		// PUBLIC Properties
		// ============================================
	}
}
