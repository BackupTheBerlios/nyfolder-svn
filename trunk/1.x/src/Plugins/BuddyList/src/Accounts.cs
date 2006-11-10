/* [ UsersManager/Accounts.cs ] NyFolder Accounts DB
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
	public class Accounts : IAccounts, IDisposable {
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
		public Accounts() {
			// SQLite File Path
			string dbPath = Path.Combine(Utils.Paths.ConfigDirectory, dbName);

			// Connect To SQLite DB and Open Connection
			this.sqlite = new SQLite(dbPath);
			this.sqlite.Open();

			if (this.sqlite.TableExists("accounts") == false) {
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

		/// Return a String Array That Contains all The Accounts UserName
		public string[] GetAllAccounts() {
			string sql = "SELECT username FROM `accounts`;";
			return(sqlite.ExecuteReadStrings(sql));
		}

		/// Return The User's Password
		public string GetUserPassword (string username) {
			string sql = "SELECT password FROM `accounts` WHERE username=@Name;";
			Hashtable sqlParams = new Hashtable();
			sqlParams.Add("@Name", username);
			return(sqlite.ExecuteReadString(sql, sqlParams));
		}

		/// Return The Username associate at The Specified ID
		public string GetUserName (int id) {
			string sql = "SELECT username FROM `accounts` WHERE id=@Id;";
			Hashtable sqlParams = new Hashtable();
			sqlParams.Add("@Id", id);
			return(sqlite.ExecuteReadString(sql, sqlParams));
		}

		/// Return The Id of The Specified User
		public int GetUserId (string username) {
			string sql = "SELECT id FROM `accounts` WHERE username=@Name;";
			Hashtable sqlParams = new Hashtable();
			sqlParams.Add("@Name", username);
			return(sqlite.ExecuteReadInt(sql, sqlParams));
		}

		/// Return The Id Of The Inserted Account
		public int Insert (string username, string password) {
			string sql = "INSERT INTO `accounts` (username, password) VALUES (@Name, @Password);";
			Hashtable sqlParams = new Hashtable();
			sqlParams.Add("@Name", username);
			sqlParams.Add("@Password", password);
			return(sqlite.ExecuteNonQueryGetID(sql, sqlParams));
		}

		public void Remove (string username) {
			string sql = "DELETE from accounts WHERE username=@Name;";
			Hashtable sqlParams = new Hashtable();
			sqlParams.Add("@Name", username);
			sqlite.ExecuteNonQuery(sql, sqlParams);
		}


		// ============================================
		// PRIVATE Methods
		// ============================================
		// +----------+------------------+
		// | id       | INTEGER          |
		// +----------+------------------+
		// | username | VARCHAR(32 + 24) |
		// +----------+------------------+
		// | password | VARCHAR(32)      |
		// +----------+------------------+
		private void CreateAccountsTable() {
			string sql = "CREATE TABLE accounts (" +
							"id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL," +
							"username VARCHAR(56) NOT NULL UNIQUE," +
							"password VARCHAR(32) NOT NULL" +
						 ");";
			sqlite.ExecuteNonQuery(sql);
		}

		// ============================================
		// PUBLIC Properties
		// ============================================
	}
}
