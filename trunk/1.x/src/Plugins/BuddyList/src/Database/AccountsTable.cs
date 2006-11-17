/* [ BuddyList/AccountsTable.cs ] NyFolder Accounts Table
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
	public class AccountsTable : DbTable {
		// ============================================
		// PUBLIC Constructors
		// ============================================
		/// Open Db Connection With Accounts Table
		public AccountsTable() : base("accounts") {
		}

		// ============================================
		// PUBLIC Methods
		// ============================================
		/// Return All The Stored Accounts (Name)
		public string[] GetAllAccounts() {
			string sql = "SELECT username FROM accounts;";
			return(db.ExecuteReadStrings(sql));
		}

		/// Get ID Of The Account
		public int GetUserId (string username) {
			string sql = "SELECT id FROM accounts WHERE username=@Name;";
			Hashtable sqlParams = new Hashtable();
			sqlParams.Add("@Name", username);
			return(db.ExecuteReadInt(sql, sqlParams));
		}

		/// Return The Username associate at The Specified ID
		public string GetUserName (int id) {
			string sql = "SELECT username FROM accounts WHERE id=@Id;";
			Hashtable sqlParams = new Hashtable();
			sqlParams.Add("@Id", id);
			return(db.ExecuteReadString(sql, sqlParams));
		}

		/// Get Account Password From User ID
		public string GetPassword (int id) {
			string sql = "SELECT password FROM accounts WHERE id=@Id;";
			Hashtable sqlParams = new Hashtable();
			sqlParams.Add("@Id", id);
			return(db.ExecuteReadString(sql, sqlParams));
		}

		/// Get Account Password From UserName
		public string GetPassword (string username) {
			string sql = "SELECT password FROM accounts WHERE username=@Name;";
			Hashtable sqlParams = new Hashtable();
			sqlParams.Add("@Name", username);
			return(db.ExecuteReadString(sql, sqlParams));
		}

		/// Set Account Password From User ID
		public void SetPassword (int id, string password) {
			string sql = "UPDATE accounts SET password=@Password WHERE id=@Id;";
			Hashtable sqlParams = new Hashtable();
			sqlParams.Add("@Id", id);
			sqlParams.Add("@Password", password);
			db.ExecuteNonQuery(sql, sqlParams);
		}

		/// Set Account Password From UserName
		public void SetPassword (string username, string password) {
			string sql = "UPDATE accounts SET password=@Password WHERE username=@Name;";
			Hashtable sqlParams = new Hashtable();
			sqlParams.Add("@Name", username);
			sqlParams.Add("@Password", password);
			db.ExecuteNonQuery(sql, sqlParams);
		}

		/// Add New Account
		public int Insert (string username, string password) {
			try {
				string sql = "INSERT INTO accounts (username, password) VALUES (@Name, @Password);";
				Hashtable sqlParams = new Hashtable();
				sqlParams.Add("@Name", username);
				sqlParams.Add("@Password", password);
				return(db.ExecuteNonQueryGetID(sql, sqlParams));
			} catch {
				return(-1);
			}
		}

		/// Remove Account From Id
		public void Remove (int id) {
			string sql = "DELETE from accounts WHERE id=@Id;";
			Hashtable sqlParams = new Hashtable();
			sqlParams.Add("@Id", id);
			db.ExecuteNonQuery(sql, sqlParams);
		}

		/// Remove Account From User Name
		public void Remove (string username) {
			string sql = "DELETE from accounts WHERE username=@Name;";
			Hashtable sqlParams = new Hashtable();
			sqlParams.Add("@Name", username);
			db.ExecuteNonQuery(sql, sqlParams);
		}

		// ============================================
		// PROTECTED Methods
		// ============================================
		// +----------+------------------+
		// | id       | INTEGER          |
		// +----------+------------------+
		// | username | VARCHAR(32 + 24) |
		// +----------+------------------+
		// | password | VARCHAR(32)      |
		// +----------+------------------+
		protected override void CreateAccountsTable() {
			string sql = "CREATE TABLE accounts (" +
							"id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL," +
							"username VARCHAR(56) NOT NULL UNIQUE," +
							"password VARCHAR(32) NOT NULL" +
						 ");";
			db.ExecuteNonQuery(sql);
		}
	}
}
